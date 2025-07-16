using System.Windows.Media.Animation;

namespace BgCommon.Prism.Wpf.Common.Views;

/// <summary>
/// SplashScreenView.xaml 的交互逻辑
/// </summary>
public partial class SplashScreenView : Window
{
    public SplashScreenView()
    {
        this.InitializeComponent();

        this.Opacity = 0;

        // 添加淡入动画
        DoubleAnimation fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(800),
        };

        this.BeginAnimation(OpacityProperty, fadeIn);
    }

    /// <summary>
    /// 以淡出动画异步关闭窗口.
    /// </summary>
    /// <returns>表示异步操作的 <see cref="Task"/>.</returns>
    public Task CloseAsync()
    {
        // 创建淡出动画
        var fadeOut = new DoubleAnimation
        {
            From = 1, // 起始不透明度
            To = 0,   // 结束不透明度
            Duration = TimeSpan.FromMilliseconds(600), // 动画持续时间
        };

        // 用于等待动画完成的 TaskCompletionSource
        var tcs = new TaskCompletionSource<bool>();
        fadeOut.Completed += (s, e) =>
        {
            this.Close(); // 动画完成后关闭窗口
            tcs.SetResult(true); // 设置任务完成
        };

        // 开始淡出动画
        this.BeginAnimation(OpacityProperty, fadeOut);
        return tcs.Task;
    }
}