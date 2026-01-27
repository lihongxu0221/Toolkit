namespace RoslynPad.UI;

/// <summary>
/// 打开文件对话框接口.
/// </summary>
public interface IOpenFileDialog
{
    /// <summary>
    /// Gets or sets a value indicating whether 是否允许选择多个文件.
    /// </summary>
    bool AllowMultiple { get; set; }

    /// <summary>
    /// Gets or sets 文件过滤器.
    /// </summary>
    FileDialogFilter Filter { get; set; }

    /// <summary>
    /// Gets or sets 初始显示目录.
    /// </summary>
    string InitialDirectory { get; set; }

    /// <summary>
    /// Gets or sets 初始文件名或当前选择的文件名.
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// 异步显示打开文件对话框.
    /// </summary>
    /// <returns>异步操作任务，结果为选中的文件路径数组；如果用户取消操作，则返回 null.</returns>
    Task<string[]?> ShowAsync();
}