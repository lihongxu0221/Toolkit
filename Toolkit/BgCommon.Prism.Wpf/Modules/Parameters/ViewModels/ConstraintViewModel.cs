using Prism.Common;

namespace BgCommon.Prism.Wpf.Modules.Parameters.ViewModels;

/// <summary>
/// 参数约束VM.
/// </summary>
public partial class ConstraintViewModel : CrudViewModelBase<ParameterConstraint>
{
    private readonly ISystemParameterService systemParameterService;
    private readonly ILoginService loginService;

    private SystemParameter systemParameter = new SystemParameter();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstraintViewModel"/> class.
    /// </summary>
    /// <param name="container">Ioc容器.</param>
    /// <param name="systemParameterService">系统参数服务.</param>
    /// <param name="loginService">登录服务.</param>
    public ConstraintViewModel(
        IContainerExtension container,
        ISystemParameterService systemParameterService,
        ILoginService loginService)
        : base(container)
    {
        this.systemParameterService = systemParameterService;
        this.loginService = loginService;
    }

    /// <inheritdoc/>
    public override bool IsNavigateToItem => false;

    /// <inheritdoc/>
    protected override string CreateViewCode => ModuleCodes.SystemParameterConstraintCreateView;

    /// <inheritdoc/>
    protected override string EditViewCode => ModuleCodes.SystemParameterConstraintEditView;

    /// <inheritdoc/>
    protected override void UpdateEntity(ref ParameterConstraint entity, DataOperation operation)
    {
        if (operation == DataOperation.Create)
        {
            entity.ParameterId = systemParameter.Id;
            entity.SystemParameter = systemParameter;
            return;
        }
    }

    /// <inheritdoc/>
    public override bool IsNavigationTarget(NavigationContext navigationContext) => false;

    /// <inheritdoc/>
    protected override object GetEntityId(ParameterConstraint entity) => entity.Id;

    /// <inheritdoc/>
    protected override Task OnBeforeInitializedAsync(IParameters parameters)
    {
        if (parameters.TryGetValue<SystemParameter>(Constraints.Parameter, out SystemParameter? parameter) && parameter != null)
        {
            this.systemParameter = parameter;
        }

        return base.OnBeforeInitializedAsync(parameters);
    }

    /// <inheritdoc/>
    protected override Task OnAfterInitializedAsync(IParameters parameters)
    {
        return base.OnAfterInitializedAsync(parameters);
    }

    /// <inheritdoc/>
    protected override Task<bool> OnConfirmNavigationRequestAsync(NavigationContext navigationContext)
    {
        return base.OnConfirmNavigationRequestAsync(navigationContext);
    }

    /// <inheritdoc/>
    protected override bool OnVerifySave(ParameterConstraint entity, DataOperation operation)
    {
        if (operation == DataOperation.Delete)
        {
            return true;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override async Task<ResponseResult<ParameterConstraint>> OnCreateAsync(ParameterConstraint entity, CancellationToken token)
    {
        return await systemParameterService.AddConstraintAsync(systemParameter.Id, entity, loginService.CurrentUser!);
    }

    /// <inheritdoc/>
    protected override async Task<ResponseResult<ParameterConstraint>> OnDeleteAsync(ParameterConstraint entity, CancellationToken token)
    {
        return await systemParameterService.DeleteConstraintAsync(entity.Id, loginService.CurrentUser!);
    }

    /// <inheritdoc/>
    protected override async Task<ResponseResult<List<ParameterConstraint>>> OnSelectAsync(CancellationToken token)
    {
        return await systemParameterService.GetConstraintsAsync(systemParameter.Id, loginService.CurrentUser!);
    }

    /// <inheritdoc/>
    protected override async Task<ResponseResult<ParameterConstraint>> OnUpdateAsync(ParameterConstraint entity, CancellationToken token)
    {
        return await systemParameterService.UpdateConstraintAsync(entity, loginService.CurrentUser!);
    }
}