using BgCommon.Authority.Models;
using ModuleInfo = BgCommon.Authority.Models.ModuleInfo;

namespace BgCommon.Authority.Services.Implementation;

public abstract class ModuleServiceBase : ObservableObject, IModuleService
{
    private readonly IContainerExtension container;
    private readonly IUserService userService;
    private List<ModuleInfo> modules = new List<ModuleInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleServiceBase"/> class.
    /// </summary>
    /// <param name="container">Prism Ioc容器.</param>
    public ModuleServiceBase(IContainerExtension container)
    {
        this.container = container;
        this.userService = container.Resolve<IUserService>();
    }

    protected IContainerExtension Container => this.container;

    protected IUserService UserService => this.userService;

    public List<ModuleInfo> Modules
    {
        get => this.modules;
        private set => _ = SetProperty(ref this.modules, value);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<ModuleInfo> GetAllModules()
    {
        return this.Modules.ToList();
    }

    /// <inheritdoc/>
    public virtual IEnumerable<ModuleInfo> GetModules(int? parentId = null)
    {
        int[] ids = this.GetAuthority(this.userService.CurrentUser);
        return this.modules.Where(m => m.ParentModuleId == parentId && ids.Contains(m.ModuleId)).ToArray();
    }

    /// <inheritdoc/>
    public virtual bool Verify(ModuleInfo module)
    {
        if (module == null)
        {
            return false;
        }

        UserAuthority authority = UserAuthority.Operator;
        if (this.userService.CurrentUser != null)
        {
            authority = this.userService.CurrentUser.Authority;
        }

        return module.Authority <= authority;
    }

    public virtual bool Verify(int moduleId)
    {
        UserAuthority authority = UserAuthority.Operator;
        if (this.userService.CurrentUser != null)
        {
            authority = this.userService.CurrentUser.Authority;
        }

        return this.modules.Any(m => m.ModuleId == moduleId && m.Authority <= authority);
    }

    /// <summary>
    /// 获取当前用户的权限ID列表.
    /// </summary>
    /// <param name="user">当前用户信息.</param>
    /// <returns>返回用户的权限ID列表</returns>
    protected virtual int[] GetAuthority(UserInfo? user)
    {
        UserAuthority authority = user?.Authority ?? UserAuthority.Operator;

        // 根据用户权限类型返回对应的模块ID列表
        return this.modules.Where(m => m.Authority <= authority)
                           .Select(m => m.ModuleId)
                           .ToArray();
    }

    /// <inheritdoc/>
    public abstract void Initialize();
}