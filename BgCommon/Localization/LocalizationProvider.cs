namespace BgCommon.Localization;

public class LocalizationProvider : ILocalizationProvider
{
    // private readonly IEventAggregator? eventAggregator = null;
    private readonly IEnumerable<LocalizationSet> localizationSets;
    private CultureInfo currentCulture;

    public LocalizationProvider(CultureInfo currentCulture, IEnumerable<LocalizationSet> localizationSets)// , IEventAggregator? eventAggregator, IEnumerable<LocalizationSet> localizationSets)
    {
        this.currentCulture = currentCulture;
        this.localizationSets = localizationSets;
        //this.eventAggregator = eventAggregator;
    }

    /// <inheritdoc />
    public IEnumerable<LocalizationSet> GetLocalizationSets(CultureInfo culture)
    {
        return localizationSets.Where(s => s.Culture.Equals(culture));
    }

    /// <inheritdoc />
    public LocalizationSet? GetLocalizationSet(CultureInfo culture, string? name)
    {
        if (name is null)
        {
            return localizationSets.FirstOrDefault(s => s.Culture.Equals(culture));
        }

        return localizationSets.FirstOrDefault(s => s.Culture.Equals(culture) && s.Name == name);
    }

    /// <inheritdoc />
    public CultureInfo GetCulture()
    {
        return currentCulture;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo cultureInfo)
    {
        bool isChanged = !currentCulture.Equals(cultureInfo);
        this.currentCulture = cultureInfo;
        if (isChanged)
        {
            // eventAggregator?.GetEvent<Prism.Events.PubSubEvent<ILocalizationProvider?>>().Publish(LocalizationProviderFactory.GetInstance());
            Ioc.Instance.Publish(LocalizationProviderFactory.GetInstance());
        }
    }

    /// <inheritdoc />
    public string GetString(string key, string? assembleyName)
    {
        return GetString(key, GetCulture(), assembleyName);
    }

    /// <inheritdoc />
    public string GetString(string key, CultureInfo culture, string? assembleyName)
    {

        IEnumerable<LocalizationSet> localSets = GetLocalizationSets(culture);
        if (localSets != null)
        {
            foreach (LocalizationSet? localSet in localSets)
            {
                if (localSet != null && localSet.ContainKey(key) && localSet.IsSame(assembleyName))
                {
                    return localSet[key] ?? key;
                }
            }
        }

        return key;
    }

    /// <inheritdoc />
    public string[] GetStrings(string key, string? assembleyName, params CultureInfo[] cultureInfos)
    {
        List<string> results = new List<string>();

        for (int i = 0; i < cultureInfos.Length; i++)
        {
            IEnumerable<LocalizationSet> localSets = GetLocalizationSets(cultureInfos[i]);
            if (localSets != null)
            {
                foreach (LocalizationSet? localSet in localSets)
                {
                    if (localSet != null && localSet.ContainKey(key) && localSet.IsSame(assembleyName))
                    {
                        results.Add(localSet[key]!);
                    }
                }
            }
        }

        string[] array = results.ToArray();
        results.Clear();
        return array;
    }
}