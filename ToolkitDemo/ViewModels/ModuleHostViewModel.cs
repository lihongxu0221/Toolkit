using BgCommon.Prism.Wpf;
using BgCommon.Prism.Wpf.MVVM;

namespace ToolkitDemo.ViewModels;

public class ModuleHostViewModel : NavigationHostViewModelBase, ICachedView
{
    public ModuleHostViewModel(IContainerExtension container)
        : base(container, RegionDefine.MainContentRegion, true)
    {
    }

    public override void Destroy()
    {
        _ = App.Current.Dispatcher.InvokeAsync(() =>
        {
            this.SetScopedRegionManager(null);
            this.Views.Clear();
        });
    }
}