namespace BgCommon.Prism.Wpf.Localization.DependencyInjection;

/// <summary>
/// Provides extension methods for the <see cref="ServiceCollectionExtensions"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the string localizer and related services to the specified <see cref="ServiceCollectionExtensions"/>.
    /// </summary>
    /// <param name="services">The <see cref="ServiceCollectionExtensions"/> to add services to.</param>
    /// <param name="configure">A delegate to configure the localization builder.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IContainerRegistry AddStringLocalizer(this IContainerRegistry services, Action<LocalizationBuilder> configure)
    {
        DependencyInjectionLocalizationBuilder builder;
        if (LocalizationProviderFactory.GetInstance() == null)
        {
            builder = new DependencyInjectionLocalizationBuilder(services);
            configure?.Invoke(builder);

            LocalizationProviderFactory.SetInstance(builder.Build());

            _ = services.RegisterSingleton(typeof(DependencyInjectionLocalizationBuilder), _ => builder);
            _ = services.RegisterSingleton(typeof(ILocalizationProvider), _ => LocalizationProviderFactory.GetInstance()!);
            _ = services.Register<IStringLocalizerFactory, ProviderBasedStringLocalizerFactory>();
            _ = services.Register<ILocalizationCultureManager, LocalizationCultureManager>();
            _ = services.Register<IStringLocalizer, ProviderBasedStringLocalizer>();
        }
        else
        {
            if (services is IContainerExtension container && container != null)
            {
                builder = container.Resolve<DependencyInjectionLocalizationBuilder>();
                configure?.Invoke(builder);
            }
        }

        return services;
    }
}