using RoslynPad.Themes;

namespace BgCommon.Script;

/// <summary>
/// 窗体扩展.
/// </summary>
public static partial class WindowExtensions
{
    public static void UseImmersiveDarkMode(this UserControl userControl, ThemeType theme)
    {
        Window parentWindow = Window.GetWindow(userControl);
        if (parentWindow != null)
        {
            parentWindow.UseImmersiveDarkMode(theme);
        }
    }

    public static void UseImmersiveDarkMode(this Window window, ThemeType theme)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();
        var error = DwmSetWindowAttribute(
            hwnd,
            DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE,
            theme == ThemeType.Dark,
            Marshal.SizeOf<bool>());

        if (error != 0)
        {
            throw new Win32Exception(error);
        }
    }

    [LibraryImport("dwmapi", SetLastError = true)]
    private static partial int DwmSetWindowAttribute(
        IntPtr hwnd,
        DwmWindowAttribute attribute,
        [MarshalAs(UnmanagedType.Bool)] in bool pvAttribute,
        int cbAttribute);

    private enum DwmWindowAttribute
    {
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
    }
}
