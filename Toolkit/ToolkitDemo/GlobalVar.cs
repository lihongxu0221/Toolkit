using BgCommon.Localization;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;

namespace ToolkitDemo;

/// <summary>
/// 全局通用变量.
/// </summary>
internal static class GlobalVar
{
    /// <summary>
    /// 程序基础路径.
    /// </summary>
    public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string LogoPath = Path.Combine(AppPath, "Assets\\Images\\logo.png");
    private static readonly string IconPath = Path.Combine(AppPath, "Assets\\Images\\Icon.ico");

    /// <summary>
    /// Gets ts Logo 图片.
    /// </summary>
    public static ImageSource? Logo { get; private set; }

    /// <summary>
    /// Gets ts 图标.
    /// </summary>
    public static ImageSource? Icon { get; private set; }

    /// <summary>
    /// Gets 应用程序名称.
    /// </summary>
    public static string SoftwareName => LocalizationProviderFactory.GetString(nameof(SoftwareName));

    /// <summary>
    /// Gets or sets 当前登录用户.
    /// </summary>
    public static UserInfo? CurrentUser { get; set; }

    /// <summary>
    /// 初始化全局通用变量.
    /// </summary>
    /// <returns>是否初始化成功</returns>
    public static bool Initialize()
    {
        // 预加载 Logo
        if (File.Exists(LogoPath))
        {
            Logo = new BitmapImage(new Uri(LogoPath, UriKind.Absolute));
        }

        // 预加载 Logo
        if (File.Exists(IconPath))
        {
            Icon = new BitmapImage(new Uri(IconPath, UriKind.Absolute));
        }

        return true;
    }
}