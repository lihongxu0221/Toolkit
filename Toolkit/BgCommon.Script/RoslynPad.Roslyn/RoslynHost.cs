using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using RoslynPad.Roslyn.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.Reflection;
using AnalyzerReference = Microsoft.CodeAnalysis.Diagnostics.AnalyzerReference;
using AnalyzerFileReference = Microsoft.CodeAnalysis.Diagnostics.AnalyzerFileReference;
using Roslyn.Utilities;
using Microsoft.CodeAnalysis.Text;

namespace RoslynPad.Roslyn;

/// <summary>
/// Roslyn 宿主环境类，管理工作区、文档及编译选项.
/// </summary>
public class RoslynHost : IRoslynHost
{
    /// <summary>
    /// 预处理符号集合.
    /// </summary>
    internal static readonly ImmutableArray<string> PreprocessorSymbols = ["TRACE", "DEBUG"];

    /// <summary>
    /// 默认组合程序集集合.
    /// </summary>
    internal static readonly ImmutableArray<Assembly> DefaultCompositionAssemblies =
    [
        typeof(WorkspacesResources).Assembly,
        typeof(CSharpWorkspaceResources).Assembly,
        typeof(FeaturesResources).Assembly,
        typeof(CSharpFeaturesResources).Assembly,
        typeof(RoslynHost).Assembly,
    ];

    /// <summary>
    /// 默认组合类型集合.
    /// </summary>
    internal static readonly ImmutableArray<Type> DefaultCompositionTypes =
        DefaultCompositionAssemblies.SelectMany(assembly => assembly.DefinedTypes)
        .Select(info => info.AsType())
        .Concat(GetDiagnosticCompositionTypes())
        .ToImmutableArray();

    /// <summary>
    /// 获取诊断相关的组合类型.
    /// </summary>
    /// <returns>返回类型集合.</returns>
    private static IEnumerable<Type> GetDiagnosticCompositionTypes() => MetadataUtil.LoadTypesByNamespaces(
        typeof(Microsoft.CodeAnalysis.CodeFixes.ICodeFixService).Assembly,
        "Microsoft.CodeAnalysis.Diagnostics",
        "Microsoft.CodeAnalysis.CodeFixes");

    /// <summary>
    /// 工作区字典，以文档标识符为键.
    /// </summary>
    private readonly ConcurrentDictionary<DocumentId, RoslynWorkspace> workspaces;

    /// <summary>
    /// 文档提供程序服务字段.
    /// </summary>
    private readonly IDocumentationProviderService documentationProviderService;

    /// <summary>
    /// 组合容器上下文字段.
    /// </summary>
    private readonly CompositionHost compositionContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynHost"/> class.
    /// </summary>
    /// <param name="additionalAssemblies">额外的程序集集合.</param>
    /// <param name="references">宿主引用参数.</param>
    /// <param name="disabledDiagnostics">禁用的诊断 ID 集合.</param>
    /// <param name="analyzerConfigFiles">分析器配置文件集合.</param>
    public RoslynHost(
        IEnumerable<Assembly>? additionalAssemblies = null,
        RoslynHostReferences? references = null,
        ImmutableHashSet<string>? disabledDiagnostics = null,
        ImmutableArray<string>? analyzerConfigFiles = null)
    {
        // 确保引用参数不为空.
        var hostReferences = references ?? RoslynHostReferences.Empty;

        this.workspaces = new ConcurrentDictionary<DocumentId, RoslynWorkspace>();

        // 获取默认组合类型并合并额外程序集中的类型.
        var compositionPartTypes = this.GetDefaultCompositionTypes();

        if (additionalAssemblies != null)
        {
            compositionPartTypes = compositionPartTypes.Concat(additionalAssemblies.SelectMany(a => a.DefinedTypes).Select(t => t.AsType()));
        }

        // 创建组合容器.
        this.compositionContext = new ContainerConfiguration()
            .WithParts(compositionPartTypes)
            .CreateContainer();

        // 初始化主服务和解析选项.
        this.HostServices = MefHostServices.Create(this.compositionContext);
        this.ParseOptions = this.CreateDefaultParseOptions();

        this.documentationProviderService = this.GetService<IDocumentationProviderService>();

        // 初始化引用、导入及配置.
        this.DefaultReferences = hostReferences.GetReferences(this.DocumentationProviderFactory);
        this.DefaultImports = hostReferences.Imports;
        this.DisabledDiagnostics = disabledDiagnostics ?? ImmutableHashSet<string>.Empty;
        this.AnalyzerConfigFiles = analyzerConfigFiles ?? ImmutableArray<string>.Empty;
    }

    /// <summary>
    /// Gets 主机服务实例.
    /// </summary>
    public HostServices HostServices { get; }

