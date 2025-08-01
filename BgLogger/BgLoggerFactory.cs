using BgControls.Controls;
using BgLogger.Logging.Models;
using BgLogger.Logging.Services;
using BgLogger.Logging.Services.Implementation;
using BgLogger.Logging.ViewModels;
using BgLogger.Logging.Views;

namespace BgLogger;

/// <summary>
/// BgLogger 工厂类，负责注册依赖、配置日志、初始化数据库等。
/// </summary>
public static class BgLoggerFactory
{
    /// <summary>
    /// 注册依赖类型和服务到容器。<br/>
    /// Prism 应用程序的 RegisterTypes 方法通常在应用程序启动时调用，<br/>
    /// </summary>
    /// <param name="containerRegistry">容器注册器。</param>
    public static void Register(IContainerRegistry containerRegistry)
    {
        // 0. 注册对话框服务（如消息框、输入框）
        containerRegistry.RegisterIocSingleton(ioc =>
        {
            ioc.Container.RegisterDialog<MessageBoxView, MessageBoxViewModel>();
            ioc.Container.RegisterDialog<InputMessageBoxView, InputMessageBoxViewModel>();
        });

        // 1. 加载 AppSettings 配置
        IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                  .AddJsonFile("bglogger.appsettings.json", optional: false, reloadOnChange: true);
        IConfiguration configuration = builder.Build();
        var appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(appSettings);
        _ = containerRegistry.RegisterInstance(appSettings); // 以单例方式注册
        _ = containerRegistry.RegisterInstance(configuration);

        // 2.配置 NLog 日志
         _ = LogManager.LogFactory.Setup().SetupExtensions(ext =>
         {
             // <--- 2b. 在实例上注册别名
             _ = ext.RegisterTarget<NLog.Targets.Wrappers.SplitGroupTarget>("Splitter");
         });

        LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

        // 3. 注册 Microsoft.Extensions.Logging 日志服务
        ServiceCollection serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddLogging(loggingBuilder =>
        {
            _ = loggingBuilder.ClearProviders();
            _ = loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

            // 注意：现在 AddNLog() 会使用我们刚刚配置好的全局 LogManager
            // 或传递 NLog.config 路径或 NLogConfiguration 对象
            _ = loggingBuilder.AddNLog(configuration);
        });

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        ILoggerFactory factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _ = containerRegistry.RegisterInstance(factory); // 用于 ILogger<T>

        // 4. 直接获取 NLog 的 ILogger 实例
        _ = containerRegistry.RegisterInstance(LogManager.GetLogger("Default")); // 默认 logger

        // 5. 注册自定义服务（如日志配置、数据库服务）
        _ = containerRegistry.RegisterSingleton<ILoggingConfigurationService, LoggingConfigurationService>();
        _ = containerRegistry.RegisterSingleton<IDatabaseService, SqliteDatabaseService>();

        // 6. 注册用于导航的视图（如主视图、历史日志视图）LoggerDialogAutoSizeWindow
        containerRegistry.RegisterDialogWindow<LoggerDialogWindow>(nameof(LoggerDialogWindow));
        containerRegistry.RegisterDialogWindow<LoggerDialogAutoSizeWindow>(nameof(LoggerDialogAutoSizeWindow));
        containerRegistry.RegisterDialog<MainView>();
        containerRegistry.RegisterDialog<HistoricalLogView, HistoricalLogViewModel>();
        containerRegistry.RegisterDialog<LoggerConfigView, LoggerConfigViewModel>();
        containerRegistry.RegisterDialog<LoggerDetailView, LoggerDetailViewModel>();

        // 7. 注册主视图模型单例
        if (containerRegistry is IContainerExtension containerExtension)
        {
            MainViewModel mainVM = new MainViewModel(containerExtension);
            _ = containerExtension.RegisterInstance(mainVM);
        }
        else
        {
            throw new InvalidOperationException("ContainerRegistry must be an instance of IContainerExtension to register MainViewModel.");
        }
    }

