namespace BgControls.Controls;

/// <summary>
/// 用于在ListView中显示的文件/文件夹信息的数据类.
/// </summary>
public class FileSystemItemInfo
{
    /// <summary>
    /// Gets or sets 文件或文件夹的名称.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 文件或文件夹的完整路径.
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 文件或文件夹的图标.
    /// </summary>
    public ImageSource? Icon { get; set; }

    /// <summary>
    /// Gets or sets 文件或文件夹的最后修改时间.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets 文件或文件夹的类型描述.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 文件的大小（以字符串形式表示）.
    /// </summary>
    public string Size { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether 指示该项是否为文件夹.
    /// </summary>
    public bool IsDirectory { get; set; }
}