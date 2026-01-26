namespace BgCommon.Prism.Wpf.Modules.Common.ViewModels;

/// <summary>
/// SplashScreenViewModel.cs .
/// </summary>
public partial class SplashScreenViewModel : NavigableDialogViewModel
{
    private readonly IGlobalVarService globalVarService;

    [ObservableProperty]
    private ImageSource? splashImage = null;

    [ObservableProperty]
    private double width = 800;

    [ObservableProperty]
    private double height = 600;

    [ObservableProperty]
    private double progress = 0.0d;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
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
        _ = this.EventAggregator?.GetEvent<PubSubEvent<(double, string, string)>>().Subscribe(OnShowSplash, ThreadOption.PublisherThread);
    }

    /// <inheritdoc/>
    protected override Task OnUnloadAsync(object? parameter)
    {
        this.EventAggregator?.GetEvent<PubSubEvent<(double, string, string)>>().Unsubscribe(OnShowSplash);
        return base.OnUnloadAsync(parameter);
    }

    /// <inheritdoc/>
    protected override async Task OnLoadedAsync(object? parameter)
    {
        bool flag = true;

        string? splashImagePath = globalVarService.GetSplashScreenImage();
        if (string.IsNullOrEmpty(splashImagePath))
        {
            splashImagePath = "pack://application:,,,/BgCommon.Prism.Wpf;component/Assets/Images/Splash.png";
            flag = false;
        }

        try
        {
            if (!System.IO.File.Exists(splashImagePath))
            {
                splashImagePath = "pack://application:,,,/BgCommon.Prism.Wpf;component/Assets/Images/Splash.png";
                flag = false;
            }
        }
        catch (Exception ex)
        {
            LogRun.Error(ex, "splashImagePath is error, please check it");
            splashImagePath = "pack://application:,,,/BgCommon.Prism.Wpf;component/Assets/Images/Splash.png";
            flag = false;
        }

        if (flag)
        {
            using (FileStream fs = new FileStream(splashImagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] buffer = br.ReadBytes((int)fs.Length);
                    MemoryStream ms = new MemoryStream();
                    await ms.WriteAsync(buffer, CancellationToken.None);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.CacheOption = BitmapCacheOption.Default;

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    this.SplashImage = bitmapImage;
                    this.Width = this.SplashImage.Width;
                    this.Height = this.SplashImage.Height;
                }
            }
        }
        else
        {
            this.SplashImage = new BitmapImage(new Uri(splashImagePath, UriKind.Absolute));
            this.SplashImage.Freeze();
            this.Width = this.SplashImage.Width;
            this.Height = this.SplashImage.Height;
        }

        await base.OnLoadedAsync(parameter);
    }

    /// <summary>
    /// 响应闪窗显示事件，更新进度、标题和状态信息.
    /// </summary>
    /// <param name="parameters">包含进度、标题和状态信息的元组.</param>
    private void OnShowSplash((double, string, string) parameters)
    {
        Progress = parameters.Item1;
        Title = parameters.Item2;
        StatusMessage = parameters.Item3;
    }
}