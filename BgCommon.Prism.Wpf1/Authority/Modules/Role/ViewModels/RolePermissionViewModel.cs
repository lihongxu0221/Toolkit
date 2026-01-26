using BgCommon.Prism.Wpf.MVVM;

namespace BgCommon.Prism.Wpf.Authority.Modules.Role.ViewModels;

/// <summary>
/// 角色与权限管理.
/// </summary>
public partial class RolePermissionViewModel : NavigationViewModelBase
{
    public RolePermissionViewModel(IContainerExtension container)
        : base(container)
    {
    }
}