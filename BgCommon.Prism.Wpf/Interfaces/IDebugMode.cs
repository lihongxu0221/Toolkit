namespace BgCommon.Prism.Wpf;

/// <summary>
/// 是否为调试模式
/// </summary>
public interface IDebugMode
{
    /// <summary>
    /// Gets or sets a value indicating whether 是否处于调试模式.
    /// </summary>
    bool IsDebugMode { get; set; }
}