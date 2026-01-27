using Prism.Common;

namespace BgCommon.Prism.Wpf.Modules.User.ViewModels;

/// <summary>
/// 用户新增和修改.
/// </summary>
public partial class UserEditViewModel : EditViewModelBase<UserInfo>
{
    private readonly IRoleService roleService;
    private readonly IModuleService moduleService;
    private readonly ILoginService loginService;
    private readonly IUserAccessRightsService userAccessRightsService;

    private HashSet<long>? userAccessRightIds;

    /// <summary>
    /// 选中的角色.
    /// </summary>
    [ObservableProperty]
    private ERole? selectedRole = null;

    /// <summary>
    /// 可用的角色列表.
    /// </summary>
    [ObservableProperty]
    private ObservableRangeCollection<ERole> roles = new ObservableRangeCollection<ERole>();

    /// <summary>
    /// 可以用的模块列表.
    /// </summary>
    [ObservableProperty]
    private ObservableRangeCollection<ModuleInfo> modules = new ObservableRangeCollection<ModuleInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEditViewModel"/> class.
    /// </summary>
    /// <param name="container">Prism Ioc容器.</param>
    /// <param name="roleService">角色服务.</param>
    /// <param name="moduleService">模块服务.</param>
    /// <param name="loginService">登录服务.</param>
    public UserEditViewModel(
        IContainerExtension container,
        IRoleService roleService,
        IModuleService moduleService,
        ILoginService loginService)
        : base(container)
    {
        this.roleService = roleService;
        this.moduleService = moduleService;
        this.loginService = loginService;
        this.userAccessRightsService = Ioc.Resolve<IUserAccessRightsService>();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync(IDialogParameters parameters)
    {
        await base.OnInitializedAsync(parameters);
        await this.OnInitializeAsync(parameters);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync(NavigationContext navigationContext)
    {
        await base.OnInitializedAsync(navigationContext);
        await this.OnInitializeAsync(navigationContext.Parameters);
    }

    /// <inheritdoc/>
    protected override bool OnCustomVerifyOK(object? parameter, ref IDialogParameters keys)
    {
        return base.OnCustomVerifyOK(parameter, ref keys);
    }

    private async Task OnInitializeAsync(IParameters keys)
    {
        await this.LoadUserAccessRightsIdsAsync();
        await this.LoadRolesAsync();
    }

    async partial void OnSelectedRoleChanged(ERole? value)
    {
        await LoadModuleListAsync(value);
    }

    private async Task LoadRolesAsync()
    {
        var response = await this.roleService.GetAllRolesAsync();
        if (!response.Success)
        {
            LogRun.Error(response.Message);
            return;
        }

        await this.UIService.RunOnUIThreadAsync(() =>
        {
            this.Roles.Clear();
            this.Roles.AddRange(response.Result!);

            if (this.Entity != null)
            {
                this.SelectedRole = this.Roles.FirstOrDefault(r => r.Id == this.Entity.MaxRole?.Id);
            }
        });
    }

    private async Task LoadUserAccessRightsIdsAsync()
    {
        if (this.Entity == null)
        {
            return;
        }

        var response = await this.userAccessRightsService.GetDirectlyGrantedModuleIdsAsync(this.Entity.Id);
        if (!response.Success)
        {
            LogRun.Error(response.Message);
            return;
        }

        this.userAccessRightIds = response.Result;
    }

    private async Task LoadModuleListAsync(ERole? role)
    {
        var response = await this.moduleService.GetAllSystemModulesAsync(this.loginService.CurrentUser);
        if (!response.Success)
        {
            LogRun.Error(response.Message);
            return;
        }

        await this.UIService.RunOnUIThreadAsync(() =>
        {
            this.Modules.Clear();
            this.Modules.AddRange(response.Result!.Where(m => m.ParentId == null || m.ParentId == 0));
        });
    }
}