    /// <summary>
    /// Gets 解析选项.
    /// </summary>
    public ParseOptions ParseOptions { get; }

    /// <summary>
    /// Gets 默认引用集合.
    /// </summary>
    public ImmutableArray<MetadataReference> DefaultReferences { get; }

    /// <summary>
    /// Gets 默认导入集合.
    /// </summary>
    public ImmutableArray<string> DefaultImports { get; }

    /// <summary>
    /// Gets 已禁用的诊断集合.
    /// </summary>
    public ImmutableHashSet<string> DisabledDiagnostics { get; }

    /// <summary>
    /// Gets 分析器配置文件集合.
    /// </summary>
    public ImmutableArray<string> AnalyzerConfigFiles { get; }

    /// <summary>
    /// Gets 文档提供程序工厂.
    /// </summary>
    public Func<string, DocumentationProvider> DocumentationProviderFactory => this.documentationProviderService.GetDocumentationProvider;

    /// <summary>
    /// 获取默认组合类型.
    /// </summary>
    /// <returns>返回类型集合.</returns>
    protected virtual IEnumerable<Type> GetDefaultCompositionTypes() => DefaultCompositionTypes;

    /// <summary>
    /// 创建默认的解析选项.
    /// </summary>
    /// <returns>返回解析选项实例.</returns>
    protected virtual ParseOptions CreateDefaultParseOptions() => new CSharpParseOptions(
        preprocessorSymbols: PreprocessorSymbols,
        languageVersion: LanguageVersion.Preview);

    /// <summary>
    /// 创建元数据引用.
    /// </summary>
    /// <param name="location">文件位置.</param>
    /// <returns>返回元数据引用.</returns>
    public MetadataReference CreateMetadataReference(string location)
    {
        // 创建文件引用并绑定文档提供程序.
        return MetadataReference.CreateFromFile(
            location,
            documentation: this.documentationProviderService.GetDocumentationProvider(location));
    }

    /// <summary>
    /// 从组合上下文中获取服务.
    /// </summary>
    /// <typeparam name="TService">服务类型.</typeparam>
    /// <returns>返回服务实例.</returns>
    public TService GetService<TService>() => this.compositionContext.GetExport<TService>();

    /// <summary>
    /// 获取特定工作区服务.
    /// </summary>
    /// <typeparam name="TService">服务类型.</typeparam>
    /// <param name="documentId">文档 ID.</param>
    /// <returns>返回工作区服务.</returns>
    public TService GetWorkspaceService<TService>(DocumentId documentId)
        where TService : IWorkspaceService
    {
        // 从对应的工作区中获取服务.
        return this.workspaces[documentId].Services.GetRequiredService<TService>();
    }

    /// <summary>
    /// 添加元数据引用内部方法.
    /// </summary>
    /// <param name="projectId">项目 ID.</param>
    /// <param name="assemblyIdentity">程序集标识.</param>
    protected internal virtual void AddMetadataReference(ProjectId projectId, AssemblyIdentity assemblyIdentity)
    {
    }

    /// <summary>
    /// 关闭并释放工作区.
    /// </summary>
    /// <param name="workspace">要关闭的工作区实例.</param>
    public void CloseWorkspace(RoslynWorkspace workspace)
    {
        // 验证工作区不为空.
        ArgumentNullException.ThrowIfNull(workspace, nameof(workspace));

        // 从管理集合中移除所有相关的文档 ID.
        foreach (var documentId in workspace.CurrentSolution.Projects.SelectMany(project => project.DocumentIds))
        {
            this.workspaces.TryRemove(documentId, out _);
        }

        // 释放工作区资源.
        workspace.Dispose();
    }

    /// <summary>
    /// 创建新的工作区.
    /// </summary>
    /// <returns>返回初始化后的工作区.</returns>
    public virtual RoslynWorkspace CreateWorkspace()
    {
        var newWorkspace = new RoslynWorkspace(this.HostServices, roslynHost: this);
        var diagnosticsUpdater = newWorkspace.Services.GetRequiredService<IDiagnosticsUpdater>();
        diagnosticsUpdater.DisabledDiagnostics = this.DisabledDiagnostics;
        return newWorkspace;
    }

