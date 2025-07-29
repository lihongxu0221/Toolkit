using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.MVVM;

namespace BgCommon.Prism.Wpf.Authority.Modules.Module.ViewModels;

/// <summary>
/// 模块管理.
/// </summary>
public partial class ModuleManagementViewModel : NavigationViewModelBase
{
    private readonly IAuthorityService authorityService;

    public ModuleManagementViewModel(IContainerExtension container)
        : base(container)
    {
        this.authorityService = this.Container.Resolve<IAuthorityService>();
    }
}