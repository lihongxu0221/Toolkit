namespace BgCommon.Prism.Wpf.Authority.Modules.User.Views;

/// <summary>
/// UserLoginView.xaml 的交互逻辑
/// </summary>
public partial class UserLoginView : UserControl
{
    public UserLoginView()
    {
        InitializeComponent();

        // 订阅主Grid的SizeChanged事件
        this.MainGrid.SizeChanged += MainGrid_SizeChanged;
    }

    private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var background = this.StarrySkyBackground;
        if (background != null)
        {
            // 直接设置Width和Height
            background.Initialize((int)e.NewSize.Width, (int)e.NewSize.Height);

            // 我们可以打印尺寸来确认
            System.Diagnostics.Debug.WriteLine($"Grid Size: {e.NewSize.Width}x{e.NewSize.Height}");
            System.Diagnostics.Debug.WriteLine($"StarrySky Size: {background.ActualWidth}x{background.ActualHeight}");
        }
    }
}