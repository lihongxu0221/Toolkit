using BgCommon.Localization;
using BgCommon.Prism.Wpf.Modules;
using BgCommon.Script;
using ToolkitDemo.Services;
using ToolKitDemo.Services;

namespace ToolkitDemo;

/// <summary>
///  Bootstrapper.
/// </summary>
internal class Bootstrapper : BootstrapperBase
{
    public Bootstrapper(Application app)
        : base(app)
    {
    }

    protected override Type GetInitialServiceType()
    {
        return typeof(InitializationService);
    }

    protected override Type GetGlobalVarService()
    {
        return typeof(GlobalVarService);
    }

    /// <inheritdoc/>
    protected override Window CreateShell()
    {
        Window window = Container.Resolve<MainWindow>();

        // 强制主窗体最大化
        window.WindowState = WindowState.Maximized;
        return window;
    }

    protected override void RegisterStringLocalizer(LocalizationBuilder builder)
    {
        // CurveEdge3D
        builder.FromResource<Assets.Localization.Tanslations>(new CultureInfo("zh-CN"), false);
        builder.FromResource<Assets.Localization.Tanslations>(new CultureInfo("en-US"), false);

        builder.SetCulture(new CultureInfo("zh-CN"));
    }

    /// <inheritdoc/>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 3.依赖注入其他公共实体集合
        _ = containerRegistry.RegisterSingleton<IFeatureProvider, ToolkitDemoFeatureProvider>();
        _ = containerRegistry.RegisterSingleton<IMonitoringService, MonitoringService>();
        _ = containerRegistry.RegisterSingleton<MainWindow>();
        _ = containerRegistry.RegisterSingleton<PropertyGridDemoWindow>();
        _ = containerRegistry.RegisterSingleton<PropertyGridDemoWindow>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        _ = moduleCatalog.AddModule<ScriptModule>();
        base.ConfigureModuleCatalog(moduleCatalog);
    }
}