using BgCommon;
using BgCommon.Prism.Wpf.Authority.Models;
using ModuleInfo = BgCommon.Prism.Wpf.Authority.Models.ModuleInfo;

namespace BgCommon.Prism.Wpf.Authority.Services.Implementation;

public abstract class ModuleServiceBase : ObservableObject, IModuleService
{
    private readonly IContainerExtension container;
    private readonly IUserService userService;
    private List<ModuleInfo> modules = new List<ModuleInfo>();

    public ModuleServiceBase(IContainerExtension container)
    {
        this.container = container;
        userService = container.Resolve<IUserService>();
    }

    protected IContainerExtension Container => container;

    protected IUserService UserService => userService;

    public List<ModuleInfo> Modules
    {
        get => modules;
        private set => _ = SetProperty(ref modules, value);
    }

    public bool IsDebugMode { get; set; }

    public virtual IEnumerable<ModuleInfo> GetAllModules()
    {
        return Modules.ToList();
    }

    public virtual IEnumerable<ModuleInfo> GetModules(int? parentId = null)
    {
        int[] ids = GetAuthority(userService.CurrentUser);
        return modules.Where(m => m.ParentModuleId == parentId && ids.Contains(m.ModuleId)).ToArray();
    }

    public virtual bool Verify(ModuleInfo module)
    {
        if (module == null)
        {
            return false;
        }

        UserAuthority authority = UserAuthority.Operator;
        if (userService.CurrentUser != null)
        {
            authority = userService.CurrentUser.Authority;
        }

        return module.Authority <= authority;
    }

    public virtual bool Verify(int moduleId)
    {
        UserAuthority authority = UserAuthority.Operator;
        if (userService.CurrentUser != null)
        {
            authority = userService.CurrentUser.Authority;
        }

        return modules.Any(m => m.ModuleId == moduleId && m.Authority <= authority);
    }

    protected virtual int[] GetAuthority(UserInfo? user)
    {
        UserAuthority authority = user?.Authority ?? UserAuthority.Operator;

        // 根据用户权限类型返回对应的模块ID列表
        return modules.Where(m => m.Authority <= authority)
                      .Select(m => m.ModuleId)
                      .ToArray();
    }

    public abstract Task<bool> InitializeAsync();
}
