using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.BraceMatching;
using RoslynPad.Roslyn.Diagnostics;
using RoslynPad.Roslyn.Structure;
using RoslynPad.Roslyn.QuickInfo;
using Microsoft.CodeAnalysis.Formatting;
using System.Reactive.Linq;
using System.Windows.Media;
using System.Windows;
using ICSharpCode.AvalonEdit.Folding;
using System.Threading;
using System.Windows.Input;

namespace RoslynPad.Editor;

/// <summary>
/// 集成了 Roslyn 功能的代码编辑器控件.
/// </summary>
public class RoslynCodeEditor : CodeTextEditor
{
    /// <summary>
    /// 标识代码折叠是否启用的依赖属性.
    /// </summary>
    public static readonly StyledProperty IsCodeFoldingEnabledProperty =
        CommonProperty.Register<RoslynCodeEditor, bool>(nameof(RoslynCodeEditor.IsCodeFoldingEnabled), defaultValue: true);

    /// <summary>
    /// 标识括号自动补全是否启用的依赖属性.
    /// </summary>
    public static readonly StyledProperty IsBraceCompletionEnabledProperty =
        CommonProperty.Register<RoslynCodeEditor, bool>(nameof(RoslynCodeEditor.IsBraceCompletionEnabled), defaultValue: true);

    /// <summary>
    /// 标识上下文操作图标的依赖属性.
    /// </summary>
    public static readonly StyledProperty ContextActionsIconProperty =
        CommonProperty.Register<RoslynCodeEditor, ImageSource>(nameof(RoslynCodeEditor.ContextActionsIcon), onChanged: OnContextActionsIconChanged);

    /// <summary>
    /// 文档创建中的路由事件.
    /// </summary>
    public static readonly RoutedEvent CreatingDocumentEvent =
        CommonEvent.Register<RoslynCodeEditor, CreatingDocumentEventArgs>(nameof(RoslynCodeEditor.CreatingDocument), RoutingStrategy.Bubble);

    private readonly TextMarkerService textMarkerService;
    private BraceMatcherHighlightRenderer? braceMatcherHighlighter;
    private ContextActionsRenderer? contextActionsRenderer;
    private IClassificationHighlightColors? classificationHighlightColors;
    private IRoslynHost? roslynHost;
    private DocumentId? documentId;
    private IQuickInfoProvider? quickInfoProvider;
    private IBraceMatchingService? braceMatchingService;
    private CancellationTokenSource? braceMatchingCts;
    private RoslynHighlightingColorizer? colorizer;
    private IBlockStructureService? blockStructureService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynCodeEditor"/> class.
    /// </summary>
    public RoslynCodeEditor()
    {
        // 初始化文本标记服务并添加到视图渲染器.
        this.textMarkerService = new TextMarkerService(this);
        this.TextArea.TextView.BackgroundRenderers.Add(this.textMarkerService);
        this.TextArea.TextView.LineTransformers.Add(this.textMarkerService);
        this.TextArea.Caret.PositionChanged += this.CaretOnPositionChanged;

        // 订阅文档变更事件，使用 2 秒节流以刷新折叠状态.
        Observable.FromEventPattern<EventHandler, EventArgs>(
            h => this.TextArea.TextView.Document.TextChanged += h,
            h => this.TextArea.TextView.Document.TextChanged -= h)
            .Throttle(TimeSpan.FromSeconds(2))
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(_ => this.RefreshFoldingsAsync().ConfigureAwait(true));
    }

