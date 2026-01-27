namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 用于在文件查找操作中报告进度的信息.
/// </summary>
public record FileSearchProgressReport
{
    /// <summary>
    /// Gets 当前正在扫描的目录.
    /// </summary>
    public string CurrentDirectory { get; init; } = string.Empty;

    /// <summary>
    /// Gets 到目前为止已找到的文件总数.
    /// </summary>
    public int FilesFound { get; init; }
}