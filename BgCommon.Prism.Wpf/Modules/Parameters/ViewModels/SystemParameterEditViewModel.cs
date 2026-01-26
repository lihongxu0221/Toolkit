using Prism.Common;

namespace BgCommon.Prism.Wpf.Modules.Parameters.ViewModels;

/// <summary>
/// 系统参数编辑.
/// </summary>
public partial class SystemParameterEditViewModel : EditViewModelBase<SystemParameter>
{
    private readonly IModuleService moduleService;
    private readonly ILoginService loginService;

    /// <summary>
    /// 模块信息列表.
    /// </summary>
    [ObservableProperty]
    private ObservableRangeCollection<ModuleInfo> modules = new ObservableRangeCollection<ModuleInfo>();

    public SystemParameterEditViewModel(
        IContainerExtension container,
        IModuleService moduleService,
        ILoginService loginService,
        ISystemParameterService systemParameterService)
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

        if (IsCreateNew)
        {
            Entity.CreatedBy = this.loginService.CurrentUser.UserName;
            Entity.ModifiedBy = this.loginService.CurrentUser.UserName;
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
        if (!IsCreateNew)
        {
            Entity.LastModifiedAt = DateTime.Now;
            Entity.ModifiedBy = this.loginService.CurrentUser.UserName;
        }

        return base.OnCustomVerifyOK(parameter, ref keys);
    }

    [RelayCommand]
    private void OnClearModule()
    {
        if (this.Entity != null)
        {
            this.Entity.ModuleId = null;
        }
    }
}