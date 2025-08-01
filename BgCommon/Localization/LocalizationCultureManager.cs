namespace BgCommon.Localization;

/// <summary>
/// Provides functionality to manage the current culture for localization.
/// </summary>
public class LocalizationCultureManager : ILocalizationCultureManager
{
    /// <inheritdoc />
    public CultureInfo GetCulture()
    {
        return LocalizationProviderFactory.GetInstance()?.GetCulture()
               ?? CultureInfo.CurrentCulture;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture)
    {
        LocalizationProviderFactory.GetInstance()?.SetCulture(culture);
    }
}