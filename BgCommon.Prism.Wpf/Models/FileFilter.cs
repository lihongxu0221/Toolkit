namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 文件过滤器类，用于在文件选择对话框中指定可选的文件类型.
/// </summary>
public class FileFilter
{
    /// <summary>
    /// Gets 用于过滤的文件扩展名.
    /// </summary>
    public string Extension { get; private set; } = string.Empty;

    /// <summary>
    /// Gets 用于在文件选择对话框中显示的过滤条件的描述信息.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets 获取过滤字符串，用于文件选择对话框的过滤器.
    /// </summary>
    public string FilterString => $"{Description}|{Extension}";

    public FileFilter(string extension, string? description = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(extension, nameof(extension));

        if (string.IsNullOrWhiteSpace(description))
        {
            description = $"{Extension.TrimStart('.')}|{Extension}";
        }

        Description = description;
        Extension = extension;
    }
}