using BgCommon.Prism.Wpf.Authority.Core;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.MVVM;
using BgLogger;
using ToolkitDemo.Models;
using ToolkitDemo.Views;
using ToolKitDemo.Services;

namespace ToolkitDemo.ViewModels;

/// <summary>
/// 主界面ViewModel类.
/// </summary>
public partial class MainViewModel : NavigationHostViewModelBase
{
    private readonly IRoleService roleService;
    private readonly ILoginService loginService; // 用户登陆管理服务
    private readonly IMonitoringService monitoringService;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private SystemRole? authority = SystemRole.Operator;

    [ObservableProperty]
    private string appVersion = "V 1.0.0";

    /// <summary>
    /// 系统实时监控信息.
    /// </summary>
    [ObservableProperty]
    private SystemRealTimeStatus? status = null;

    public MainViewModel(
        IContainerExtension container,
        IRoleService roleService,
        ILoginService loginService,
        IMonitoringService monitoringService)
        : base(container, RegionDefine.MainContentRegion)
    {
        this.roleService = roleService;
        this.loginService = loginService;
        this.SetUser(this.loginService.CurrentUser);
        this.monitoringService = monitoringService;
        this.Status = this.monitoringService.SystemStatus;
    }

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _ = this.EventAggregator.Subscribe<UserInfo?>(OnSwitchUser);
    }

    protected override void OnUnsubscribe()
    {
        base.OnUnsubscribe();
        this.EventAggregator.Unsubscribe<UserInfo?>(OnSwitchUser);
    }

    protected override void OnActivated()
    {
        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
    }

    private void OnSwitchUser(UserInfo? userInfo)
    {
        this.SetUser(userInfo);
        _ = this.ReloadAsync();
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);

        // 不等待执行结果.
        _ = this.InitializeMonitoringAsync();
    }

    /// <summary>
    /// 切换权限.
    /// </summary>
    [RelayCommand]
    private async Task OnLogin()
    {
        _ = await this.loginService.SwitchUserAsync();
    }

    [RelayCommand]
    private async Task OnShowNumberInputDemo()
    {
        // var result = await Ioc.ShowDialogAsync<NumberInputDemoView>();
        TestDemoView testDemoView = new TestDemoView();
        testDemoView.Show();
    }

    private void SetUser(UserInfo? user = null)
    {
        GlobalVar.CurrentUser = user;
        if (GlobalVar.CurrentUser == null)
        {
            this.UserName = this.GetString("未登陆");
            this.Authority = null;
        }
        else
        {
            this.UserName = GlobalVar.CurrentUser.UserName;

            var result = this.roleService.GetRolesByUserIdAsync(GlobalVar.CurrentUser.Id)
                .ConfigureAwait(true)
                .GetAwaiter()
                .GetResult();
            this.Authority = result.Result?.MaxRole?.SystemRole;
        }
    }

    /// <summary>
    /// 初始化启动后台监控.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task InitializeMonitoringAsync()
    {
        try
        {
            var startResult = await this.monitoringService.StartAsync()
                .ConfigureAwait(false);

            if (startResult?.Success != true)
            {
                return;
            }

            await this.monitoringService.StartMonitoringAsync(
                MonitoringTarget.Application | MonitoringTarget.Devices)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // 处理取消操作
        }
        catch (Exception ex)
        {
            // 处理其他异常
            LogRun.Error(ex, "Failed to initialize monitoring");
        }
    }
}
