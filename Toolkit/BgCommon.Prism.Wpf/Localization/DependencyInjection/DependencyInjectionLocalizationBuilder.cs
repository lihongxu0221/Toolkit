namespace BgCommon.Prism.Wpf.Localization.DependencyInjection;

/// <summary>
/// Provides a dependency injection-based implementation of the <see cref="LocalizationBuilder"/> class.
/// </summary>
/// <remarks>
/// This class uses the .NET Core dependency injection framework to register localization services.
/// It allows the registration of resources for specific cultures using the <see cref="FromResource{TResource}"/> method.
/// </remarks>
public class DependencyInjectionLocalizationBuilder : LocalizationBuilder
{
    private readonly IContainerRegistry services;

    public DependencyInjectionLocalizationBuilder(IContainerRegistry services)
    {
        this.services = services;
    }

    /// <inheritdoc />
    public override void FromResource<TResource>(CultureInfo culture, bool isPublic)
    {
        base.FromResource<TResource>(culture, isPublic);
        _ = services.RegisterSingleton<IStringLocalizer<TResource>, ProviderBasedStringLocalizer<TResource>>();
    }
}