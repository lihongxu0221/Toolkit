using BgCommon.Prism.Wpf.MVVM;
using BgCommon.Prism.Wpf.Services;

namespace BgCommon.Prism.Wpf.Common.ViewModels;

public class SplashScreenViewModel : ViewModelBase
{
    private readonly IGlobalVarService globalVarService;
    private ImageSource? imageSource = null;
    private double width = 800;
    private double height = 600;
    private double progress = 0.0d;
    private string title = string.Empty;
    private string statusMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreenViewModel"/> class.
    /// </summary>
    /// <param name="container">Ioc容器实例.</param>
    /// <param name="globalVarService">全局变量服务.</param>
    public SplashScreenViewModel(
        IContainerExtension container,
        IGlobalVarService globalVarService)
        : base(container)
    {
        this.globalVarService = globalVarService;
    }

    public ImageSource? SplashImage
    {
        get => imageSource;
        set => _ = SetProperty(ref imageSource, value);
    }

    public double Width
    {
        get => width;
        set => _ = SetProperty(ref width, value);
    }

    public double Height
    {
        get => height;
        set => _ = SetProperty(ref height, value);
    }

    public double Progress
    {
        get => progress;
        set => _ = SetProperty(ref progress, value);
    }

    public string Title
    {
        get => title;
        set => _ = SetProperty(ref title, value);
    }

    public string StatusMessage
    {
        get => statusMessage;
        set => _ = SetProperty(ref statusMessage, value);
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        _ = this.Subscribe<(double, string, string)>(OnShowSplash);
    }

    protected override void OnDeactivated()
    {
        _ = this.Subscribe<(double, string, string)>(OnShowSplash);
    }

    protected override Task OnLoadedAsync(object? parameter)
    {
        _ = Application.Current.Dispatcher.InvokeAsync(new Action(() =>
        {
            string? splashImagePath = globalVarService.GetSplashScreenImage();
            if (string.IsNullOrEmpty(splashImagePath) || !System.IO.File.Exists(splashImagePath))
            {
                splashImagePath = "pack://application:,,,/BgCommon.Prism.Wpf;component/Assets/Images/Splash.png";
            }

            this.SplashImage = new BitmapImage(new Uri(splashImagePath, UriKind.Absolute));
            this.Width = this.SplashImage.Width;
            this.Height = this.SplashImage.Height;
        }));

        return base.OnLoadedAsync(parameter);
    }

    /// <summary>
    /// 响应闪窗显示事件，更新进度、标题和状态信息.
    /// </summary>
    /// <param name="parameters">包含进度、标题和状态信息的元组.</param>
    private void OnShowSplash((double, string, string) parameters)
    {
        _ = Application.Current.Dispatcher.InvokeAsync(() =>
         {
             Progress = parameters.Item1;
             Title = parameters.Item2;
             StatusMessage = parameters.Item3;
         });
    }
}