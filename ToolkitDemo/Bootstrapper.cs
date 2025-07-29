using BgCommon;
using BgCommon.Localization.DependencyInjection;
using BgCommon.Prism.Wpf;
using ToolkitDemo.Services;

namespace ToolkitDemo;

/// <summary>
///  Bootstrapper
/// </summary>
internal class Bootstrapper : BootstrapperBase
{
    public Bootstrapper(Application app)
        : base(app)
    {
    }

    protected override string[]? ModuleDirectories => null;

    protected override Type GetInitialServiceType()
    {
        return typeof(InitializationService);
    }

    protected override Type GetGlobalVarService()
    {
        return typeof(GlobalVarService);
    }

    protected override Type GetModuleService()
    {
        return typeof(ModuleService);
    }

    /// <inheritdoc/>
    protected override Window CreateShell()
    {
        Window window = Container.Resolve<MainWindow>();

        // 强制主窗体最大化
        window.WindowState = WindowState.Maximized;
        return window;
    }

    /// <inheritdoc/>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        _ = containerRegistry.AddStringLocalizer(b =>
        {
            // CurveEdge3D
            b.FromResource<Assets.Localization.Tanslations>(new CultureInfo("zh-CN"), false);
            b.FromResource<Assets.Localization.Tanslations>(new CultureInfo("en-US"), false);

            b.SetCulture(new CultureInfo("zh-CN"));
        });

        // 3.依赖注入其他公共实体集合
        _ = containerRegistry.RegisterSingleton<MainWindow>();
    }
}