    /// <summary>
    /// 应用初始化时调用，初始化数据库并启动日志清理定时任务。<br/>
    /// Prism 应用程序的 OnInitialized 方法通常在应用程序启动时调用，<br/>
    /// </summary>
    public static void OnInitialized()
    {
        // 初始化数据库
        IDatabaseService? dbService = Ioc.Instance.Resolve<IDatabaseService>();
        if (dbService != null)
        {
            _ = dbService.InitializeDatabase();

            // 启动定时任务，定期清理旧日志
            ILoggingConfigurationService? logConfigService = Ioc.Instance.Resolve<ILoggingConfigurationService>();
            if (logConfigService != null)
            {
                var cleanupTimer = new System.Threading.Timer(
                    async _ =>
                    {
                        // 清理过期日志
                        _ = await dbService.CleanupOldLogsAsync(logConfigService.GetLogSourceSettings(), false);
                    },
                    null,
                    dueTime: TimeSpan.Zero,
                    TimeSpan.FromHours(24)); // 首次立即执行，之后每24小时执行一次
            }

            //// 示例：使用日志记录器记录应用启动信息
            // BgLoggerSource.General.Info("应用程序已启动。");

            //// 记录 MES 系统初始化日志
            // BgLoggerSource.MES.Info("MES系统初始化中...");
        }
    }

    /// <summary>
    /// 显示日志主视图的对话框窗口。<br/>
    /// 该方法异步弹出 MainView，并返回对话框结果。
    /// </summary>
    /// <returns>对话框结果（IDialogResult），如果未显示则为 null。</returns>
    public static async Task<IDialogResult?> ShowViewAsync()
    {
        return await Ioc.Instance.ShowAsync<MainView>(nameof(LoggerDialogWindow));
    }

    /// <summary>
    /// 导航到实时日志界面。<br/>
    /// </summary>
    /// <param name="regionName">导航到的区域名称</param>
    /// <param name="callBack">回调</param>
    public static async Task<NavigationResult> ShowViewAsync(string regionName, Action<NavigationResult>? callBack = null)
    {
        return await Ioc.Instance.RequestNavigateAsync(regionName, nameof(MainView), callBack: callBack);
    }

    /// <summary>
    /// 销毁 NLog 日志服务并刷新日志。
    /// </summary>
    public static void Destroy()
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        logger.Trace("应用程序正在退出...");
        LogManager.Shutdown(); // 刷新并关闭 NLog 目标
    }

    /// <summary>
    /// 异步清理过期的日志条目。
    /// </summary>
    public static async Task<bool> CleanupAllLogsAsync()
    {
        if (Ioc.Instance.IsRegistered(typeof(ILoggingConfigurationService)))
        {
            // 检查是否注册了日志配置服务
            ILoggingConfigurationService? logConfigService = Ioc.Instance.Resolve<ILoggingConfigurationService>();
            if (logConfigService != null)
            {
                IEnumerable<LogSourceSetting>? settings = logConfigService.GetLogSourceSettings();
                if (settings != null && settings.Any())
                {
                    if (Ioc.Instance.IsRegistered(typeof(IDatabaseService)))
                    {
                        // 检查是否注册了日志数据服务
                        IDatabaseService? databaseService = Ioc.Instance.Resolve<IDatabaseService>();
                        if (databaseService != null)
                        {
                            // 清理所有日志
                            return await databaseService.CleanupOldLogsAsync(settings, true);
                        }
                    }
                }
            }
        }

        return await Task.FromResult(false);
    }

    /// <summary>
    /// 异步获取历史日志文件大小.
    /// </summary>
    /// <returns>返回 历史日志文件大小.</returns>
    public static async Task<long> GetHistorySizeAsync()
    {
        try
        {
            if (Ioc.Instance.IsRegistered(typeof(IDatabaseService)))
            {
                IDatabaseService? databaseService = Ioc.Instance.Resolve<IDatabaseService>();

                if (databaseService != null)
                {
                    return await databaseService.GetHistoryFileSize();
                }
            }
        }
        catch (Exception ex)
        {
            return await Task.FromException<long>(ex);
        }

        return await Task.FromResult(0L);
    }
}