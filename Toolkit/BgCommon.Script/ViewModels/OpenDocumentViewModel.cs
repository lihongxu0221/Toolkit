using BgCommon.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Build;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.Rename;
using RoslynPad.UI;
using TextChange = Microsoft.CodeAnalysis.Text.TextChange;

namespace BgCommon.Script.ViewModels;

public partial class OpenDocumentViewModel : NavigableDialogViewModel
{
    private const string DefaultDocumentName = "New";
    private const string RegularFileExtension = ".cs";
    private const string ScriptFileExtension = ".csx";

    private readonly ITelemetryProvider telemetryProvider;
    private readonly IPlatformsFactory platformsFactory;
    private readonly ObservableCollection<IResultObject> results = new ObservableCollection<IResultObject>();
    private readonly List<RestoreResultObject> restoreResults = new List<RestoreResultObject>();

    private volatile bool isSaving;
    private volatile bool isInitialized;

    private ExecutionHost? executionHost;
    private ExecutionHostParameters? executionHostParameters;
    private CancellationTokenSource? runCts;
    private ExecutionPlatform? platform;
    private IDisposable? _viewDisposable;
    private Func<TextSpan>? _getSelection;
    private Timer? _liveModeTimer;
    private IReadOnlyList<ExecutionPlatform>? _availablePlatforms;
    private SourceCodeKind? _sourceCodeKind;

    public string Id { get; }

    public bool HasDocumentId => this.DocumentId is not null;

    [ObservableProperty]
    private DocumentViewModel? document = null;

    public new string Title => Document != null && !Document.IsAutoSaveOnly ? Document.Name : DefaultDocumentName + GetFileExtension();

    public string BuildPath { get; }

    public string WorkingDirectory => Document != null ? Path.GetDirectoryName(Document.Path)! : MainViewModel.DocumentRoot.Path;

    [ObservableProperty]
    private DocumentId? documentId = null;

    [ObservableProperty]
    private string? selectedText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private bool isRunning = false;

    [ObservableProperty]
    private bool isRestoring = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private bool restoreSuccessful = false;

    [ObservableProperty]
    private bool isDirty = false;

    [ObservableProperty]
    private double? reportedProgress;

    [ObservableProperty]
    private bool isLiveMode = false;

    public ScriptMainViewModel MainViewModel { get; internal set; }

    public bool HasReportedProgress => ReportedProgress.HasValue;

    public IEnumerable<IResultObject> Results => results;

    public SourceCodeKind SourceCodeKind
    {
        get
        {
            if (_sourceCodeKind is not null)
            {
                return _sourceCodeKind.Value;
            }

            var isScript = Path.GetExtension(Document?.Name)?.Equals(ScriptFileExtension, StringComparison.OrdinalIgnoreCase);
            return isScript is null
                ? throw new InvalidOperationException("Document not initialized")
                : (_sourceCodeKind ??= isScript == true ? SourceCodeKind.Script : SourceCodeKind.Regular);
        }
        set => _sourceCodeKind = value;
    }

