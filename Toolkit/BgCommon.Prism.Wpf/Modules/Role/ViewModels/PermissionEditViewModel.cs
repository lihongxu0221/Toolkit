using Prism.Common;

namespace BgCommon.Prism.Wpf.Modules.Role.ViewModels;

/// <summary>
/// 操作权限新增或编辑
/// </summary>
public partial class PermissionEditViewModel : EditViewModelBase<Permission>
{
    private readonly IModuleService moduleService;
    private readonly ILoginService loginService;

    /// <summary>
    /// 模块信息列表.
    /// </summary>
    [ObservableProperty]
    private ObservableRangeCollection<ModuleInfo> modules = new ObservableRangeCollection<ModuleInfo>();

    protected PermissionEditViewModel(
        IContainerExtension container,
        IModuleService moduleService,
        ILoginService loginService)
        : base(container)
    {
        this.moduleService = moduleService;
        this.loginService = loginService;
    }

    protected override void OnDispose()
    {
        UIService.RunOnUIThread(() =>
       {
           Modules.Clear();
       });

        base.OnDispose();
    }

    protected override async Task OnAfterInitializedAsync(IParameters parameters)
    {
        ResponseResult<List<ModuleInfo>> response = await moduleService.GetAllSystemModulesAsync(loginService.CurrentUser);
        if (response.Success && response.Result != null)
        {
            await UIService.RunOnUIThreadAsync(() =>
             {
                 Modules.Clear();
                 Modules.AddRange(response.Result);
             });
        }
        else
        {
            LogRun.Error(response.Message);
            _ = await this.ErrorAsync(response.Message);
        }
    }

    protected override Task OnInitializedAsync(IDialogParameters parameters)
    {
        return base.OnInitializedAsync(parameters);
    }

    protected override Task OnInitializedAsync(NavigationContext navigationContext)
    {
        return base.OnInitializedAsync(navigationContext);
    }

    protected override bool OnCustomVerifyOK(object? parameter, ref IDialogParameters keys)
    {
        return base.OnCustomVerifyOK(parameter, ref keys);
    }
}