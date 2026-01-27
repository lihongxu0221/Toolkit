namespace ToolkitDemo;

/// <summary>
/// 提供应用程序中使用的区域定义.
/// </summary>
/// <remarks>此类包含表示区域名称的常量，可用于标识应用程序用户界面中的特定区域或组件.
/// 这些区域名称通常与支持基于区域的 UI 组合的框架或库一起使用.</remarks>
public static class RegionDefine
{
    /// <summary>
    /// Gets 表示应用程序中使用的主区域名称.
    /// </summary>
    public static string MainRegion => nameof(MainRegion);

    /// <summary>
    /// Gets 表示应用程序中使用的主区域名称.
    /// </summary>
    public static string MainContentRegion => nameof(MainContentRegion);

    /// <summary>
    /// Gets 表示应用程序中使用的运控主区域名称.
    /// </summary>
    public static string MotionMainRegion => nameof(MotionMainRegion);

    /// <summary>
    /// Gets 表示应用程序中使用的视觉主区域名称.
    /// </summary>
    public static string VisionMainRegion => nameof(VisionMainRegion);

    /// <summary>
    /// Gets 表示应用程序中使用的CAD主区域名称.
    /// </summary>
    public static string CADMainRegion => nameof(CADMainRegion);

    /// <summary>
    /// Gets 表示应用程序中使用的系统参数设置主区域名称.
    /// </summary>
    public static string SysConfigMainRegion => nameof(SysConfigMainRegion);

    /// <summary>
    /// Gets 可调整大小的弹窗.
    /// </summary>
    public static string SizeableDialog => nameof(SizeableDialog);
}