namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 打开文件对话框的选项.
/// </summary>
public class OpenFileOptions
{
    /// <summary>
    /// Gets or sets 文件对话框标题.
    /// </summary>
    public string Title { get; set; } = "请选择文件";

    /// <summary>
    /// Gets or sets 初始目录路径.
    /// </summary>
    public string? InitialDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets 是否可多选文件.
    /// </summary>
    public bool Multiselect { get; set; } = false;

    /// <summary>
    /// Gets or sets 过滤器选项列表. 如果列表为空，则默认为 "所有文件".
    /// </summary>
    public List<FileFilter> Filters { get; set; } = new();
}