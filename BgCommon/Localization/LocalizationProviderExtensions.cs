namespace BgCommon.Localization;

#nullable enable
/// <summary>
/// Provides extension methods for the <see cref="ILocalizationProvider"/> interface.
/// </summary>
public static class LocalizationProviderExtensions
{
    /// <summary>
    /// Retrieves a set of localized strings for a specific culture from the <see cref="ILocalizationProvider"/>.
    /// </summary>
    /// <param name="provider">The <see cref="ILocalizationProvider"/> to retrieve the localized strings from.</param>
    /// <param name="cultureName">The culture for which the localized strings are provided.</param>
    /// <returns>The set of localized strings, or null if no such set exists.</returns>
    public static LocalizationSet? GetLocalizationSet(
        this ILocalizationProvider provider,
        string cultureName
    )
    {
        return provider.GetLocalizationSet(new CultureInfo(cultureName), default);
    }

    /// <summary>
    /// Retrieves a set of localized strings for a specific culture and name from the <see cref="ILocalizationProvider"/>.
    /// </summary>
    /// <param name="provider">The <see cref="ILocalizationProvider"/> to retrieve the localized strings from.</param>
    /// <param name="cultureName">The culture for which the localized strings are provided.</param>
    /// <param name="name">The base name of the resource.</param>
    /// <returns>The set of localized strings, or null if no such set exists.</returns>
    public static LocalizationSet? GetLocalizationSet(
        this ILocalizationProvider provider,
        string cultureName,
        string name
    )
    {
        return provider.GetLocalizationSet(new CultureInfo(cultureName), name);
    }
}