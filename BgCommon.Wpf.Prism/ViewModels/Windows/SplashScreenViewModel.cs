using BgCommon.Wpf.Prism.MVVM;

namespace BgCommon.Wpf.Prism.ViewModels;

public class SplashScreenViewModel : ViewModelBase
{
    private ImageSource? imageSource = null;
    private double width = 800;
    private double height = 600;
    private double progress = 0.0d;
    private string title = string.Empty;
    private string statusMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreenViewModel"/> class.
    /// </summary>
    /// <param name="container">container.</param>
    public SplashScreenViewModel(IContainerExtension container)
        : base(container)
    {
        _ = this.Subscribe<(double, string, string)>(this.OnShowSplash);
    }

    /// <summary>
    /// Gets or Sets 闪窗背景图.
    /// </summary>
    public ImageSource? SplashImage
    {
        get => this.imageSource;
        set => _ = this.SetProperty(ref this.imageSource, value);
    }

    /// <summary>
    /// Gets or Sets 宽度.
    /// </summary>
    public double Width
    {
        get => this.width;
        set => _ = this.SetProperty(ref this.width, value);
    }

    /// <summary>
    /// Gets or Sets 宽度.
    /// </summary>
    public double Height
    {
        get => this.height;
        set => _ = this.SetProperty(ref this.height, value);
    }

    /// <summary>
    /// Gets or Sets 进度百分比.
    /// </summary>
    public double Progress
    {
        get => this.progress;
        set => _ = this.SetProperty(ref this.progress, value);
    }

    /// <summary>
    /// Gets or Sets 标题.
    /// </summary>
    public string Title
    {
        get => this.title;
        set => _ = this.SetProperty(ref this.title, value);
    }

    /// <summary>
    /// Gets or Sets 内容.
    /// </summary>
    public string StatusMessage
    {
        get => this.statusMessage;
        set => _ = this.SetProperty(ref this.statusMessage, value);
    }

    /// <inheritdoc/>
    protected override Task OnLoadedAsync(object? parameter)
    {
        // // 预加载 Logo
        // if (File.Exists(FileNames.SplashImagePath))
        // {
        //     this.SplashImage = new BitmapImage(new Uri(FileNames.SplashImagePath, UriKind.Absolute));
        //     this.Width = this.SplashImage.Width;
        //     this.Height = this.SplashImage.Height;
        // }

        return base.OnLoadedAsync(parameter);
    }

    /// <summary>
    /// 响应闪窗显示事件，更新进度、标题和状态信息.
    /// </summary>
    /// <param name="parameters">包含进度、标题和状态信息的元组.</param>
    private void OnShowSplash((double, string, string) parameters)
    {
        this.Progress = parameters.Item1;
        this.Title = parameters.Item2;
        this.StatusMessage = parameters.Item3;
    }
}