namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 选择文件夹的选项类。
/// </summary>
public class OpenFolderOptions
{
    /// <summary>
    /// Gets or sets 弹窗标题.
    /// </summary>
    public string Title { get; set; } = "请选择文件夹";

    /// <summary>
    /// Gets or sets 初始目录路径。
    /// </summary>
    public string? InitialDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets
    /// 注意：标准的 FolderBrowserDialog 不支持多选。
    ///      如果需要多选，必须使用更现代的Windows API库。
    ///      这里我们保留这个属性，但会在实现中说明其限制。
    /// </summary>
    public bool Multiselect { get; set; } = false;

    /// <summary>
    /// Gets or sets 搜索选项. 默认为 SearchOption.TopDirectoryOnly.
    /// </summary>
    public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
}