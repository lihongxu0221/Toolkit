namespace BgCommon.Localization.DependencyInjection;

/// <summary>
/// Provides a provider-based implementation of the <see cref="IStringLocalizer{TResource}"/> interface.
/// </summary>
/// <typeparam name="TResource">The type of the resource used for localization.</typeparam>
/// <remarks>
/// This class uses an <see cref="ILocalizationProvider"/> to retrieve localization sets,
/// and an <see cref="ILocalizationCultureManager"/> to manage the current culture.
/// </remarks>
public class ProviderBasedStringLocalizer<TResource>: IStringLocalizer<TResource>
{
    private readonly ILocalizationProvider localizations;
    private readonly ILocalizationCultureManager cultureManager;

    public ProviderBasedStringLocalizer(ILocalizationProvider _localizations, ILocalizationCultureManager _cultureManager )
    {
        localizations = _localizations;
        cultureManager = _cultureManager;
    }
    /// <inheritdoc />
    public LocalizedString this[string name] => this[name, new object[0]];

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] =>
        LocalizeString(name, arguments);

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool _)
    {
        return localizations
                .GetLocalizationSet(
                    cultureManager.GetCulture(),
                    typeof(TResource).FullName?.ToLower()
                )
                ?.Strings.Select(x => new LocalizedString(x.Key, x.Value ?? x.Key)) ?? new LocalizedString[0];
    }

    /// <summary>
    /// Fills placeholders in a string with the provided values.
    /// </summary>
    /// <param name="name">The string with placeholders.</param>
    /// <param name="placeholders">The values to fill the placeholders with.</param>
    /// <returns>The string with filled placeholders.</returns>
    private LocalizedString LocalizeString(string name, object[] placeholders)
    {
        return new LocalizedString(
            name,
            FillPlaceholders(
                GetAllStrings(true).FirstOrDefault(x => x.Name == name) ?? name,
                placeholders
            )
        );
    }

    /// <summary>
    /// Fills placeholders in a string with the provided values.
    /// </summary>
    /// <param name="value">The string with placeholders.</param>
    /// <param name="placeholders">The values to fill the placeholders with.</param>
    /// <returns>The string with filled placeholders.</returns>
    private static string FillPlaceholders(string value, object[] placeholders)
    {
        for (int i = 0; i < placeholders.Length; i++)
        {
            value = value.Replace($"{{{i}}}", placeholders[i].ToString());
        }

        return value;
    }
}
