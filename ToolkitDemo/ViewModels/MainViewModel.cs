using BgCommon.Prism.Wpf;
using BgCommon.Prism.Wpf.Authority;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.MVVM;

namespace ToolkitDemo.ViewModels;

/// <summary>
/// 主界面ViewModel类.
/// </summary>
public partial class MainViewModel : NavigationHostViewModelBase, IPersistAcrossNavigation, ICachedView
{
    private readonly IAuthorityService userService;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private UserAuthority? authority = UserAuthority.Operator;

    [ObservableProperty]
    private string appVersion = "V 1.0.0";

    public MainViewModel(IContainerExtension container, IAuthorityService userService)
        : base(container, RegionDefine.MainContentRegion, true)
    {
        this.userService = userService;
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);
        this.Initialize(null);
    }

    /// <summary>
    /// 切换权限.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoginExecute))]
    private async Task OnLogin()
    {
        UserInfo? user = null;
        AuthorityResult result = await this.userService.ShowLoginViewAsync();
        if (result.Code != 0)
        {
            if (result.Success)
            {
                user = this.userService.CurrentUser;
            }

            this.SetUser(user);
        }
    }

    private bool CanLoginExecute => GlobalVar.CurrentUser == null;

    /// <summary>
    /// 登出.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanQuitExecute))]
    private async Task OnQuit()
    {
        UserInfo? user = null;
        var result = await this.userService.LogoutAsync();
        if (result)
        {
            user = this.userService.CurrentUser;
            this.SetUser(user);
        }
    }

    private bool CanQuitExecute => GlobalVar.CurrentUser != null;

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
            this.Authority = GlobalVar.CurrentUser.Authority;
        }

        // 重新初始化视图.
        this.Initialize();
    }
}