    /// <summary>
    /// 关闭指定标识符的文档.
    /// </summary>
    /// <param name="documentId">文档标识符参数.</param>
    public void CloseDocument(DocumentId documentId)
    {
        // 验证文档 ID 不为空.
        ArgumentNullException.ThrowIfNull(documentId, nameof(documentId));

        if (this.workspaces.TryGetValue(documentId, out var workspace))
        {
            // 关闭工作区中的文档.
            workspace.CloseDocument(documentId);

            var document = workspace.CurrentSolution.GetDocument(documentId);
            if (document != null)
            {
                // 从项目中移除该文档.
                var updatedSolution = document.Project.RemoveDocument(documentId).Solution;

                // 如果项目中已无任何文档，则销毁工作区.
                if (!updatedSolution.Projects.SelectMany(p => p.DocumentIds).Any())
                {
                    if (this.workspaces.TryRemove(documentId, out var removedWorkspace))
                    {
                        removedWorkspace.Dispose();
                    }
                }
                else
                {
                    // 否则更新解决方案状态.
                    workspace.SetCurrentSolution(updatedSolution);
                }
            }
        }
    }

    /// <summary>
    /// 获取文档实例.
    /// </summary>
    /// <param name="documentId">文档标识符.</param>
    /// <returns>返回文档对象.</returns>
    public Document? GetDocument(DocumentId documentId)
    {
        // 验证文档 ID.
        ArgumentNullException.ThrowIfNull(documentId, nameof(documentId));

        return this.workspaces.TryGetValue(documentId, out var targetWorkspace)
            ? targetWorkspace.CurrentSolution.GetDocument(documentId)
            : null;
    }

    /// <summary>
    /// 添加新文档到宿主环境.
    /// </summary>
    /// <param name="args">创建文档的参数接口.</param>
    /// <returns>返回文档 ID.</returns>
    public DocumentId AddDocument(DocumentCreationArgs args)
    {
        // 验证参数.
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        return this.AddDocument(this.CreateWorkspace(), args);
    }

    /// <summary>
    /// 添加关联文档.
    /// </summary>
    /// <param name="relatedDocumentId">关联的文档 ID.</param>
    /// <param name="args">创建参数.</param>
    /// <param name="addProjectReference">是否添加项目引用.</param>
    /// <returns>返回新文档 ID.</returns>
    public DocumentId AddRelatedDocument(DocumentId relatedDocumentId, DocumentCreationArgs args, bool addProjectReference = true)
    {
        // 验证参数.
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        if (!this.workspaces.TryGetValue(relatedDocumentId, out var targetWorkspace))
        {
            throw new ArgumentException("无法找到该文档所在的工作区.", nameof(relatedDocumentId));
        }

        // 创建关联文档，并可选地添加引用.
        var newDocumentId = this.AddDocument(
            targetWorkspace,
            args,
            addProjectReference ? targetWorkspace.CurrentSolution.GetDocument(relatedDocumentId) : null);

        return newDocumentId;
    }

    /// <summary>
    /// 内部执行文档添加逻辑.
    /// </summary>
    /// <param name="workspace">目标工作区.</param>
    /// <param name="args">创建参数.</param>
    /// <param name="previousDocument">前置相关文档.</param>
    /// <returns>返回新文档 ID.</returns>
    private DocumentId AddDocument(RoslynWorkspace workspace, DocumentCreationArgs args, Document? previousDocument = null)
    {
        var solutionSnapshot = workspace.CurrentSolution;

        // 如果是全新项目，添加分析器引用.
        if (previousDocument == null)
        {
            solutionSnapshot = solutionSnapshot.AddAnalyzerReferences(this.GetSolutionAnalyzerReferences());
        }

        // 创建项目和文档.
        var targetProject = this.CreateProject(
            solutionSnapshot,
            args,
            this.CreateCompilationOptions(args, previousDocument == null),
            previousDocument?.Project);

        var targetDocument = this.CreateDocument(targetProject, args);
        var createdId = targetDocument.Id;

        // 更新工作区状态并打开文档.
        workspace.SetCurrentSolution(targetDocument.Project.Solution);
        workspace.OpenDocument(createdId, args.SourceTextContainer);

        this.workspaces.TryAdd(createdId, workspace);

        // 处理文本更新订阅回调.
        var updateAction = args.OnTextUpdated;
        if (updateAction != null)
        {
            workspace.ApplyingTextChange += (id, sourceText) =>
            {
                if (createdId == id)
                {
                    updateAction?.Invoke(sourceText);
                }
            };
        }

        return createdId;
    }

    /// <summary>
    /// 获取解决方案级别的分析器引用.
    /// </summary>
    /// <returns>返回分析器引用集合.</returns>
    protected virtual IEnumerable<AnalyzerReference> GetSolutionAnalyzerReferences()
    {
        var assemblyLoader = this.GetService<IAnalyzerAssemblyLoader>();
        yield return new AnalyzerFileReference(MetadataUtil.GetAssemblyPath(typeof(Compilation).Assembly), assemblyLoader);
        yield return new AnalyzerFileReference(MetadataUtil.GetAssemblyPath(typeof(CSharpResources).Assembly), assemblyLoader);
        yield return new AnalyzerFileReference(MetadataUtil.GetAssemblyPath(typeof(FeaturesResources).Assembly), assemblyLoader);
        yield return new AnalyzerFileReference(MetadataUtil.GetAssemblyPath(typeof(CSharpFeaturesResources).Assembly), assemblyLoader);
    }

