namespace RoslynPad.UI;

/// <summary>
/// 文件夹浏览对话框接口.
/// </summary>
public interface IFolderBrowserDialog
{
    /// <summary>
    /// Gets or sets a value indicating whether 是否显示编辑框.
    /// </summary>
    bool ShowEditBox { get; set; }

    /// <summary>
    /// Gets or sets 选择的路径.
    /// </summary>
    string SelectedPath { get; set; }

    /// <summary>
    /// 显示对话框.
    /// </summary>
    /// <returns>如果用户点击确定，则返回 true；如果点击取消，则返回 false；如果直接关闭，则返回 null.</returns>
    bool? Show();
}