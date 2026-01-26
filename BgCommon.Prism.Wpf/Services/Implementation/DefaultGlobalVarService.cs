using BgCommon.Core;

namespace BgCommon.Prism.Wpf.Services;

internal class DefaultGlobalVarService : IGlobalVarService
{
    private readonly IContainerExtension container;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultGlobalVarService"/> class.
    /// </summary>
    /// <param name="container">Prism Ioc 容器.</param>
    public DefaultGlobalVarService(IContainerExtension container)
    {
        this.container = container;
    }

    /// <inheritdoc/>
    public bool IsDebugMode { get; set; }

    /// <inheritdoc/>
    public string? GetAppIconImage()
    {
        return FileNames.IconPath;
    }

    /// <inheritdoc/>
    public string? GetAppLogoImage()
    {
        return FileNames.LogoPath;
    }

    /// <inheritdoc/>
    public string? GetSplashScreenImage()
    {
        return FileNames.SplashImagePath;
    }

    /// <inheritdoc/>
    public string? GetAppName()
    {
        return "Baigu.Studio";
    }

    /// <inheritdoc/>
    public async Task<bool> InitializeAsync()
    {
        // 0. 初始化全局参数
        try
        {
            // // 载入软件配置,并根据配置初始化软件
            // ConfigurationMgr<SoftwareConfig>? softwareConfigMgr = SoftwareConfig.Load();
            // if (softwareConfigMgr != null && softwareConfigMgr.Entity != null)
            // {
            //     // FileNames.IconPath = softwareConfigMgr.Entity.IconImagePath;
            //     FileNames.LogoPath = softwareConfigMgr.Entity.LogoImagePath;
            //     FileNames.SplashImagePath = softwareConfigMgr.Entity.SplashImagePath;
            // }
            // // 预加载 AppName
            // GlobalVar.RefreshSoftwareSet();
        }
        catch (Exception ex)
        {
            LogDialog.Error(ex);
            return false;
        }

        // 1. 加载其他配置
        try
        {
            // 这里可以加载其他配置文件或初始化其他服务
            // 例如：加载数据库连接、初始化日志服务等
            // await this.container.Resolve<IConfigurationService>().LoadConfigurationsAsync();
        }
        catch (Exception ex)
        {
            LogDialog.Error(ex);
            return false;
        }

        await Task.Delay(0);
        return true;
    }
}