using Prism.Common;

namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 显示模块服务实现.
/// </summary>
internal class ModuleViewService : IModuleViewService
{
    private readonly IDialogService dialogService;
    private readonly IRegionManager regionManager;
    private readonly IAuthService authService;
    private readonly IModuleService moduleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleViewService"/> class.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="regionManager">区域管理器.</param>
    /// <param name="authService">权限校验服务.</param>
    /// <param name="moduleService">模块服务.</param>
    public ModuleViewService(
        IDialogService dialogService,
        IRegionManager regionManager,
        IAuthService authService,
        IModuleService moduleService)
    {
        this.dialogService = dialogService;
        this.regionManager = regionManager;
        this.authService = authService;
        this.moduleService = moduleService;
    }

    /// <inheritdoc/>
    public async Task RequestNavigateAsync(
        string regionName,
        UserInfo? currentUser,
        string moduleCode,
        string? permissionCode = null,
        Action<INavigationParameters>? configura = null,
        Action<IParameters>? callBack = null)
    {
        bool isNeedAuth = true;
        ResponseResult? result;
        if (!string.IsNullOrEmpty(permissionCode))
        {
            isNeedAuth = false;
            ResponseResult authResult = await this.authService.VerifyPermissionAsync(currentUser.Id, permissionCode);
            if (!authResult.Success)
            {
                result = ResponseResult.ToFail(authResult.Message, authResult.Code);

                if (callBack != null)
                {
                    DialogParameters parameters = new DialogParameters();
                    parameters.Add(Constraints.Result, result);
                    callBack.Invoke(parameters);
                }

                return;
            }
        }

        var moduleResp = await this.moduleService.GetModuleByCodeAsync(moduleCode);
        if (moduleResp.Success)
        {
            var moduleInfo = moduleResp.Result;
            if (moduleInfo != null)
            {
                if (!isNeedAuth || (currentUser != null && currentUser.MaxRole?.SystemRole >= moduleInfo.SystemRole))
                {
                    // 导航模块到区域.
                    this.regionManager.RequestNavigate(
                        regionName,
                        moduleInfo.ModuleType.Name,
                        configura: keys =>
                        {
                            keys.Add(Constraints.CurrentView, moduleInfo);
                            configura?.Invoke(keys);
                        },
                        callBack: navigationResult =>
                        {
                            IParameters parameters = navigationResult.Context.Parameters;
                            parameters.Add(Constraints.Result, ResponseResult.ToSuccess());
                            parameters.Add(Constraints.Parameters, navigationResult);
                            callBack?.Invoke(navigationResult.Context.Parameters);
                        });
                    return;
                }
                else
                {
                    result = ResponseResult.ToFail(
                        Lang.GetString(
                            "用户{UserName}没有模块{ModuleCode}的操作权限",
                            currentUser?.UserName ?? Lang.GetString("未登录"),
                            moduleCode),
                        500);
                }
            }
            else
            {
                result = ResponseResult.ToFail(Lang.GetString("未找到{ModuleCode}对应的模块", moduleCode), 400);
            }
        }
        else
        {
            result = ResponseResult.ToFail(moduleResp.Message, moduleResp.Code, moduleResp.Result);
        }

        if (callBack != null)
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add(Constraints.Result, result);
            callBack.Invoke(parameters);
        }
    }

    /// <inheritdoc/>
    public async Task<ResponseResult<IDialogResult>> ShowViewAsync(
        UserInfo? currentUser,
        string moduleCode,
        string? permissionCode = null,
        string? dialogWindowName = null,
        Action<IDialogParameters>? configuration = null)
    {
        bool isNeedAuth = true;
        if (!string.IsNullOrEmpty(permissionCode))
        {
            isNeedAuth = false;
            ResponseResult authResult = await this.authService.VerifyPermissionAsync(currentUser.Id, permissionCode);
            if (!authResult.Success)
            {
                return ResponseResult<IDialogResult>.ToFail(
                    authResult.Message,
                    authResult.Code,
                    new DialogResult(ButtonResult.No));
            }
        }

        var moduleResp = await this.moduleService.GetModuleByCodeAsync(moduleCode);
        if (!moduleResp.Success)
        {
            return ResponseResult<IDialogResult>.ToFail(
                moduleResp.Message,
                moduleResp.Code,
                new DialogResult(ButtonResult.No));
        }

        var moduleInfo = moduleResp.Result;
        if (moduleInfo == null)
        {
            return ResponseResult<IDialogResult>.ToFail(
                    Lang.GetString("未找到{ModuleCode}对应的模块", moduleCode),
                    400,
                    new DialogResult(ButtonResult.No));
        }

        if (currentUser == null || (isNeedAuth && currentUser.MaxRole?.SystemRole < moduleInfo.SystemRole))
        {
            return ResponseResult<IDialogResult>.ToFail(
                Lang.GetString(
                    "用户{UserName}没有模块{ModuleCode}的操作权限",
                    currentUser?.UserName ?? Lang.GetString("未登录"),
                    moduleCode),
                500,
                new DialogResult(ButtonResult.No));
        }

        // 导航模块到区域.
        IDialogResult? dialogResult = await this.dialogService.ShowAsync(
                moduleInfo.ModuleType.Name,
                dialogWindowName,
                config: keys =>
                {
                    keys.Add(Constraints.CurrentView, moduleInfo);
                    configuration?.Invoke(keys);
                });
        return ResponseResult<IDialogResult>.ToSuccess(1, Lang.GetString("操作成功"), dialogResult);
    }

    /// <inheritdoc/>
    public async Task<ResponseResult<IDialogResult>> ShowDialogViewAsync(
        UserInfo? currentUser,
        string moduleCode,
        string? permissionCode = null,
        string? dialogWindowName = null,
        Action<IDialogParameters>? configuration = null)
    {
        bool isNeedAuth = true;
        if (!string.IsNullOrEmpty(permissionCode))
        {
            isNeedAuth = false;
            ResponseResult authResult = await this.authService.VerifyPermissionAsync(currentUser.Id, permissionCode);
            if (!authResult.Success)
            {
                return ResponseResult<IDialogResult>.ToFail(
                    authResult.Message,
                    authResult.Code,
                    new DialogResult(ButtonResult.No));
            }
        }

        var moduleResp = await this.moduleService.GetModuleByCodeAsync(moduleCode);
        if (!moduleResp.Success)
        {
            return ResponseResult<IDialogResult>.ToFail(
                moduleResp.Message,
                moduleResp.Code,
                new DialogResult(ButtonResult.No));
        }

        var moduleInfo = moduleResp.Result;
        if (moduleInfo == null)
        {
            return ResponseResult<IDialogResult>.ToFail(
                    Lang.GetString("未找到{moduleCode}对应的模块", moduleCode),
                    400,
                    new DialogResult(ButtonResult.No));
        }

        if (currentUser == null || (isNeedAuth && currentUser.MaxRole?.SystemRole < moduleInfo.SystemRole))
        {
            return ResponseResult<IDialogResult>.ToFail(
                Lang.GetString(
                    "用户{userNaem}没有模块{moduleCode}的操作权限",
                    currentUser?.UserName ?? Lang.GetString("未登录"),
                    moduleCode),
                500,
                new DialogResult(ButtonResult.No));
        }

        // 导航模块到区域.
        IDialogResult? dialogResult = await this.dialogService.ShowDialogAsync(
                moduleInfo.ModuleType.Name,
                dialogWindowName,
                config: keys =>
                {
                    keys.Add(Constraints.CurrentView, moduleInfo);
                    configuration?.Invoke(keys);
                });
        return ResponseResult<IDialogResult>.ToSuccess(1, Lang.GetString("操作成功"), dialogResult);
    }
}
