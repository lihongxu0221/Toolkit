namespace BgCommon.Localization;

public static class LocalizationProviderFactory
{
    private static readonly ConcurrentDictionary<string, ILocalizationProvider> instances = new();

    public static ILocalizationProvider? GetInstance()
    {
        return GetInstance(string.Empty);
    }

    public static ILocalizationProvider? GetInstance(string key)
    {
        _ = instances.TryGetValue(key, out ILocalizationProvider? instance);

        return instance;
    }

    public static void SetInstance(ILocalizationProvider provider)
    {
        SetInstance(provider, string.Empty);
    }

    public static void SetInstance(ILocalizationProvider provider, string key)
    {
        _ = instances.AddOrUpdate(key, provider, (_, _) => provider);
    }

    /// <summary>
    /// 获取多语言字符串资源。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    public static string GetString(string key, params object[] args)
    {
        Assembly? assembly = Assembly.GetCallingAssembly();
        return GetString(assembly?.GetName()?.Name, key, args);
    }

    /// <summary>
    /// 获取多语言字符串资源
    /// </summary>
    /// <param name="assemblyName">资源包所在程序集名称</param>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    public static string GetString(string? assemblyName, string key, params object[] args)
    {
        string content = GetInstance()?.GetString(key, assemblyName) ?? key;

        if (args.Length > 0)
        {
            content = string.Format(content, args);
        }

        return content;
    }

    public static string[] GetStrings(string key, string? assemblyName = "")
    {
        if (string.IsNullOrEmpty(assemblyName))
        {
            Assembly? assembly = Assembly.GetCallingAssembly();
            assemblyName = assembly.GetName()?.Name;
        }

        List<CultureInfo> cultureInfos = new List<CultureInfo>();
        Array enumValues = Enum.GetValues(typeof(LanguageEnum));
        for (int i = 0; i < enumValues.Length; i++)
        {
            try
            {
                CultureInfo culture = new CultureInfo((int)enumValues.GetValue(i));
                cultureInfos.Add(culture);
            }
            catch
            {
                continue;
            }
        }

        string[] array = GetInstance()?.GetStrings(key, assemblyName, cultureInfos.ToArray())!;
        cultureInfos.Clear();
        return array;
    }
}