namespace BgCommon.Localization.IO;

/// <summary>
/// 为<see cref="LocalizationSet"/>提供将资源解析 .
/// </summary>
public static class LocalizationSetResourceParser
{
    /// <summary>
    /// 将指定程序集中的资源解析为 <see cref="LocalizationSet"/>.
    /// </summary>
    /// <param name="assembly">包含资源的程序集.</param>
    /// <param name="baseName">资源名称.</param>
    /// <param name="culture">提供资源的语言.</param>
    /// <param name="isPublic">是否为公共使用的语言包资源.</param>
    /// <returns>A <see cref="LocalizationSet"/> 包含解析的资源，如果找不到资源，则为null.</returns>
    /// <exception cref="LocalizationBuilderException">在程序集中找不到资源时抛出.</exception>
    public static LocalizationSet? Parse(Assembly assembly, string baseName, CultureInfo culture, bool isPublic)
    {
        CultureInfo cultureToRestore = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ResourceManager resourceManager = new ResourceManager(baseName, assembly);
            ResourceSet? resourceSet = resourceManager.GetResourceSet(culture, true, true);
            if (resourceSet is null)
            {
                return null;
            }

            string? assemblyName = assembly.GetName().Name;

            Dictionary<string, string?> localizations = resourceSet.Cast<DictionaryEntry>()
                .Where(x => x.Key is string)
                .ToDictionary(x => (string)x.Key!, x => (string?)x.Value);

            return new LocalizationSet(assembly, baseName.ToLowerInvariant(), culture, localizations, isPublic);
        }
        catch (MissingManifestResourceException ex)
        {
            throw new LocalizationBuilderException($"Failed to register translation resources for \"{culture}\".", ex);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = cultureToRestore;
            Thread.CurrentThread.CurrentUICulture = cultureToRestore;
        }
    }
}