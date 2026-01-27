namespace RoslynPad.UI;

/// <summary>
/// 重命名符号对话框接口.
/// </summary>
public interface IRenameSymbolDialog : IDialog
{
    /// <summary>
    /// Gets a value indicating whether 是否应该执行重命名操作.
    /// </summary>
    bool ShouldRename { get; }

    /// <summary>
    /// Gets or sets 符号名称.
    /// </summary>
    string SymbolName { get; set; }

    /// <summary>
    /// 初始化对话框数据.
    /// </summary>
    /// <param name="symbolName">符号的初始名称.</param>
    void Initialize(string symbolName);
}