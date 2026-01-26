using BgCommon.Prism.Wpf.Modules.Logging;
using BgCommon.Prism.Wpf.MVVM;
using BgLogger;
using ToolkitDemo.Views;

namespace ToolkitDemo.ViewModels;

/// <summary>
/// MainWindowViewModel.cs .
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(IContainerExtension container)
        : base(container)
    {
    }

    protected override Task OnLoadedAsync(object? parameter)
    {
        this.RequestNavigate<MainView>(RegionDefine.MainRegion);
        return base.OnLoadedAsync(parameter);
    }

    private void OnLoaded()
    {
        var sw = new Stopwatch();
        sw.Start();
        LogRun.Info("1111");
        LogDialog.Info("1111");
        LogMES.Info("1111");
        LogVision.Info("1111");
        LogMotion.Info("1111");
        Trace.WriteLine($"LogMotion.Info cost {sw.ElapsedMilliseconds}ms");
        sw.Stop();
    }

    [RelayCommand]
    private void OnTest()
    {
        var sw = new Stopwatch();
        sw.Start();
        LogRun.Info("1111");
        LogDialog.Info("1111");
        LogMES.Info("1111");
        LogVision.Info("1111");
        LogMotion.Info("1111");
        Trace.WriteLine($"LogMotion.Info cost {sw.ElapsedMilliseconds}ms");
        sw.Stop();
    }

    [RelayCommand]
    private async void OnClick()
    {
        _ = await BgLoggerFactory.ShowViewAsync();
    }
}