    public ExecutionPlatform? Platform
    {
        get => platform;
        set
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            if (SetProperty(ref platform, value))
            {
                if (executionHost is not null)
                {
                    executionHost.Platform = value;
                }

                UpdatePackages();

                RunCommand.NotifyCanExecuteChanged();
                TerminateCommand.NotifyCanExecuteChanged();

                if (isInitialized)
                {
                    TerminateCommand.Execute(null);
                }
            }
        }
    }

    private string GetFileExtension() => SourceCodeKind == SourceCodeKind.Script ? ScriptFileExtension : RegularFileExtension;

    private Action<ExceptionResultObject?>? _onError;

    public event EventHandler? DocumentUpdated;

    public event Action? ReadInput;

    public event Action? ResultsAvailable;

    public OpenDocumentViewModel(
        IContainerExtension container,
        ITelemetryProvider telemetryProvider,
        bool restoreSuccessful = false)
        : base(container)
    {
        Id = Guid.NewGuid().ToString("n");
        BuildPath = Path.Combine(FileNames.ScriptsBuildPath, Id);
        _ = Directory.CreateDirectory(BuildPath);

        results = new ObservableCollection<IResultObject>();
        restoreResults = new List<RestoreResultObject>();
        this.telemetryProvider = telemetryProvider;
        this.platformsFactory = this.Container.Resolve<IPlatformsFactory>();

        RestoreSuccessful = true;

        InitializePlatforms();
        this.restoreSuccessful = restoreSuccessful;
    }

    [MemberNotNull(nameof(executionHost))]
    private void InitializeExecutionHost()
    {
        var roslynHost = MainViewModel.RoslynHost;

        executionHostParameters = new ExecutionHostParameters(
            BuildPath,
            string.Empty, // _serviceProvider.GetRequiredService<NuGetViewModel>().ConfigPath,
            roslynHost.DefaultImports,
            roslynHost.DisabledDiagnostics,
            WorkingDirectory,
            SourceCodeKind);

        executionHost = new ExecutionHost(executionHostParameters, roslynHost)
        {
            Name = Document?.Name ?? "Untitled",
            DocumentId = DocumentId,
            Platform = Platform.NotNull(),
            DotNetExecutable = platformsFactory.DotNetExecutable
        };

        executionHost.Dumped += Host_ExecutionHostOnDump;
        executionHost.Error += Host_ExecutionHostOnError;
        executionHost.ReadInput += Host_ExecutionHostOnInputRequest;
        executionHost.CompilationErrors += Host_ExecutionHostOnCompilationErrors;
        executionHost.Disassembled += Host_ExecutionHostOnDisassembled;
        executionHost.RestoreStarted += Host_OnRestoreStarted;
        executionHost.RestoreCompleted += Host_OnRestoreCompleted;
        executionHost.ProgressChanged += p => ReportedProgress = p.Progress;
    }

    private void InitializePlatforms()
    {
        AvailablePlatforms = platformsFactory.GetExecutionPlatforms();
    }

    private void OnDocumentUpdated() => DocumentUpdated?.Invoke(this, EventArgs.Empty);

    private void AddResult(IResultObject o)
    {
        lock (results)
        {
            results.Add(o);
        }

        ResultsAvailable?.Invoke();
    }

    private void AddRestoreResult(RestoreResultObject o)
    {
        lock (results)
        {
            restoreResults.Add(o);
            AddResult(o);
        }
    }

    private void Host_ExecutionHostOnDump(ResultObject result)
    {
        AddResult(result);
    }

    private void Host_ExecutionHostOnError(ExceptionResultObject errorResult)
    {
        _ = UIService.RunOnUIThreadAsync(
        () =>
        {
            _onError?.Invoke(errorResult);
            if (errorResult != null)
            {
                AddResult(errorResult);
            }
        }, DispatcherPriority.Background);
    }

    private void Host_ExecutionHostOnInputRequest()
    {
        if (ReadInput != null)
        {
            _ = UIService.RunOnUIThreadAsync(ReadInput.Invoke, DispatcherPriority.Background);
        }
    }

    private void Host_ExecutionHostOnCompilationErrors(IList<CompilationErrorResultObject> errors)
    {
        foreach (CompilationErrorResultObject error in errors)
        {
            AddResult(error);
        }
    }

    private void Host_ExecutionHostOnDisassembled(string il)
    {

    }

    private void Host_OnRestoreStarted()
    {
        IsRestoring = true;
    }

    private void Host_OnRestoreCompleted(RestoreResult restoreResult)
    {
        if (executionHost is null)
        {
            return;
        }

        IsRestoring = false;

        lock (results)
        {
            restoreResults.Clear();
            ClearResults();
        }

        if (restoreResult.Success)
        {
            var host = MainViewModel.RoslynHost;
            var document = host.GetDocument(DocumentId);
            if (document == null)
            {
                return;
            }

            var project = document.Project;

            project = project
                .WithMetadataReferences(executionHost.MetadataReferences)
                .WithAnalyzerReferences(executionHost.Analyzers);

            document = project.GetDocument(DocumentId);

            host.UpdateDocument(document!);
            OnDocumentUpdated();
        }
        else
        {
            foreach (var error in restoreResult.Errors)
            {
                AddRestoreResult(new RestoreResultObject(error, "Error"));
            }
        }

        _ = UIService.RunOnUIThreadAsync(() =>
        {
            RestoreSuccessful = restoreResult.Success;
        });
    }

    public void SetDocument(DocumentViewModel? document)
    {
        Document = document == null ? null : DocumentViewModel.FromPath(document.Path);

        IsDirty = document?.IsAutoSave == true;
    }

    public void SendInput(string input)
    {
        _ = executionHost?.SendInputAsync(input);
    }

    private enum CommentAction
    {
        Comment,
        Uncomment
    }

    private async Task CommentUncommentSelectionAsync(CommentAction action)
    {
        const string singleLineCommentString = "//";

        var document = MainViewModel.RoslynHost.GetDocument(DocumentId);
        if (document == null)
        {
            return;
        }

        if (_getSelection == null)
        {
            return;
        }

        var selection = _getSelection();

        var documentText = await document.GetTextAsync().ConfigureAwait(false);
        var changes = new List<TextChange>();
        var lines = documentText.Lines.SkipWhile(x => !x.Span.IntersectsWith(selection))
            .TakeWhile(x => x.Span.IntersectsWith(selection)).ToArray();

        if (action == CommentAction.Comment)
        {
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(documentText.GetSubText(line.Span).ToString()))
                {
                    changes.Add(new TextChange(new TextSpan(line.Start, 0), singleLineCommentString));
                }
            }
        }
        else if (action == CommentAction.Uncomment)
        {
            foreach (var line in lines)
            {
                var text = documentText.GetSubText(line.Span).ToString();
                if (text.TrimStart().StartsWith(singleLineCommentString, StringComparison.Ordinal))
                {
                    changes.Add(new TextChange(
                        new TextSpan(
                        line.Start + text.IndexOf(singleLineCommentString, StringComparison.Ordinal),
                        singleLineCommentString.Length), string.Empty));
                }
            }
        }

        if (changes.Count == 0)
        {
            return;
        }

        MainViewModel.RoslynHost.UpdateDocument(document.WithText(documentText.WithChanges(changes)));
        if (action == CommentAction.Uncomment && MainViewModel.Settings.FormatDocumentOnComment)
        {
            await OnFormatDocumentAsync().ConfigureAwait(false);
        }
    }

    public IReadOnlyList<ExecutionPlatform> AvailablePlatforms
    {
        get => _availablePlatforms ?? throw new ArgumentNullException(nameof(_availablePlatforms));
        private set => SetProperty(ref _availablePlatforms, value);
    }

    private void SetIsRunning(bool value)
    {
        _ = UIService.RunOnUIThreadAsync(() => IsRunning = value, DispatcherPriority.Normal);
    }

    public async Task AutoSaveAsync()
    {
        if (!IsDirty) return;

        var document = Document;

        if (document == null)
        {
            var index = 1;
            string path;

            do
            {
                path = Path.Combine(WorkingDirectory, DocumentViewModel.GetAutoSaveName("Program" + index++ + GetFileExtension()));
            }
            while (File.Exists(path));

            document = DocumentViewModel.FromPath(path);
        }

        Document = document;

        await SaveDocumentAsync(Document.GetAutoSavePath()).ConfigureAwait(false);
    }

    public async Task<SaveResult> SaveAsync(bool promptSave)
    {
        if (isSaving)
        {
            return SaveResult.Cancel;
        }

        if (!IsDirty && promptSave)
        {
            return SaveResult.Save;
        }

        isSaving = true;
        try
        {
            SaveResult result = SaveResult.Save;
            if (Document == null || Document.IsAutoSaveOnly)
            {
                ISaveDocumentDialog dialog = this.Container.Resolve<ISaveDocumentDialog>();
                dialog.ShowDoNotSave = promptSave;
                dialog.AllowNameEdit = true;
                dialog.FilePathFactory = name => DocumentViewModel.GetDocumentPathFromName(WorkingDirectory, name);
                await dialog.ShowAsync().ConfigureAwait(true);
                result = dialog.Result;
                if (result == SaveResult.Save && dialog.DocumentName != null)
                {
                    Document?.DeleteAutoSave();
                    Document = MainViewModel.AddDocument(dialog.DocumentName + GetFileExtension());
                    OnPropertyChanged(nameof(Title));
                }
            }
            else if (promptSave)
            {
                ISaveDocumentDialog dialog = this.Container.Resolve<ISaveDocumentDialog>();
                dialog.ShowDoNotSave = true;
                dialog.DocumentName = Document.Name;
                await dialog.ShowAsync().ConfigureAwait(true);
                result = dialog.Result;
            }

            if (result == SaveResult.Save && Document != null)
            {
                await SaveDocumentAsync(Document.GetSavePath()).ConfigureAwait(true);
                IsDirty = false;
            }

            if (result != SaveResult.Cancel)
            {
                Document?.DeleteAutoSave();
            }

            return result;
        }
        finally
        {
            isSaving = false;
        }
    }

    private async Task SaveDocumentAsync(string path)
    {
        if (!isInitialized)
        {
            return;
        }

        var document = MainViewModel.RoslynHost.GetDocument(DocumentId);
        if (document == null)
        {
            return;
        }

        var text = await document.GetTextAsync().ConfigureAwait(false);

        using var writer = File.CreateText(path);
        for (int lineIndex = 0; lineIndex < text.Lines.Count - 1; ++lineIndex)
        {
            var lineText = text.Lines[lineIndex].ToString();
            await writer.WriteLineAsync(lineText).ConfigureAwait(false);
        }

        await writer.WriteAsync(text.Lines[text.Lines.Count - 1].ToString()).ConfigureAwait(false);
    }

    internal void Initialize(
        DocumentId documentId,
        Action<ExceptionResultObject?> onError,
        Func<TextSpan> getSelection,
        IDisposable viewDisposable)
    {
        ArgumentNullException.ThrowIfNull(documentId, nameof(documentId));

        _viewDisposable = viewDisposable;
        _onError = onError;
        _getSelection = getSelection;
        DocumentId = documentId;

        Platform = AvailablePlatforms.FirstOrDefault(p => p.ToString() == MainViewModel.Settings.DefaultPlatformName) ?? AvailablePlatforms.FirstOrDefault();

        InitializeExecutionHost();

        isInitialized = true;

        UpdatePackages();

        TerminateCommand?.Execute(null);
    }

    private void StartExec()
    {
        ClearResults();

        _onError?.Invoke(null);
    }

    private void ClearResults()
    {
        lock (results)
        {
            results.Clear();
            results.AddRange(restoreResults);
        }
    }

    private OptimizationLevel OptimizationLevel => MainViewModel.Settings.OptimizeCompilation ? OptimizationLevel.Release : OptimizationLevel.Debug;

    private void UpdatePackages(bool alwaysRestore = true) =>
        _ = executionHost?.UpdateReferencesAsync(alwaysRestore);

    private async Task<string> GetCodeAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(SelectedText))
        {
            return SelectedText;
        }

        var document = MainViewModel.RoslynHost.GetDocument(DocumentId);
        if (document == null)
        {
            return string.Empty;
        }

        return (await document.GetTextAsync(cancellationToken)
            .ConfigureAwait(false)).ToString();
    }

    private CancellationToken ResetCancellation()
    {
        if (this.runCts != null)
        {
            this.runCts.Cancel();
            this.runCts.Dispose();
        }

        var runCts = new CancellationTokenSource();
        this.runCts = runCts;
        return runCts.Token;
    }

    public async Task<string> LoadTextAsync()
    {
        if (Document == null)
        {
            return string.Empty;
        }

        using StreamReader fileStream = File.OpenText(Document.Path);
        return await fileStream.ReadToEndAsync().ConfigureAwait(false);
    }

    public void Close()
    {
        _viewDisposable?.Dispose();
    }

    public event EventHandler? EditorFocus;

    private void OnEditorFocus()
    {
        EditorFocus?.Invoke(this, EventArgs.Empty);
    }

    public void OnTextChanged()
    {
        IsDirty = true;

        if (IsLiveMode)
        {
            _liveModeTimer?.Change(MainViewModel.Settings.LiveModeDelayMs, Timeout.Infinite);
        }

        UpdatePackages(alwaysRestore: false);
    }

    protected override void OnDispose()
    {
        runCts?.Dispose();
    }

    public event Action<(int line, int column)>? EditorChangeLocation;

    public void TryJumpToLine(IResultWithLineNumber result)
    {
        if (result.LineNumber is { } lineNumber)
        {
            EditorChangeLocation?.Invoke((lineNumber, result.Column));
        }
    }

    [RelayCommand]
    private Task OnOpenBuildPathAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                ProcessStartInfo startInfo = new(new Uri("file://" + BuildPath).ToString())
                {
                    UseShellExecute = true
                };

                _ = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                telemetryProvider.ReportError(ex);
            }
        });
    }

    [RelayCommand]
    private Task<SaveResult> OnSaveAsync()
    {
        return this.SaveAsync(promptSave: false);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteRun))]
    private async Task OnRunAsync()
    {
        if (IsRunning || executionHost is null || executionHostParameters is null)
        {
            return;
        }

        ReportedProgress = null;

        CancellationToken cancellationToken = ResetCancellation();

        await MainViewModel.AutoSaveOpenDocumentsAsync().ConfigureAwait(true);

        var documentPath = IsDirty ? Document?.GetAutoSavePath() : Document?.Path;
        if (documentPath is null)
        {
            return;
        }

        SetIsRunning(true);

        StartExec();

        try
        {
            if (executionHost is not null && executionHostParameters is not null)
            {
                if (executionHostParameters.WorkingDirectory != WorkingDirectory)
                {
                    executionHostParameters.WorkingDirectory = WorkingDirectory;
                }

                await executionHost.ExecuteAsync(documentPath, false, OptimizationLevel, cancellationToken).ConfigureAwait(true);
            }
        }
        catch (CompilationErrorException ex)
        {
            foreach (Diagnostic? diagnostic in ex.Diagnostics)
            {
                LinePosition startLinePosition = diagnostic.Location.GetLineSpan().StartLinePosition;
                AddResult(CompilationErrorResultObject.Create(diagnostic.Severity.ToString(), diagnostic.Id, diagnostic.GetMessage(CultureInfo.InvariantCulture), startLinePosition.Line, startLinePosition.Character));
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            AddResult(new ExceptionResultObject { Value = ex.ToString() });
        }
        finally
        {
            SetIsRunning(false);
            ReportedProgress = null;
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteTerminate))]
    private async Task OnTerminateAsync()
    {
        _ = ResetCancellation();
        try
        {
            await Task.Run(() => executionHost?.TerminateAsync()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            telemetryProvider.ReportError(e);
            throw;
        }
        finally
        {
            SetIsRunning(false);
        }
    }

    [RelayCommand]
    private async Task OnFormatDocumentAsync()
    {
        var document = MainViewModel.RoslynHost.GetDocument(DocumentId);
        var formattedDocument = await Formatter.FormatAsync(document!).ConfigureAwait(false);
        MainViewModel.RoslynHost.UpdateDocument(formattedDocument);
    }

    [RelayCommand]
    private async Task OnCommentSelectionAsync()
    {
        await CommentUncommentSelectionAsync(CommentAction.Comment);
    }

    [RelayCommand]
    private async Task OnUncommentSelectionAsync()
    {
        await CommentUncommentSelectionAsync(CommentAction.Uncomment);
    }

    [RelayCommand]
    private async Task OnRenameSymbolAsync()
    {
        RoslynHost? host = MainViewModel.RoslynHost;
        Document? document = host?.GetDocument(DocumentId);
        if (document == null || _getSelection == null)
        {
            return;
        }

        ISymbol? symbol = await RenameHelper.GetRenameSymbol(document, _getSelection().Start).ConfigureAwait(true);
        if (symbol == null)
        {
            return;
        }

        IRenameSymbolDialog dialog = this.Container.Resolve<IRenameSymbolDialog>();
        dialog.Initialize(symbol.Name);
        await dialog.ShowAsync().ConfigureAwait(true);
        if (dialog.ShouldRename)
        {
            Solution newSolution = await Renamer.RenameSymbolAsync(
                document.Project.Solution,
                symbol,
                default(SymbolRenameOptions),
                dialog.SymbolName ?? string.Empty)
                .ConfigureAwait(true);
            Document? newDocument = newSolution.GetDocument(DocumentId);
            host?.UpdateDocument(newDocument!);
        }

        OnEditorFocus();
    }

    [RelayCommand]
    private void OnSetDefaultPlatform()
    {
        if (Platform is not null)
        {
            MainViewModel.Settings.DefaultPlatformName = Platform.ToString();
        }
    }

    [RelayCommand]
    private void OnToggleLiveMode()
    {
        IsLiveMode = !IsLiveMode;
    }

    private bool CanExecuteRun()
    {
        return !IsRunning && RestoreSuccessful && Platform != null;
    }

    private bool CanExecuteTerminate => Platform != null;

    partial void OnIsRunningChanged(bool value)
    {
        _ = UIService.RunOnUIThreadAsync(RunCommand.NotifyCanExecuteChanged, DispatcherPriority.Normal);
    }

    partial void OnRestoreSuccessfulChanged(bool value)
    {
        _ = UIService.RunOnUIThreadAsync(RunCommand.NotifyCanExecuteChanged, DispatcherPriority.Normal);
    }

    partial void OnReportedProgressChanged(double? value)
    {
        OnPropertyChanged(nameof(HasReportedProgress));
    }

    partial void OnIsLiveModeChanged(bool oldValue, bool newValue)
    {
        RunCommand.NotifyCanExecuteChanged();

        if (newValue)
        {
            _ = OnRunAsync();

            _liveModeTimer ??= new Timer(
                o => _ = UIService.RunOnUIThreadAsync(() => _ = OnRunAsync(), DispatcherPriority.Normal),
                state: null,
                Timeout.Infinite,
                Timeout.Infinite);
        }
    }
}
