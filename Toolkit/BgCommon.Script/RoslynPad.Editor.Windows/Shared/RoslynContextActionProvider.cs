using System.Collections.Immutable;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.CodeActions;
using RoslynPad.Roslyn.CodeFixes;
using RoslynPad.Roslyn.CodeRefactorings;

namespace RoslynPad.Editor;

/// <summary>
/// Roslyn 上下文操作提供程序，用于获取代码修复和代码重构操作.
/// </summary>
public sealed class RoslynContextActionProvider : IContextActionProvider
{
    /// <summary>
    /// 排除掉的重构提供程序名称集合.
    /// </summary>
    private static readonly ImmutableArray<string> excludedRefactoringProviders = ["ExtractInterface"];

    /// <summary>
    /// 文档标识符字段.
    /// </summary>
    private readonly DocumentId documentId;

    /// <summary>
    /// Roslyn 宿主环境字段.
    /// </summary>
    private readonly IRoslynHost roslynHost;

    /// <summary>
    /// 代码修复服务字段.
    /// </summary>
    private readonly ICodeFixService codeFixService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynContextActionProvider"/> class.
    /// </summary>
    /// <param name="documentId">关联的文档标识符.</param>
    /// <param name="roslynHost">Roslyn 宿主环境接口.</param>
    public RoslynContextActionProvider(DocumentId documentId, IRoslynHost roslynHost)
    {
        // 验证参数是否为空.
        ArgumentNullException.ThrowIfNull(documentId, nameof(documentId));
        ArgumentNullException.ThrowIfNull(roslynHost, nameof(roslynHost));

        this.documentId = documentId;
        this.roslynHost = roslynHost;
        this.codeFixService = this.roslynHost.GetService<ICodeFixService>();
    }

    /// <summary>
    /// 异步获取指定文本范围内的所有可用代码操作.
    /// </summary>
    /// <param name="offset">文本偏移量.</param>
    /// <param name="length">文本跨度长度.</param>
    /// <param name="cancellationToken">取消令牌.</param>
    /// <returns>返回包含代码修复和重构操作的集合.</returns>
    public async Task<IEnumerable<object>> GetActionsAsync(int offset, int length, CancellationToken cancellationToken)
    {
        // 创建文本跨度对象.
        var selectionSpan = new TextSpan(offset, length);

        // 从宿主中获取对应的文档实例.
        var roslynDocument = this.roslynHost.GetDocument(this.documentId);
        if (roslynDocument == null)
        {
            return Enumerable.Empty<object>();
        }

        // 异步获取文档源代码文本.
        var sourceText = await roslynDocument.GetTextAsync(cancellationToken).ConfigureAwait(false);

        // 验证选择范围是否在文本长度范围内.
        if (selectionSpan.End >= sourceText.Length)
        {
            return Enumerable.Empty<object>();
        }

        // 流式获取当前位置的所有代码修复项.
        var availableCodeFixes = await this.codeFixService.StreamFixesAsync(roslynDocument, selectionSpan, cancellationToken)
                                                          .ToArrayAsync(cancellationToken: cancellationToken)
                                                          .ConfigureAwait(false);

        // 获取当前位置的所有代码重构项.
        var availableRefactorings = await this.roslynHost.GetService<ICodeRefactoringService>()
                                                         .GetRefactoringsAsync(roslynDocument, selectionSpan, cancellationToken)
                                                         .ConfigureAwait(false);

        // 将代码修复的操作序列与过滤后的重构操作序列合并.
        return ((IEnumerable<object>)availableCodeFixes.SelectMany(fix => fix.Fixes))
            .Concat(availableRefactorings
                .Where(refactoring => excludedRefactoringProviders.All(excludedName => !refactoring.Provider.GetType().Name.Contains(excludedName)))
                .SelectMany(refactoring => refactoring.Actions));
    }

    /// <summary>
    /// 获取操作对象关联的可执行命令.
    /// </summary>
    /// <param name="action">选中的操作对象.</param>
    /// <returns>返回包装后的命令对象，若不支持则返回 null.</returns>
    public ICommand? GetActionCommand(object action)
    {
        // 验证动作对象不为空.
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        // 如果动作是标准的代码动作.
        if (action is CodeAction targetCodeAction)
        {
            return new CodeActionCommand(this, targetCodeAction);
        }

        // 如果动作是代码修复且包含嵌套的子动作.
        if (action is CodeFix targetCodeFix && targetCodeFix.Action.HasCodeActions())
        {
            return new CodeActionCommand(this, targetCodeFix.Action);
        }

        return null;
    }

    /// <summary>
    /// 异步执行具体的代码操作逻辑.
    /// </summary>
    /// <param name="codeAction">要执行的代码动作.</param>
    /// <param name="cancellationToken">执行过程的取消令牌.</param>
    /// <returns>异步任务.</returns>
    private async Task ExecuteCodeActionAsync(CodeAction codeAction, CancellationToken cancellationToken)
    {
        // 在获取操作步骤时传入 CancellationToken.
        var operations = await codeAction.GetOperationsAsync(cancellationToken).ConfigureAwait(true);

        // 遍历并执行每一个操作步骤.
        foreach (var operation in operations)
        {
            // 获取文档的最新快照.
            var document = this.roslynHost.GetDocument(this.documentId);
            if (document != null)
            {
                // 在应用操作时传入 CancellationToken.
                operation.Apply(document.Project.Solution.Workspace, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 内部实现的命令类，用于桥接 ICommand 和异步操作.
    /// </summary>
    private class CodeActionCommand : ICommand
    {
        /// <summary>
        /// 提供程序引用字段.
        /// </summary>
        private readonly RoslynContextActionProvider provider;

        /// <summary>
        /// 关联的代码动作字段.
        /// </summary>
        private readonly CodeAction codeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeActionCommand"/> class.
        /// </summary>
        /// <param name="provider">宿主提供程序实例.</param>
        /// <param name="codeAction">具体的代码动作.</param>
        public CodeActionCommand(RoslynContextActionProvider provider, CodeAction codeAction)
        {
            this.provider = provider;
            this.codeAction = codeAction;
        }

        /// <summary>
        /// 可执行状态变更事件.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// 确定命令是否可以执行.
        /// </summary>
        /// <param name="parameter">命令参数.</param>
        /// <returns>始终返回 true.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>
        /// 执行命令.
        /// </summary>
        /// <param name="parameter">命令参数.</param>
        public void Execute(object? parameter)
        {
            // 触发并忽略异步任务，防止 async void 带来的潜在问题.
            _ = this.provider.ExecuteCodeActionAsync(this.codeAction, CancellationToken.None);
        }
    }
}