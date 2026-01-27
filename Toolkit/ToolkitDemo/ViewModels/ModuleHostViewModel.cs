using BgCommon.Prism.Wpf.MVVM;namespace ToolkitDemo.ViewModels;public class ModuleHostViewModel : NavigationHostViewModelBase{    public ModuleHostViewModel(IContainerExtension container)        : base(container, RegionDefine.MainContentRegion)    {    }
    protected override void OnDispose()
    {        _ = App.Current.Dispatcher.InvokeAsync(() =>        {            this.Views.Clear();        });
    }}