    /// <summary>
    /// 更新文档到工作区.
    /// </summary>
    /// <param name="document">最新的文档对象.</param>
    public void UpdateDocument(Document document)
    {
        if (this.workspaces.TryGetValue(document.Id, out var targetWorkspace))
        {
            // 尝试应用更改到物理工作区.
            targetWorkspace.TryApplyChanges(document.Project.Solution);
        }
    }

    /// <summary>
    /// 创建编译选项.
    /// </summary>
    /// <param name="args">文档创建参数.</param>
    /// <param name="addDefaultImports">是否添加默认导入.</param>
    /// <returns>返回编译选项.</returns>
    protected virtual CompilationOptions CreateCompilationOptions(DocumentCreationArgs args, bool addDefaultImports)
    {
        var targetCompilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            usings: addDefaultImports ? this.DefaultImports : [],
            allowUnsafe: true,
            sourceReferenceResolver: new SourceFileResolver([], args.WorkingDirectory),
            metadataReferenceResolver: DummyScriptMetadataResolver.Instance, // 所有 #r 引用由编辑器处理.
            nullableContextOptions: NullableContextOptions.Enable);
        return targetCompilationOptions;
    }

    /// <summary>
    /// 创建文档实例.
    /// </summary>
    /// <param name="project">所属项目.</param>
    /// <param name="args">文档参数接口.</param>
    /// <returns>返回创建成功的文档.</returns>
    protected virtual Document CreateDocument(Project project, DocumentCreationArgs args)
    {
        var newDocumentId = DocumentId.CreateNewId(project.Id);
        var updatedSolution = project.Solution.AddDocument(newDocumentId, args.Name ?? project.Name, args.SourceTextContainer.CurrentText);
        return updatedSolution.GetDocument(newDocumentId)!;
    }

    /// <summary>
    /// 创建项目并配置其选项.
    /// </summary>
    /// <param name="solution">当前的解决方案 snapshot.</param>
    /// <param name="args">文档创建参数接口.</param>
    /// <param name="compilationOptions">编译选项.</param>
    /// <param name="previousProject">关联的前置项目.</param>
    /// <returns>返回配置完成的项目.</returns>
    protected virtual Project CreateProject(Solution solution, DocumentCreationArgs args, CompilationOptions compilationOptions, Project? previousProject = null)
    {
        var projectName = args.Name ?? "New";
        var projectFilePath = Path.Combine(args.WorkingDirectory, projectName);
        var newProjectId = ProjectId.CreateNewId(projectName);

        var parseOptionsSnapshot = this.ParseOptions.WithKind(args.SourceCodeKind);
        var isScriptMode = args.SourceCodeKind == SourceCodeKind.Script;

        if (isScriptMode)
        {
            compilationOptions = compilationOptions.WithScriptClassName(projectName);
        }

        // 加载分析器配置文件.
        var analyzerConfigInfos = this.AnalyzerConfigFiles.Where(File.Exists).Select(configFile => DocumentInfo.Create(
            DocumentId.CreateNewId(newProjectId, debugName: configFile),
            name: configFile,
            loader: new FileTextLoader(configFile, defaultEncoding: null),
            filePath: configFile));

        // 向解决方案添加项目.
        var updatedSolution = solution.AddProject(ProjectInfo.Create(
            newProjectId,
            VersionStamp.Create(),
            projectName,
            projectName,
            LanguageNames.CSharp,
            filePath: projectFilePath,
            isSubmission: isScriptMode,
            parseOptions: parseOptionsSnapshot,
            compilationOptions: compilationOptions,
            metadataReferences: previousProject != null ? [] : this.DefaultReferences,
            projectReferences: previousProject != null ? new[] { new ProjectReference(previousProject.Id) } : null)
            .WithAnalyzerConfigDocuments(analyzerConfigInfos));

        var targetProject = updatedSolution.GetProject(newProjectId)!;

        // 如果非脚本模式且有导入，则生成全局 Using 文档.
        if (!isScriptMode && GetUsings(targetProject) is { Length: > 0 } generatedUsings)
        {
            targetProject = targetProject.AddDocument("RoslynPadGeneratedUsings", generatedUsings).Project;
        }

        return targetProject;

        // 获取全局 Using 字符串.
        static string GetUsings(Project projectInstance)
        {
            if (projectInstance.CompilationOptions is CSharpCompilationOptions csharpOptions)
            {
                return string.Join(" ", csharpOptions.Usings.Select(import => $"global using {import};"));
            }

            return string.Empty;
        }
    }
}