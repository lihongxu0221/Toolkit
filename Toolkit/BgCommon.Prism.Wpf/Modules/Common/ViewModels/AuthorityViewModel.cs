namespace BgCommon.Prism.Wpf.Modules.Common.ViewModels;

/// <summary>
/// 二级菜单列表通用模型.
/// </summary>
public partial class AuthorityViewModel : NavigationHostViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorityViewModel"/> class.
    /// </summary>
    public AuthorityViewModel(IContainerExtension container)
        : base(container)
    {
        this.RegionName = this.GetType().Name;
    }

    protected override async Task OnLoadedAsync(object? parameter)
    {
        NotifyViewAttached();
        await Task.Delay(0);
    }
}