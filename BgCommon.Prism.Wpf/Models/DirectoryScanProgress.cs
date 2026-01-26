namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 封装了目录扫描过程中的进度信息.
/// </summary>
public record DirectoryScanProgress
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