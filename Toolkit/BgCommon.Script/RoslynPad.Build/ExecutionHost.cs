using System.Buffers;
using System.Buffers.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BgCommon.Core;
using BgLogger;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Nerdbank.Streams;
using NuGet.Versioning;
using RoslynPad.Build.ILDecompiler;
using RoslynPad.Roslyn;

namespace RoslynPad.Build;

/// <summary>
/// An <see cref="IExecutionHost"/> implementation that compiles to disk and executes in separated processes.
/// </summary>
internal partial class ExecutionHost : IExecutionHost, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new BooleanConverter(),
        },
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private static readonly ImmutableArray<string> BinFilesToRename = new ImmutableArray<string>()
    {
        "{0}.deps.json",
        "{0}.runtimeconfig.json",
        "{0}.exe.config",
    };

    private static readonly ImmutableArray<byte> NewLine = [.. Encoding.UTF8.GetBytes(Environment.NewLine)];

    private readonly string version;
    private readonly ExecutionHostParameters parameters;
    private readonly IRoslynHost roslynHost;
    private readonly IAnalyzerAssemblyLoader analyzerAssemblyLoader;
    private readonly SortedSet<LibraryRef> libraries;
    private readonly ImmutableArray<string> imports;
    private readonly SemaphoreSlim lockSlim;
    private readonly SyntaxTree scriptInitSyntax;
    private readonly SyntaxTree moduleInitAttributeSyntax;
    private readonly SyntaxTree moduleInitSyntax;
    private readonly SyntaxTree importsSyntax;
    private readonly LibraryRef runtimeAssemblyLibraryRef;
    private readonly LibraryRef runtimeNetFxAssemblyLibraryRef;
    private readonly string restoreCachePath;
    private readonly object ctsLock;
    private CancellationTokenSource? executeCts;
    private Task? restoreTask;
    private CancellationTokenSource? restoreCts;
    private ExecutionPlatform? platform;
    private string? restorePath;
    private string? assemblyPath;
    private string name;
    private bool running;
    private bool initializeBuildPathAfterRun;
    private TextWriter? processInputStream;
    private string? dotNetExecutable;

    public ExecutionHost(ExecutionHostParameters parameters, IRoslynHost roslynHost)
    {
        this.version = typeof(ExecutionContext).Assembly.GetName().Version?.ToString() ?? string.Empty;
        this.name = string.Empty;
        this.parameters = parameters;
        this.roslynHost = roslynHost;
        this.analyzerAssemblyLoader = this.roslynHost.GetService<IAnalyzerAssemblyLoader>();
        this.libraries = [];
        this.imports = parameters.Imports;
        this.ctsLock = new object();
        this.lockSlim = new SemaphoreSlim(1, 1);

        this.scriptInitSyntax = SyntaxFactory.ParseSyntaxTree(BuildCode.ScriptInit, roslynHost.ParseOptions.WithKind(SourceCodeKind.Script));

        var regularParseOptions = roslynHost.ParseOptions.WithKind(SourceCodeKind.Regular);
        this.moduleInitAttributeSyntax = SyntaxFactory.ParseSyntaxTree(BuildCode.ModuleInitAttribute, regularParseOptions);
        this.moduleInitSyntax = SyntaxFactory.ParseSyntaxTree(BuildCode.ModuleInit, regularParseOptions);
        this.importsSyntax = SyntaxFactory.ParseSyntaxTree(GetGlobalUsings(), regularParseOptions);

        this.MetadataReferences = [];

        // _runtimeAssemblyLibraryRef = LibraryRef.Reference(Path.Combine(AppContext.BaseDirectory, "runtimes", "net", "RoslynPad.Runtime.dll"));
        // _runtimeNetFxAssemblyLibraryRef = LibraryRef.Reference(Path.Combine(AppContext.BaseDirectory, "runtimes", "netfx", "RoslynPad.Runtime.dll"));
        this.runtimeAssemblyLibraryRef = LibraryRef.Reference(Path.Combine(AppContext.BaseDirectory, "RoslynPad.Runtime.dll"));
        this.runtimeNetFxAssemblyLibraryRef = LibraryRef.Reference(Path.Combine(AppContext.BaseDirectory, "RoslynPad.Runtime.dll"));
        this.restoreCachePath = FileNames.ScriptsRestorePath;
    }

    public ExecutionPlatform Platform
    {
        get
        {
            if (platform == null)
            {
                throw new InvalidOperationException("No platform selected");
            }

            return platform;
        }
        set
        {
            platform = value;
            InitializeBuildPath(stopProcess: true);
        }
    }

    private bool IsScript => parameters.SourceCodeKind == SourceCodeKind.Script;

    public bool UseCache => Platform.FrameworkVersion?.Major >= 6;

    public bool HasPlatform => platform != null;

    public string DotNetExecutable
    {
        get => HasDotNetExecutable ? dotNetExecutable : throw new InvalidOperationException("Missing dotnet");
        set => dotNetExecutable = value;
    }

    [MemberNotNullWhen(true, nameof(dotNetExecutable))]
    private bool HasDotNetExecutable => !string.IsNullOrEmpty(dotNetExecutable);

    public string Name
    {
        get => name;
        set
        {
            if (!string.Equals(name, value, StringComparison.Ordinal))
            {
                name = value;
                InitializeBuildPath(stopProcess: false);
                _ = RestoreAsync();
            }
        }
    }

    private string BuildPath => parameters.BuildPath;

    private string ExecutableExtension => Platform.IsDotNet ? "dll" : "exe";

    public ImmutableArray<MetadataReference> MetadataReferences { get; private set; } = [];

    public ImmutableArray<AnalyzerFileReference> Analyzers { get; private set; } = [];

    public event Action<IList<CompilationErrorResultObject>>? CompilationErrors;

    public event Action<string>? Disassembled;

    public event Action<ResultObject>? Dumped;

    public event Action<ExceptionResultObject>? Error;

    public event Action? ReadInput;

    public event Action? RestoreStarted;

    public event Action<RestoreResult>? RestoreCompleted;

    public event Action<ProgressResultObject>? ProgressChanged;

    public void Dispose()
    {
        executeCts?.Dispose();
        restoreCts?.Dispose();
    }

    private string GetGlobalUsings() => string.Join(" ", imports.Select(i => $"global using {i};"));

    private void InitializeBuildPath(bool stopProcess)
    {
        if (!HasPlatform)
        {
            return;
        }

        if (stopProcess)
        {
            StopProcess();
        }
        else if (running)
        {
            initializeBuildPathAfterRun = true;
            return;
        }

        CleanupBuildPath();
    }

    private void CleanupBuildPath()
    {
        StopProcess();

        foreach (var file in IOUtilities.EnumerateFilesRecursive(BuildPath))
        {
            IOUtilities.PerformIO(() => File.Delete(file));
        }
    }

    public void ClearRestoreCache() => Directory.Delete(restoreCachePath);

    public async Task ExecuteAsync(string path, bool disassemble, OptimizationLevel? optimizationLevel, CancellationToken cancellationToken)
    {
        if (!HasDotNetExecutable)
        {
            NoDotNetError();
            return;
        }

        LogRun.Info("Start ExecuteAsync");

        await new NoContextYieldAwaitable();

        await RestoreTask.ConfigureAwait(false);

        using var executeCts = CancelAndCreateNew(ref this.executeCts, cancellationToken);
        cancellationToken = executeCts.Token;

        using var _ = await lockSlim.DisposableWaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            running = true;
            var binPath = IsScript ? BuildPath : Path.Combine(BuildPath, "bin");
            assemblyPath = Path.Combine(binPath, $"{Name}.{ExecutableExtension}");

            var success = IsScript
                ? CompileInProcess(path, optimizationLevel, assemblyPath, cancellationToken)
                : await CompileWithMsbuild(path, optimizationLevel, cancellationToken).ConfigureAwait(false);

            if (!success)
            {
                return;
            }

            if (disassemble)
            {
                Disassemble();
            }

            await ExecuteAssemblyAsync(assemblyPath, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.executeCts?.Dispose();
            this.executeCts = null;
            running = false;

            if (initializeBuildPathAfterRun)
            {
                initializeBuildPathAfterRun = false;
                InitializeBuildPath(stopProcess: false);
            }
        }
    }

    private async Task<bool> CompileWithMsbuild(string path, OptimizationLevel? optimizationLevel, CancellationToken cancellationToken)
    {
        if (restorePath is null)
        {
            return false;
        }

        var targetPath = Path.Combine(BuildPath, "Program.cs");
        var code = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        var syntaxTree = ParseAndTransformCode(code, path, (CSharpParseOptions)roslynHost.ParseOptions, cancellationToken: cancellationToken);
        var finalCode = syntaxTree.ToString();
        if (!File.Exists(targetPath) || !string.Equals(await File.ReadAllTextAsync(targetPath, cancellationToken).ConfigureAwait(false), finalCode, StringComparison.Ordinal))
        {
            await File.WriteAllTextAsync(targetPath, finalCode, cancellationToken).ConfigureAwait(false);
        }

        var csprojPath = Path.Combine(BuildPath, UseCache ? "program.csproj" : $"{Name}.csproj");
        if (Platform.IsDotNetFramework || Platform.FrameworkVersion?.Major < 5)
        {
            var moduleInitAttributeFile = Path.Combine(BuildPath, BuildCode.ModuleInitAttributeFileName);
            if (!File.Exists(moduleInitAttributeFile))
            {
                await File.WriteAllTextAsync(moduleInitAttributeFile, BuildCode.ModuleInitAttribute, cancellationToken).ConfigureAwait(false);
            }
        }

        var moduleInitFile = Path.Combine(BuildPath, BuildCode.ModuleInitFileName);
        if (!File.Exists(moduleInitFile))
        {
            await File.WriteAllTextAsync(moduleInitFile, BuildCode.ModuleInit, cancellationToken).ConfigureAwait(false);
        }

        var buildWarningsPath = Path.Combine(BuildPath, "build-warnings.log");
        var buildErrorsPath = Path.Combine(BuildPath, "build-errors.log");

        var buildArgs =
            $"-nologo -v:q -p:Configuration={optimizationLevel} \"-p:AssemblyName={Name}\" " +
            $"\"-flp1:logfile={buildWarningsPath};warningsonly;Encoding=UTF-8\" \"-flp2:logfile={buildErrorsPath};errorsonly;Encoding=UTF-8\" \"{csprojPath}\" ";
        using var buildResult = await ProcessUtil.RunProcessAsync(DotNetExecutable, BuildPath,
            $"build {buildArgs}", cancellationToken).ConfigureAwait(false);
        await buildResult.WaitForExitAsync().ConfigureAwait(false);

        var compilationErrors = await ReadBuildLogAsync(buildWarningsPath, "Warning")
            .Concat(ReadBuildLogAsync(buildErrorsPath, "Error"))
            .ToArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var success = buildResult.ExitCode == 0;
        if (!success && compilationErrors.Length == 0)
        {
            compilationErrors = [new CompilationErrorResultObject { Severity = "Error", Message = "Build failed: " + buildResult.StandardError }];
        }

        CompilationErrors?.Invoke(compilationErrors);

        return success;
    }

    private async IAsyncEnumerable<CompilationErrorResultObject> ReadBuildLogAsync(string path, string severity)
    {
        if (!File.Exists(path))
        {
            yield break;
        }

        await foreach (var line in File.ReadLinesAsync(path).ConfigureAwait(false))
        {
            var match = MsbuildLogRegex().Match(line);
            if (!match.Success)
            {
                continue;
            }

            var code = match.Groups["code"].Value;
            if (parameters.DisabledDiagnostics.Contains(code))
            {
                continue;
            }

            var error = new CompilationErrorResultObject
            {
                Severity = severity,
                ErrorCode = code,
                Message = match.Groups["message"].Value,
            };

            if (match.Groups["file"].Value.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                error.LineNumber = int.Parse(match.Groups["line"].ValueSpan, CultureInfo.InvariantCulture);
                error.Column = int.Parse(match.Groups["column"].ValueSpan, CultureInfo.InvariantCulture);
            }

            yield return error;
        }
    }

    private bool CompileInProcess(string path, OptimizationLevel? optimizationLevel, string assemblyPath, CancellationToken cancellationToken)
    {
        var code = File.ReadAllText(path);
        var script = CreateCompiler(code, optimizationLevel, cancellationToken);

        var diagnostics = script.CompileAndSaveAssembly(assemblyPath, cancellationToken);
        var hasErrors = diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        LogRun.Info("Assembly saved at {AssemblyPath}, has errors = {HasErrors}", this.assemblyPath, hasErrors);

        SendDiagnostics(diagnostics);
        return !hasErrors;
    }

    private void NoDotNetError()
    {
        CompilationErrors?.Invoke(
        [
            CompilationErrorResultObject.Create("Error", errorCode: "",
                message: "The .NET SDK is required to use RoslynPad. https://aka.ms/dotnet/download", line: 0, column: 0)
        ]);
    }

    private void Disassemble()
    {
        using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
        var output = new PlainTextOutput();
        var disassembler = new ReflectionDisassembler(output, false, CancellationToken.None);
        disassembler.WriteModuleContents(assembly.MainModule);
        Disassembled?.Invoke(output.ToString());
    }

    private Compiler CreateCompiler(string code, OptimizationLevel? optimizationLevel, CancellationToken cancellationToken)
    {
        var platform = Platform.Architecture == Architecture.X86
            ? Microsoft.CodeAnalysis.Platform.AnyCpu32BitPreferred
            : Microsoft.CodeAnalysis.Platform.AnyCpu;

        var optimization = optimizationLevel ?? OptimizationLevel.Release;

        LogRun.Info("Creating script runner, platform = {Platform}, " +
            "references = {References}, imports = {Imports}, directory = {Directory}, " +
            "optimization = {Optimization}",
            platform,
            MetadataReferences.Select(t => t.Display),
            imports,
            parameters.WorkingDirectory,
            optimizationLevel);

        var parseOptions = ((CSharpParseOptions)roslynHost.ParseOptions).WithKind(parameters.SourceCodeKind);

        var syntaxTrees = ImmutableList.Create(ParseAndTransformCode(code, path: string.Empty, parseOptions, cancellationToken));
        if (parameters.SourceCodeKind == SourceCodeKind.Script)
        {
            syntaxTrees = syntaxTrees.Insert(0, scriptInitSyntax);
        }
        else
        {
            if (Platform.IsDotNetFramework || Platform.FrameworkVersion?.Major < 5)
            {
                syntaxTrees = syntaxTrees.Add(moduleInitAttributeSyntax);
            }

            syntaxTrees = syntaxTrees.Add(moduleInitSyntax).Add(importsSyntax);
        }

        return new Compiler(syntaxTrees,
            parseOptions,
            OutputKind.ConsoleApplication,
            platform,
            MetadataReferences,
            imports,
            parameters.WorkingDirectory,
            optimizationLevel: optimization,
            checkOverflow: parameters.CheckOverflow,
            allowUnsafe: parameters.AllowUnsafe);
    }

    private async Task ExecuteAssemblyAsync(string assemblyPath, CancellationToken cancellationToken)
    {
        using var process = new Process { StartInfo = GetProcessStartInfo(assemblyPath) };
        using var _ = cancellationToken.Register(() =>
        {
            try
            {
                processInputStream = null;
                process.Kill();
            }
            catch (Exception ex)
            {
                LogRun.Warn(ex, "Error killing process");
            }
        });

        LogRun.Info("Starting process {Executable}, arguments = {Arguments}", process.StartInfo.FileName, process.StartInfo.Arguments);
        if (!process.Start())
        {
            LogRun.Warn("Process.Start returned false");
            return;
        }

        processInputStream = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);

        await Task.WhenAll(
            Task.Run(() => ReadObjectProcessStreamAsync(process.StandardOutput), cancellationToken),
            Task.Run(() => ReadProcessStreamAsync(process.StandardError), cancellationToken)).ConfigureAwait(false);

        ProcessStartInfo GetProcessStartInfo(string assemblyPath) => new()
        {
            FileName = Platform.IsDotNet ? DotNetExecutable : assemblyPath,
            Arguments = $"\"{assemblyPath}\" --pid {Environment.ProcessId}",
            WorkingDirectory = parameters.WorkingDirectory,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
    }

    public async Task SendInputAsync(string message)
    {
        var stream = processInputStream;
        if (stream != null)
        {
            await stream.WriteLineAsync(message).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }
    }

    private async Task ReadProcessStreamAsync(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line != null)
            {
                Dumped?.Invoke(new ResultObject { Value = line });
            }
        }
    }

    private async Task ReadObjectProcessStreamAsync(StreamReader reader)
    {
        const int prefixLength = 2;
        using var sequence = new Sequence<byte>(ArrayPool<byte>.Shared) { AutoIncreaseMinimumSpanLength = false };
        while (true)
        {
            var eolPosition = await ReadLineAsync().ConfigureAwait(false);
            if (eolPosition == null)
            {
                return;
            }

            var readOnlySequence = sequence.AsReadOnlySequence;
            if (readOnlySequence.FirstSpan.Length > 1 && readOnlySequence.FirstSpan[1] == ':')
            {
                switch (readOnlySequence.FirstSpan[0])
                {
                    case (byte)'i':
                        ReadInput?.Invoke();
                        break;
                    case (byte)'o':
                        var objectResult = Deserialize<ResultObject>(readOnlySequence);
                        Dumped?.Invoke(objectResult);
                        break;
                    case (byte)'e':
                        var exceptionResult = Deserialize<ExceptionResultObject>(readOnlySequence);
                        Error?.Invoke(exceptionResult);
                        break;
                    case (byte)'p':
                        var progressResult = Deserialize<ProgressResultObject>(readOnlySequence);
                        ProgressChanged?.Invoke(progressResult);
                        break;

                }
            }

            sequence.AdvanceTo(eolPosition.Value);
        }

        async ValueTask<SequencePosition?> ReadLineAsync()
        {
            var readOnlySequence = sequence.AsReadOnlySequence;
            var position = readOnlySequence.PositionOf(NewLine[^1]);
            if (position != null)
            {
                return readOnlySequence.GetPosition(1, position.Value);
            }

            while (true)
            {
                var memory = sequence.GetMemory(0);
                var read = await reader.BaseStream.ReadAsync(memory).ConfigureAwait(false);
                if (read == 0)
                {
                    return null;
                }

                var eolIndex = memory.Span.Slice(0, read).IndexOf(NewLine[^1]);
                if (eolIndex != -1)
                {
                    var length = sequence.Length;
                    sequence.Advance(read);
                    var index = length + eolIndex + 1;
                    return sequence.AsReadOnlySequence.GetPosition(index);
                }

                sequence.Advance(read);
            }
        }

        static T Deserialize<T>(ReadOnlySequence<byte> sequence)
        {
            var jsonReader = new Utf8JsonReader(sequence.Slice(prefixLength));
            return JsonSerializer.Deserialize<T>(ref jsonReader, SerializerOptions)!;
        }
    }

    private static SyntaxTree ParseAndTransformCode(string code, string path, CSharpParseOptions parseOptions, CancellationToken cancellationToken)
    {
        var tree = SyntaxFactory.ParseSyntaxTree(code, parseOptions, path, cancellationToken: cancellationToken);
        var root = tree.GetRoot(cancellationToken);

        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return tree;
        }

        // references directives are resolved by msbuild, so removing from compilation
        var nodesToRemove = compilationUnit.GetReferenceDirectives().AsEnumerable<SyntaxNode>();
        if (parseOptions.Kind == SourceCodeKind.Regular)
        {
            // load directives' files are added to the compilation separately
            nodesToRemove = nodesToRemove.Concat(compilationUnit.GetLoadDirectives());
        }

        compilationUnit = compilationUnit.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepExteriorTrivia) ?? compilationUnit;
        var members = compilationUnit.Members;

        // add .Dump() to the last bare expression
        var lastMissingSemicolon = compilationUnit.Members.OfType<GlobalStatementSyntax>()
            .LastOrDefault(m => m.Statement is ExpressionStatementSyntax expr && expr.SemicolonToken.IsMissing);
        if (lastMissingSemicolon != null)
        {
            var statement = (ExpressionStatementSyntax)lastMissingSemicolon.Statement;
            members = members.Replace(lastMissingSemicolon, BuildCode.GetDumpCall(statement));
        }

        root = compilationUnit.WithMembers(members);

        return tree.WithRootAndOptions(root, parseOptions);
    }

    private void SendDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        if (diagnostics.Length > 0)
        {
            CompilationErrors?.Invoke([.. diagnostics.Where(d => !parameters.DisabledDiagnostics.Contains(d.Id)).Select(GetCompilationErrorResultObject)]);
        }
    }

    private static CompilationErrorResultObject GetCompilationErrorResultObject(Diagnostic diagnostic)
    {
        var lineSpan = diagnostic.Location.GetLineSpan();

        var result = CompilationErrorResultObject.Create(diagnostic.Severity.ToString(),
                diagnostic.Id, diagnostic.GetMessage(CultureInfo.InvariantCulture),
                lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character);
        return result;
    }

    public Task TerminateAsync()
    {
        StopProcess();
        return Task.CompletedTask;
    }

    private void StopProcess() => executeCts?.Cancel();

    public async Task UpdateReferencesAsync(bool alwaysRestore)
    {
        SyntaxNode? syntaxRoot = await GetSyntaxRootAsync().ConfigureAwait(false);
        if (syntaxRoot == null)
        {
            return;
        }

        IEnumerable<LibraryRef> libraries = ParseReferences(syntaxRoot).
            Append(
            Platform.IsDotNet ?
            runtimeAssemblyLibraryRef :
            runtimeNetFxAssemblyLibraryRef);
        if (UpdateLibraries(libraries))
        {
            await RestoreAsync().ConfigureAwait(false);
        }

        async ValueTask<SyntaxNode?> GetSyntaxRootAsync()
        {
            if (DocumentId == null)
            {
                return null;
            }

            var document = roslynHost.GetDocument(DocumentId);
            return document != null ? await document.GetSyntaxRootAsync().ConfigureAwait(false) : null;
        }

        bool UpdateLibraries(IEnumerable<LibraryRef> libraries)
        {
            lock (this.libraries)
            {
                if (!this.libraries.SetEquals(libraries))
                {
                    this.libraries.Clear();
                    this.libraries.UnionWith(libraries);
                    return true;
                }
                else if (alwaysRestore)
                {
                    return true;
                }
            }

            return false;
        }

        static List<LibraryRef> ParseReferences(SyntaxNode syntaxRoot)
        {
            const string LegacyNuGetPrefix = "$NuGet\\";
            const string FxPrefix = "framework:";

            var libraries = new List<LibraryRef>();

            if (syntaxRoot is not CompilationUnitSyntax compilation)
            {
                return libraries;
            }

            foreach (var directive in compilation.GetReferenceDirectives())
            {
                var value = directive.File.ValueText;
                string? id, version;

                if (HasPrefix(FxPrefix, value))
                {
                    libraries.Add(LibraryRef.FrameworkReference(
                        value.Substring(FxPrefix.Length, value.Length - FxPrefix.Length)));
                    continue;
                }

                if (HasPrefix(ReferenceDirectiveHelper.NuGetPrefix, value))
                {
                    (id, version) = ReferenceDirectiveHelper.ParseNuGetReference(value);
                }
                else if (HasPrefix(LegacyNuGetPrefix, value))
                {
                    (id, version) = ParseLegacyNuGetReference(value);
                    if (id == null)
                    {
                        continue;
                    }
                }
                else
                {
                    libraries.Add(LibraryRef.Reference(value));

                    continue;
                }

                if (!string.IsNullOrEmpty(version) && !VersionRange.TryParse(version, out _))
                {
                    continue;
                }

                libraries.Add(LibraryRef.PackageReference(id, version ?? string.Empty));
            }

            return libraries;

            static bool HasPrefix(string prefix, string value) =>
                value.Length > prefix.Length &&
                value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);

            static (string? id, string? version) ParseLegacyNuGetReference(string value)
            {
                var split = value.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                return split.Length >= 3 ? (split[1], split[2]) : (null, null);
            }
        }
    }

    private Task RestoreTask => restoreTask ?? Task.CompletedTask;

    public DocumentId? DocumentId { get; set; }

    private async Task RestoreAsync(CancellationToken cancellationToken = default)
    {
        if (!HasPlatform || string.IsNullOrEmpty(Name))
        {
            return;
        }

        var restoreCts = CancelAndCreateNew(ref this.restoreCts, cancellationToken);
        cancellationToken = restoreCts.Token;

        RestoreStarted?.Invoke();

        var lockDisposer = await lockSlim.DisposableWaitAsync(cancellationToken).ConfigureAwait(false);
        restoreTask = DoRestoreAsync(RestoreTask, cancellationToken);

        async Task DoRestoreAsync(Task previousTask, CancellationToken cancellationToken)
        {
            try
            {
                if (!HasDotNetExecutable)
                {
                    NoDotNetError();
                    return;
                }

                try
                {
                    await previousTask.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogRun.Warn(ex, "Error in previous restore task");
                }

                var projBuildResult = await BuildCsproj().ConfigureAwait(false);

                var outputPath = Path.Combine(projBuildResult.RestorePath, "output.json");

                if (!projBuildResult.MarkerExists)
                {
                    File.WriteAllText(Path.Combine(projBuildResult.RestorePath, "Program.cs"), "_ = 0;");
                    await BuildGlobalJson(projBuildResult.RestorePath).ConfigureAwait(false);
                    // File.Copy(_parameters.NuGetConfigPath, Path.Combine(projBuildResult.RestorePath, "nuget.config"), overwrite: true);

                    var restoreErrorsPath = Path.Combine(projBuildResult.RestorePath, "restore-errors.log");
                    File.Delete(restoreErrorsPath);

                    cancellationToken.ThrowIfCancellationRequested();

                    var buildArgs =
                        $"--interactive -nologo " +
                        $"-flp:errorsonly;logfile=\"{restoreErrorsPath}\";Encoding=UTF-8 \"{projBuildResult.CsprojPath}\" " +
                        $"-getTargetResult:build -getItem:ReferencePathWithRefAssemblies,Analyzer ";
                    using var restoreResult = await ProcessUtil.RunProcessAsync(DotNetExecutable, BuildPath,
                        $"build {buildArgs}", cancellationToken).ConfigureAwait(false);

                    await restoreResult.GetStandardOutputLinesAsync().LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                    if (restoreResult.ExitCode != 0)
                    {
                        var errors = await GetRestoreErrorsAsync(restoreErrorsPath, restoreResult, cancellationToken).ConfigureAwait(false);
                        RestoreCompleted?.Invoke(RestoreResult.FromErrors(errors));
                        return;
                    }

                    var restoreOutput = JsonSerializer.Deserialize<BuildOutput>(restoreResult.StandardOutput);
                    using var resultOutputStream = File.OpenWrite(outputPath);
                    await JsonSerializer.SerializeAsync(resultOutputStream, restoreOutput, cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (projBuildResult.UsesCache)
                    {
                        await File.WriteAllTextAsync(projBuildResult.MarkerPath, string.Empty, cancellationToken).ConfigureAwait(false);
                    }
                }

                if (projBuildResult.UsesCache)
                {
                    if (IsScript)
                    {
                        IOUtilities.DirectoryCopy(Path.Combine(projBuildResult.RestorePath, "bin"), BuildPath, overwrite: true);
                    }
                    else
                    {
                        IOUtilities.DirectoryCopy(Path.Combine(projBuildResult.RestorePath), BuildPath, overwrite: true, recursive: false);
                        File.Delete(Path.Combine(BuildPath, "Program.cs"));
                    }

                    await File.WriteAllTextAsync(Path.Combine(BuildPath, Path.GetFileName(projBuildResult.RestorePath)), string.Empty, cancellationToken).ConfigureAwait(false);

                    if (IsScript)
                    {
                        foreach (var fileToRename in BinFilesToRename)
                        {
                            var originalFile = Path.Combine(BuildPath, string.Format(CultureInfo.InvariantCulture, fileToRename, "program"));
                            var newFile = Path.Combine(BuildPath, string.Format(CultureInfo.InvariantCulture, fileToRename, Name));
                            if (File.Exists(originalFile))
                            {
                                File.Move(originalFile, newFile, overwrite: true);
                            }
                        }
                    }
                }

                await ReadReferencesAsync(outputPath, cancellationToken).ConfigureAwait(false);
                RestoreCompleted?.Invoke(RestoreResult.SuccessResult);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogRun.Warn(ex, "Restore error");
                RestoreCompleted?.Invoke(RestoreResult.FromErrors([ex.ToString()]));
            }
            finally
            {
                lockDisposer.Dispose();
            }
        }

        async Task ReadReferencesAsync(string path, CancellationToken cancellationToken)
        {
            using var stream = File.OpenRead(path);
            var output = await JsonSerializer.DeserializeAsync<BuildOutput>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (output is null)
            {
                return;
            }

            MetadataReferences = [.. output.Items.ReferencePathWithRefAssemblies
                .Select(r => r.FullPath)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(roslynHost.CreateMetadataReference)];

            Analyzers = [.. output.Items.Analyzer
                .Select(r => r.FullPath)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => new AnalyzerFileReference(r, analyzerAssemblyLoader))];
        }

        async Task BuildGlobalJson(string restorePath)
        {
            if (Platform?.IsDotNet != true)
            {
                return;
            }

            var globalJson = $@"{{ ""sdk"": {{ ""version"": ""{Platform.FrameworkVersion}"" }} }}";
            await File.WriteAllTextAsync(Path.Combine(restorePath, "global.json"), globalJson, cancellationToken).ConfigureAwait(false);
        }

        async Task<CsprojBuildResult> BuildCsproj()
        {
            var platform = this.Platform;

            var csproj = MSBuildHelper.CreateCsproj(
                platform.TargetFrameworkMoniker,
                libraries,
                parameters.Imports);

            string csprojPath;
            string? markerPath;
            bool markerExists;

            if (UseCache)
            {
                string csprojString = csproj.ToString(SaveOptions.DisableFormatting);
                string description = platform.Description;
                string version = this.version;

                var hash = GetHash(csprojString, description, version);
                var hashedRestorePath = Path.Combine(restoreCachePath, hash);
                Directory.CreateDirectory(hashedRestorePath);

                csprojPath = Path.Combine(hashedRestorePath, "program.csproj");
                markerPath = Path.Combine(hashedRestorePath, ".restored");
                restorePath = hashedRestorePath;
                markerExists = File.Exists(markerPath);
            }
            else
            {
                csprojPath = Path.Combine(BuildPath, $"{Name}.csproj");
                markerPath = null;
                restorePath = BuildPath;
                markerExists = false;
            }

            if (!markerExists)
            {
                await Task.Run(() => csproj.Save(csprojPath), cancellationToken).ConfigureAwait(false);
            }

            return new(restorePath, csprojPath, markerPath, markerExists);
        }

        static async Task<string[]> GetRestoreErrorsAsync(string errorsPath, ProcessUtil.ProcessResult result, CancellationToken cancellationToken)
        {
            string[] errors;
            try
            {
                errors = await File.ReadAllLinesAsync(errorsPath, cancellationToken).ConfigureAwait(false);
                if (errors.Length == 0)
                {
                    errors = GetErrorsFromResult(result);
                }
                else
                {
                    for (var i = 0; i < errors.Length; i++)
                    {
                        var match = RestoreErrorRegex().Match(errors[i]);
                        if (match.Success)
                        {
                            errors[i] = match.Value;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                errors = GetErrorsFromResult(result);
            }

            return errors;
        }

        static string[] GetErrorsFromResult(ProcessUtil.ProcessResult result) =>
            [result.StandardError ?? string.Empty];
    }

    private CancellationTokenSource CancelAndCreateNew(ref CancellationTokenSource? cts, CancellationToken cancellationToken)
    {
        lock (ctsLock)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            var newCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts = newCts;
            return newCts;
        }
    }

    private static string GetHash(string a, string b, string c)
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(MemoryMarshal.AsBytes(a.AsSpan()));
        hash.AppendData(MemoryMarshal.AsBytes(b.AsSpan()));
        hash.AppendData(MemoryMarshal.AsBytes(c.AsSpan()));
        hash.TryGetHashAndReset(hashBuffer, out _);
        return Convert.ToHexString(hashBuffer);
    }

    private class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var span = reader.GetSpan();
            return Utf8Parser.TryParse(span.Span, out bool value, out _) ? value : throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => throw new NotSupportedException();
    }

    [GeneratedRegex(@"(?<=\: error )[^\]]+")]
    private static partial Regex RestoreErrorRegex();

    [GeneratedRegex(@"(?<file>[\\/][^\\/(]+)?\((?<line>\d+),(?<column>\d+)\): (warning|error) (?<code>\w+): ((?<message>.+)\s*\[.+\]|(?<message>.+))", RegexOptions.ExplicitCapture)]
    private static partial Regex MsbuildLogRegex();

    private record BuildOutput(BuildOutputItems Items);
    private record BuildOutputItems(BuildOutputReferenceItem[] ReferencePathWithRefAssemblies, BuildOutputReferenceItem[] Analyzer);
    private record BuildOutputReferenceItem(string FullPath);
    private record CsprojBuildResult(string RestorePath, string CsprojPath, string? MarkerPath, bool MarkerExists)
    {
        [MemberNotNullWhen(true, nameof(MarkerPath))]
        public bool UsesCache => MarkerPath is not null;
    }
}
