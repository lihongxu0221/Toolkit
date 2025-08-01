namespace BgControls.Tools;

/// <summary>
///     资源帮助类
/// </summary>
internal class ResourceHelper
{
    private static ResourceDictionary? _theme;

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="key">资源名称</param>
    /// <returns>资源</returns>
    public static T? GetResource<T>(string key)
    {
        if (Application.Current.TryFindResource(key) is T resource)
        {
            return resource;
        }

        return default;
    }

    public static T? GetResourceInternal<T>(string key)
    {
        if (GetTheme()[key] is T resource)
        {
            return resource;
        }

        return default;
    }

    /// <summary>
    ///     get HandyControl theme
    /// </summary>
    public static ResourceDictionary GetTheme() => _theme ??= GetStandaloneTheme();

    public static ResourceDictionary GetStandaloneTheme()
    {
        return new()
        {
            Source = new Uri("pack://application:,,,/BgControls;component/Assets/Style/Theme.xaml")
        };
    }
}