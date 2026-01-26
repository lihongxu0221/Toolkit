using BgCommon.Prism.Wpf.Authority.Models;
using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.Common.ViewModels;
using BgCommon.Prism.Wpf.Common.Views;
using BgCommon.Prism.Wpf.DependencyInjection;

namespace BgCommon.Prism.Wpf.Services.Implementation;

/// <summary>
/// 初始化服务基类
/// </summary>
public abstract class InitializationServiceBase : IInitializationService
{
    protected readonly IContainerExtension container;  // Prism Ioc容器实例.
    protected readonly IGlobalVarService globalVarService; // 弹窗服务.
    protected readonly IAuthorityService userService; // 用户服务.
    protected readonly IModuleService moduleService; // 模块.
    protected readonly IRegistrationService registrationService; // 动态注册服务.
    protected readonly IDialogService dialogService; // 弹窗服务.
    protected readonly IEventAggregator eventAggregator;

    private SplashScreenView? splashScreenView = null;

    public InitializationServiceBase(IContainerExtension container)
    {
        this.container = container;
        this.globalVarService = container.Resolve<IGlobalVarService>();
        this.userService = container.Resolve<IAuthorityService>();
        this.moduleService = container.Resolve<IModuleService>();
        this.registrationService = container.Resolve<IRegistrationService>();
        this.dialogService = container.Resolve<IDialogService>();
        this.eventAggregator = container.Resolve<IEventAggregator>();
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否处于调试模式.
    /// </summary>
    public bool IsDebugMode { get; set; }

    /// <summary>
    /// 加载数据.
    /// </summary>
    /// <returns>true: 加载成功 false:加载失败.</returns>
    private async Task<bool> LoadAsync()
    {
        try
        {
            // 加载模块 动态注入导航视图，弹窗视图，普通视图.
            this.OnReport(10, "[加载模块]", "正在动态注入导航视图，弹窗视图，普通视图...");
            this.registrationService.RegisterViews();
            await Task.Delay(10);

            this.OnReport(20, "[加载模块]", "功能模块服务初始化...");
            if (!await this.moduleService.InitializeAsync())
            {
                await Task.Delay(100);
                return false;
            }

            this.OnReport(30, "[加载模块]", "功能模块服务初始化完成...");

            // 加载模块.
            IModuleManager moduleManager = this.container.Resolve<IModuleManager>();
            var modules = moduleManager.Modules.ToList();
            for (int i = 0; i < modules.Count; i++)
            {
                IModuleInfo module = modules[i];
                this.OnReport(30 + (i * 20 / modules.Count), "[加载模块]", $"加载模块: {module.ModuleName}");
                await Task.Run(() => moduleManager.LoadModule(module.ModuleName));
            }

            // 载入应用程序需要的数据.
            this.OnReport(50, "[应用程序数据]", "开始载入应用程序数据...");
            if (!await OnLoadAsync(50))
            {
                await Task.Delay(100);
                return false;
            }

            // 完成.
            this.OnReport(100, "[初始化完成]", "初始化完成");
            await Task.Delay(10);
            return true;
        }
        catch (Exception ex)
        {
            LogRun.Error(ex, "应用程序初始化失败:{0}", ex.Message);
            string errorMsg = $"应用程序初始化失败: {ex.Message}";

            // 处理初始化错误.
            _ = Application.Current.Dispatcher.Invoke(async () =>
            {
                this.OnReport(100, "[加载出错]", errorMsg);
                _ = await dialogService.ErrorAsync(errorMsg);
            });

            return false;
        }
        finally
        {
            // 关闭启动画面.
            if (this.splashScreenView != null)
            {
                await this.splashScreenView.CloseAsync();
            }
        }
    }

    /// <summary>
    /// 初始化相关数据.
    /// </summary>
    private async Task<bool> InitializeAsync()
    {
        this.IsDebugMode = System.IO.File.Exists("debugtest.txt");
        this.globalVarService.IsDebugMode = this.IsDebugMode;
        this.userService.IsDebugMode = this.IsDebugMode;
        this.moduleService.IsDebugMode = this.IsDebugMode;

        // 0. 加载全局配置.
        if (!await this.globalVarService.InitializeAsync())
        {
            return false;
        }

        // 1. 用户服务初始化
        if (!await this.userService.InitializeAsync())
        {
            return false;
        }

        // 2. 更多的初始化方法
        if (!await OnInitializeAsync())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 启动画面.
    /// </summary>
    /// <returns>Task.</returns>
    private Task ShowSplashScreenAsync()
    {
        return Task.Run(() =>
        {
            _ = Application.Current.Dispatcher.BeginInvoke(() =>
            {
                // 创建启动画面.
                this.splashScreenView = new SplashScreenView()
                {
                    DataContext = this.container.Resolve<SplashScreenViewModel>(),
                    ShowInTaskbar = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true,
                };

                // 显示启动画面.
                _ = this.splashScreenView?.ShowDialog();
            });
        });
    }

    /// <summary>
    /// 进度上报.
    /// </summary>
    /// <param name="progress">进度百分比.</param>
    /// <param name="title">标题.</param>
    /// <param name="content">内容.</param>
    protected void OnReport(double progress, string title, string content)
    {
        eventAggregator.Publish<(double, string, string)>((progress, title, content));
    }

    protected virtual void Dispose(bool isDispose)
    {
    }

    /// <summary>
    /// 登录.
    /// </summary>
    /// <returns>Task.</returns>
    protected virtual async Task<bool> OnLoginAsync()
    {
        bool result = false;
        AuthorityResult loginRet = await this.userService.ShowLoginViewAsync();
        if (loginRet != null && loginRet.Success)
        {
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 更多的初始化.
    /// </summary>
    /// <returns>初始化是否成功.</returns>
    protected virtual Task<bool> OnInitializeAsync()
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// 载入其他内容.
    /// </summary>
    protected virtual Task<bool> OnLoadAsync(int startProgress)
    {
        return Task.FromResult(true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> RunAsync()
    {
        // 0. 初始化失败，关闭进程并返回
        if (!await this.InitializeAsync())
        {
            return false;
        }

        // 1. 登陆鉴权失败，关闭进程并返回.
        if (!await this.OnLoginAsync())
        {
            return false;
        }

        // 0. 显示启动界面.
        await this.ShowSplashScreenAsync();

        // 1. 整体加载.
        if (!await this.LoadAsync())
        {
            return false;
        }

        return true;
    }
}