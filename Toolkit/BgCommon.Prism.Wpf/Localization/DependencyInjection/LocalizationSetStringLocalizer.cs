namespace BgCommon.Prism.Wpf.Localization.DependencyInjection;

/// <summary>
/// Provides a localization set-based implementation of the <see cref="IStringLocalizer"/> interface.
/// </summary>
/// <remarks>
/// This class uses a <see cref="LocalizationSet"/> to retrieve localized strings.
/// </remarks>
public class LocalizationSetStringLocalizer : IStringLocalizer
{
    private readonly LocalizationSet _localizationSet;

    public LocalizationSetStringLocalizer(LocalizationSet localizationSet)
    {
        _localizationSet = localizationSet;
    }

    /// <inheritdoc />
    public LocalizedString this[string name] => this[name, new object[0]];

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] =>
        LocalizeString(name, arguments);

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _localizationSet.Strings.Select(x => new LocalizedString(x.Key, x.Value ?? x.Key))
            ?? new LocalizedString[0];
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