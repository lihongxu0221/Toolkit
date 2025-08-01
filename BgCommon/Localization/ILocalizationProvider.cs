namespace BgCommon.Localization;

/// <summary>
/// 提供检索特定区域性的本地化集的功能.
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>
    /// 检索具有指定区域性的指定名称的本地化集列表
    /// </summary>
    /// <param name="culture">实现本地化的文化.</param>
    /// <returns>具有指定区域性的指定名称的本地化集列表</returns>
    IEnumerable<LocalizationSet> GetLocalizationSets(CultureInfo culture);

    /// <summary>
    /// 检索具有指定区域性的指定名称的本地化集
    /// </summary>
    /// <param name="culture">实现本地化的文化.</param>
    /// <param name="name">要获取的本地化集的名称.</param>
    /// <returns>具有指定区域性的指定名称的本地化集，如果找不到本地化集，则为null.</returns>
    LocalizationSet? GetLocalizationSet(CultureInfo culture, string? name);

    /// <summary>
    /// 获取当前的文化
    /// </summary>
    /// <returns>当前的文化</returns>
    CultureInfo GetCulture();

    /// <summary>
    /// 设置当前的文化
    /// </summary>
    /// <param name="cultureInfo">要设置的文化</param>
    void SetCulture(CultureInfo cultureInfo);

    /// <summary>
    /// 遍历所有的区域语言资源包，检索具有指定区域的指定名称的本地化字符串
    /// </summary>
    /// <param name="key">本地化关键字</param>
    /// <returns>返回本地化字符串</returns>
    string GetString(string key, string? assembleyName = null);

    /// <summary>
    /// 遍历所有的区域语言资源包，检索具有指定区域的指定名称的本地化字符串<br/>
    /// 支持不同模块多语言关键字相同。不支持同一模块多语言关键字相同。<br/>
    /// 调用此方法时，默认只载入调用程序集的资源文件
    /// </summary>
    /// <param name="key">本地化关键字</param>
    /// <param name="cultureInfo">区域语言</param>
    /// <param name="assembleyName">程序集名称</param>
    /// <returns>返回本地化字符串</returns>
    string GetString(string key, CultureInfo cultureInfo, string? assembleyName = null);

    /// <summary>
    /// 遍历所有的区域语言资源包，检索所有本地化字符串<br/>
    /// </summary>
    /// <param name="key">本地化关键字</param>
    /// <param name="assembleyName">程序集名称</param>
    /// <param name="cultureInfos">区域语言列表</param>
    /// <returns>返回本地化字符串集合数组</returns>
    string[] GetStrings(string key, string? assembleyName, params CultureInfo[] cultureInfos);
}