    /// <summary>
    /// 处理文本变更事件的异步回调.
    /// </summary>
    /// <param name="sender">事件源参数.</param>
    /// <param name="e">事件参数.</param>
    private async void OnTextChanged(object? sender, EventArgs e)
    {
        // 调用折叠刷新逻辑.
        await this.RefreshFoldingsAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Gets 折叠管理器.
    /// </summary>
    public FoldingManager? FoldingManager { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether 代码折叠是否启用.
    /// </summary>
    public bool IsCodeFoldingEnabled
    {
        get => (bool)this.GetValue(IsCodeFoldingEnabledProperty);
        set => this.SetValue(IsCodeFoldingEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 括号自动补全是否启用.
    /// </summary>
    public bool IsBraceCompletionEnabled
    {
        get => (bool)this.GetValue(IsBraceCompletionEnabledProperty);
        set => this.SetValue(IsBraceCompletionEnabledProperty, value);
    }

    /// <summary>
    /// 上下文操作图标属性变更回调.
    /// </summary>
    /// <param name="editor">编辑器实例参数.</param>
    /// <param name="args">属性变更参数.</param>
    private static void OnContextActionsIconChanged(RoslynCodeEditor editor, CommonPropertyChangedArgs<ImageSource> args)
    {
        // 如果渲染器已初始化，更新其图标镜像.
        if (editor.contextActionsRenderer != null)
        {
            editor.contextActionsRenderer.IconImage = args.NewValue;
        }
    }

    /// <summary>
    /// Gets or sets 上下文操作图标.
    /// </summary>
    public ImageSource ContextActionsIcon
    {
        get => (ImageSource)this.GetValue(ContextActionsIconProperty);
        set => this.SetValue(ContextActionsIconProperty, value);
    }

    /// <summary>
    /// Gets or sets 分类高亮颜色.
    /// </summary>
    public IClassificationHighlightColors? ClassificationHighlightColors
    {
        get => this.classificationHighlightColors;
        set
        {
            this.classificationHighlightColors = value;

            // 如果括号高亮器存在且新值不为空，同步更新颜色.
            if (this.braceMatcherHighlighter is not null && value is not null)
            {
                this.braceMatcherHighlighter.ClassificationHighlightColors = value;
            }

            this.RefreshHighlighting();
        }
    }

    /// <summary>
    /// 正在创建文档时触发的事件.
    /// </summary>
    public event EventHandler<CreatingDocumentEventArgs> CreatingDocument
    {
        add => this.AddHandler(CreatingDocumentEvent, value);
        remove => this.RemoveHandler(CreatingDocumentEvent, value);
    }

    /// <summary>
    /// 触发正在创建文档事件的方法.
    /// </summary>
    /// <param name="e">创建文档参数.</param>
    protected virtual void OnCreatingDocument(CreatingDocumentEventArgs e)
    {
        // 引发路由事件.
        this.RaiseEvent(e);
    }

    /// <summary>
    /// 异步初始化编辑器，绑定 Roslyn 环境.
    /// </summary>
    /// <param name="roslynHost">Roslyn 宿主接口.</param>
    /// <param name="highlightColors">分类高亮颜色.</param>
    /// <param name="workingDirectory">当前工作目录.</param>
    /// <param name="documentText">文档初始文本.</param>
    /// <param name="sourceCodeKind">源代码种类接口.</param>
    /// <returns>返回创建的文档标识符.</returns>
    public async ValueTask<DocumentId> InitializeAsync(
        IRoslynHost roslynHost,
        IClassificationHighlightColors highlightColors,
        string workingDirectory,
        string documentText,
        SourceCodeKind sourceCodeKind)
    {
        // 参数非空校验.
        ArgumentNullException.ThrowIfNull(roslynHost, nameof(roslynHost));
        ArgumentNullException.ThrowIfNull(highlightColors, nameof(highlightColors));

        this.roslynHost = roslynHost;
        this.classificationHighlightColors = highlightColors;

        // 初始化括号匹配高亮器.
        this.braceMatcherHighlighter = new BraceMatcherHighlightRenderer(this.TextArea.TextView, this.classificationHighlightColors);

        // 获取 Roslyn 内部服务.
        this.quickInfoProvider = this.roslynHost.GetService<IQuickInfoProvider>();
        this.braceMatchingService = this.roslynHost.GetService<IBraceMatchingService>();

        // 构造 AvalonEdit 文本容器并触发文档创建事件.
        var textContainer = new AvalonEditTextContainer(this.Document) { Editor = this };
        var creatingArgs = new CreatingDocumentEventArgs(textContainer);
        this.OnCreatingDocument(creatingArgs);

        // 获取或向宿主添加文档.
        this.documentId = creatingArgs.DocumentId ??
            this.roslynHost.AddDocument(new DocumentCreationArgs(
                textContainer,
                workingDirectory,
                sourceCodeKind,
                textContainer.UpdateText));

        // 订阅诊断变更事件.
        this.roslynHost.GetWorkspaceService<IDiagnosticsUpdater>(this.documentId).DiagnosticsChanged += this.ProcessDiagnostics;

        if (this.roslynHost.GetDocument(this.documentId) is { } roslynDocument)
        {
            // 同步编辑器的缩进和制表符设置.
            var documentOptions = await roslynDocument.GetOptionsAsync().ConfigureAwait(true);
            this.Options.IndentationSize = documentOptions.GetOption(FormattingOptions.IndentationSize);
            this.Options.ConvertTabsToSpaces = !documentOptions.GetOption(FormattingOptions.UseTabs);

            this.blockStructureService = roslynDocument.GetLanguageService<IBlockStructureService>();
        }

        // 填充文本并清除撤销栈.
        this.AppendText(documentText);
        this.Document.UndoStack.ClearAll();
        this.AsyncToolTipRequest = this.OnAsyncToolTipRequest;

        // 初始化上下文操作渲染器.
        this.contextActionsRenderer = new ContextActionsRenderer(this, this.textMarkerService) { IconImage = this.ContextActionsIcon };
        this.contextActionsRenderer.Providers.Add(new RoslynContextActionProvider(this.documentId, this.roslynHost));

        // 预热补全提供程序.
        var roslynCompletionProvider = new RoslynCodeEditorCompletionProvider(this.documentId, this.roslynHost);
        roslynCompletionProvider.Warmup();
        this.CompletionProvider = roslynCompletionProvider;

        // 刷新高亮、安装折叠管理器并更新折叠.
        this.RefreshHighlighting();
        this.InstallFoldingManager();
        await this.RefreshFoldingsAsync().ConfigureAwait(true);

        return this.documentId;
    }

    /// <summary>
    /// 刷新语法高亮着色器.
    /// </summary>
    public void RefreshHighlighting()
    {
        // 移除旧的着色器.
        if (this.colorizer != null)
        {
            this.TextArea.TextView.LineTransformers.Remove(this.colorizer);
        }

        // 如果环境就绪，应用新的着色器.
        if (this.documentId != null && this.roslynHost != null && this.classificationHighlightColors != null)
        {
            this.colorizer = new RoslynHighlightingColorizer(this.documentId, this.roslynHost, this.classificationHighlightColors);
            this.TextArea.TextView.LineTransformers.Insert(0, this.colorizer);
        }
    }

    /// <summary>
    /// 当光标位置改变时，执行括号匹配高亮逻辑.
    /// </summary>
    /// <param name="sender">事件源参数.</param>
    /// <param name="eventArgs">事件参数.</param>
    private async void CaretOnPositionChanged(object? sender, EventArgs eventArgs)
    {
        // 状态检查.
        if (this.roslynHost == null || this.documentId == null || this.braceMatcherHighlighter == null)
        {
            return;
        }

        // 取消先前的匹配任务.
        this.braceMatchingCts?.Cancel();

        if (this.braceMatchingService == null)
        {
            return;
        }

        // 构造新的取消令牌.
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        this.braceMatchingCts = cts;

        var roslynDocument = this.roslynHost.GetDocument(this.documentId);
        if (roslynDocument == null)
        {
            return;
        }

        try
        {
            var sourceText = await roslynDocument.GetTextAsync(token).ConfigureAwait(true);
            var currentOffset = this.CaretOffset;

            // 获取并应用匹配结果.
            if (currentOffset <= sourceText.Length)
            {
                var matchingResult = await this.braceMatchingService.GetAllMatchingBracesAsync(roslynDocument, currentOffset, token).ConfigureAwait(true);
                this.braceMatcherHighlighter.SetHighlight(matchingResult.leftOfPosition, matchingResult.rightOfPosition);
            }
        }
        catch (OperationCanceledException)
        {
            // 任务取消，忽略过时数据.
        }
    }

    /// <summary>
    /// 尝试将光标跳转到匹配的括号处.
    /// </summary>
    private void TryJumpToBrace()
    {
        if (this.braceMatcherHighlighter == null)
        {
            return;
        }

        var currentCaretOffset = this.CaretOffset;

        // 依次尝试左右两侧的跨度跳转.
        if (this.TryJumpToPosition(this.braceMatcherHighlighter.LeftOfPosition, currentCaretOffset) ||
            this.TryJumpToPosition(this.braceMatcherHighlighter.RightOfPosition, currentCaretOffset))
        {
            this.ScrollToLine(this.TextArea.Caret.Line);
        }
    }

    /// <summary>
    /// 内部执行位置跳转逻辑.
    /// </summary>
    /// <param name="matchingResult">匹配结果数据.</param>
    /// <param name="caretOffset">当前光标位置.</param>
    /// <returns>如果执行了跳转则返回 true.</returns>
    private bool TryJumpToPosition(BraceMatchingResult? matchingResult, int caretOffset)
    {
        if (matchingResult != null)
        {
            // 如果光标在左跨度，跳到右端.
            if (matchingResult.Value.LeftSpan.Contains(caretOffset))
            {
                this.CaretOffset = matchingResult.Value.RightSpan.End;
                return true;
            }

            // 如果光标在右跨度，跳到左端.
            if (matchingResult.Value.RightSpan.Contains(caretOffset) || matchingResult.Value.RightSpan.End == caretOffset)
            {
                this.CaretOffset = matchingResult.Value.LeftSpan.Start;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 处理异步工具提示请求，展示快速修复建议或信息.
    /// </summary>
    /// <param name="arg">工具提示请求参数.</param>
    /// <returns>异步任务.</returns>
    private async Task OnAsyncToolTipRequest(ToolTipRequestEventArgs arg)
    {
        // 状态验证.
        if (this.roslynHost == null || this.documentId == null || this.quickInfoProvider == null)
        {
            return;
        }

        var roslynDocument = this.roslynHost.GetDocument(this.documentId);
        if (roslynDocument == null)
        {
            return;
        }

        // 获取 QuickInfo 并在参数中设置提示内容.
        var quickInfoItem = await this.quickInfoProvider.GetItemAsync(roslynDocument, arg.Position).ConfigureAwait(true);
        if (quickInfoItem != null)
        {
            arg.SetToolTip(quickInfoItem.Create());
        }
    }

    /// <summary>
    /// 处理诊断变更事件，更新编辑器内的波浪线标记.
    /// </summary>
    /// <param name="args">诊断变更事件参数.</param>
    protected async void ProcessDiagnostics(DiagnosticsChangedArgs args)
    {
        // 过滤非本文件的诊断.
        if (args.DocumentId != this.documentId)
        {
            return;
        }

        // 切换到 UI 调度器执行.
        await this.GetDispatcher();

        // 移除已失效的诊断标记.
        this.textMarkerService.RemoveAll(marker => marker.Tag is DiagnosticData diagnosticData && args.RemovedDiagnostics.Contains(diagnosticData));

        if (this.roslynHost == null || this.documentId == null)
        {
            return;
        }

        var roslynDocument = this.roslynHost.GetDocument(this.documentId);
        if (roslynDocument == null || !roslynDocument.TryGetText(out var sourceText))
        {
            return;
        }

        // 添加新的诊断标记.
        foreach (var newDiagnostic in args.AddedDiagnostics)
        {
            if (newDiagnostic.Severity == DiagnosticSeverity.Hidden || newDiagnostic.IsSuppressed)
            {
                continue;
            }

            var textSpan = newDiagnostic.GetTextSpan(sourceText);
            if (textSpan == null)
            {
                continue;
            }

            var diagnosticMarker = this.textMarkerService.TryCreate(textSpan.Value.Start, textSpan.Value.Length);
            if (diagnosticMarker != null)
            {
                diagnosticMarker.Tag = newDiagnostic;
                diagnosticMarker.MarkerColor = RoslynCodeEditor.GetDiagnosticsColor(newDiagnostic);
                diagnosticMarker.ToolTip = newDiagnostic.Message;
            }
        }
    }

    /// <summary>
    /// 根据诊断严重程度获取对应的显示颜色.
    /// </summary>
    /// <param name="diagnosticData">诊断数据参数.</param>
    /// <returns>返回对应的颜色.</returns>
    private static Color GetDiagnosticsColor(DiagnosticData diagnosticData)
    {
        return diagnosticData.Severity switch
        {
            DiagnosticSeverity.Info => Colors.LimeGreen,
            DiagnosticSeverity.Warning => Colors.DodgerBlue,
            DiagnosticSeverity.Error => Colors.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(diagnosticData)),
        };
    }

    /// <summary>
    /// 处理按键按下事件.
    /// </summary>
    /// <param name="e">按键参数.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // 如果按下 Ctrl 键.
        if (e.HasModifiers(ModifierKeys.Control))
        {
            switch (e.Key)
            {
                case Key.OemCloseBrackets:
                    // Ctrl + ] 执行括号跳转.
                    this.TryJumpToBrace();
                    break;
            }
        }
    }

    /// <summary>
    /// 异步刷新代码折叠块的状态.
    /// </summary>
    /// <returns>异步任务.</returns>
    public async Task RefreshFoldingsAsync()
    {
        // 如果管理器未安装或功能已禁用，直接返回.
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        if (this.documentId == null || this.roslynHost == null || this.blockStructureService == null)
        {
            return;
        }

        var roslynDocument = this.roslynHost.GetDocument(this.documentId);
        if (roslynDocument == null)
        {
            return;
        }

        try
        {
            // 获取块结构信息.
            var blockStructure = await this.blockStructureService.GetBlockStructureAsync(roslynDocument).ConfigureAwait(true);

            // 构造折叠信息序列.
            var foldingElements = blockStructure.Spans
                .Select(span => new NewFolding { Name = span.BannerText, StartOffset = span.TextSpan.Start, EndOffset = span.TextSpan.End })
                .OrderBy(item => item.StartOffset);

            // 更新折叠管理器数据.
            this.FoldingManager?.UpdateFoldings(foldingElements, firstErrorOffset: 0);
        }
        catch
        {
            // 忽略计算折叠时的异常.
        }
    }

    /// <summary>
    /// 为当前编辑器安装折叠管理器.
    /// </summary>
    private void InstallFoldingManager()
    {
        if (!this.IsCodeFoldingEnabled)
        {
            return;
        }

        // 在文本区域安装折叠功能.
        this.FoldingManager = FoldingManager.Install(this.TextArea);
    }

    /// <summary>
    /// 全部折叠当前文档中的所有块.
    /// </summary>
    public void FoldAllFoldings()
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        // 遍历设置所有折叠段的状态.
        foreach (var foldingSection in this.FoldingManager.AllFoldings)
        {
            foldingSection.IsFolded = true;
        }
    }

    /// <summary>
    /// 全部展开当前文档中的所有块.
    /// </summary>
    public void UnfoldAllFoldings()
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        foreach (var foldingSection in this.FoldingManager.AllFoldings)
        {
            foldingSection.IsFolded = false;
        }
    }

    /// <summary>
    /// 切换所有块的折叠状态（若有未折叠的则全部折叠）.
    /// </summary>
    public void ToggleAllFoldings()
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        // 判断是否需要全部折叠.
        var shouldFoldAll = this.FoldingManager.AllFoldings.All(folding => !folding.IsFolded);

        foreach (var foldingSection in this.FoldingManager.AllFoldings)
        {
            foldingSection.IsFolded = shouldFoldAll;
        }
    }

    /// <summary>
    /// 切换当前光标所在位置块的折叠状态.
    /// </summary>
    public void ToggleCurrentFolding()
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        // 查找最适合切换的折叠段.
        var targetFolding = this.FoldingManager.GetNextFolding(this.TextArea.Caret.Offset);
        if (targetFolding == null || this.TextArea.Document.GetLocation(targetFolding.StartOffset).Line != this.TextArea.Document.GetLocation(this.TextArea.Caret.Offset).Line)
        {
            targetFolding = this.FoldingManager.GetFoldingsContaining(this.TextArea.Caret.Offset).LastOrDefault();
        }

        if (targetFolding != null)
        {
            targetFolding.IsFolded = !targetFolding.IsFolded;
        }
    }

    /// <summary>
    /// 获取当前编辑器所有的折叠快照.
    /// </summary>
    /// <returns>返回折叠数据集合.</returns>
    public IEnumerable<NewFolding> SaveFoldings()
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return Enumerable.Empty<NewFolding>();
        }

        return this.FoldingManager?.AllFoldings
            .Select(folding => new NewFolding
            {
                StartOffset = folding.StartOffset,
                EndOffset = folding.EndOffset,
                Name = folding.Title,
                DefaultClosed = folding.IsFolded,
            })
            .ToList() ?? Enumerable.Empty<NewFolding>();
    }

    /// <summary>
    /// 还原给定的折叠状态到当前文档.
    /// </summary>
    /// <param name="foldings">要恢复的折叠集合接口.</param>
    public void RestoreFoldings(IEnumerable<NewFolding> foldings)
    {
        if (this.FoldingManager == null || !this.IsCodeFoldingEnabled)
        {
            return;
        }

        // 重新同步折叠数据.
        this.FoldingManager.Clear();
        this.FoldingManager.UpdateFoldings(foldings, -1);
    }
}