namespace BgCommon.Prism.Wpf.Localization.DependencyInjection;

/// <summary>
/// Provides a provider-based implementation of the <see cref="IStringLocalizerFactory"/> interface.
/// </summary>
/// <remarks>
/// This class uses an <see cref="ILocalizationProvider"/> to retrieve localization sets,
/// and an <see cref="ILocalizationCultureManager"/> to manage the current culture.
/// </remarks>
public class ProviderBasedStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ILocalizationProvider localizations;
    private readonly ILocalizationCultureManager cultureManager;

    public ProviderBasedStringLocalizerFactory(ILocalizationProvider _localizations, ILocalizationCultureManager _cultureManager)
    {
        localizations = _localizations;
        cultureManager = _cultureManager;
    }

    /// <inheritdoc />
    public IStringLocalizer Create(Type resourceSource)
    {
        string? baseName = resourceSource.FullName?.ToLower().Trim();

        return Create(baseName ?? default, default);
    }

    /// <inheritdoc />
    public IStringLocalizer Create(string? baseName, string? location)
    {
        LocalizationSet? resourceLocalizationSet = localizations.GetLocalizationSet(
            cultureManager.GetCulture(),
            baseName
        );

        if (resourceLocalizationSet is null)
        {
            resourceLocalizationSet = localizations.GetLocalizationSet(
                cultureManager.GetCulture(),
                default
            );
        }

        if (resourceLocalizationSet is null)
        {
            throw new InvalidOperationException(
                "No localization set found for the given resource source."
            );
        }

        return new LocalizationSetStringLocalizer(resourceLocalizationSet);
    }
}