namespace BgCommon.Prism.Wpf.Services;

/// <summary>
/// IUIService 的 WPF 平台实现.
/// 使用 WPF Dispatcher 来封送调用.
/// </summary>
internal class WpfUIService : IUIService
{
    private readonly Dispatcher dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfUIService"/> class.
    /// </summary>
    public WpfUIService()
    {
        // 在构造时捕获UI线程的Dispatcher
        // 确保这个服务是在UI线程上创建的（通常通过Prism的DI容器在应用启动时注册为单例）
        dispatcher = Application.Current?.Dispatcher
             ?? throw new InvalidOperationException("Cannot create WpfUIService outside of a WPF application with a valid dispatcher.");
    }

    public void RunOnUIThread(Action action, DispatcherPriority priority)
    {
        if (this.dispatcher.CheckAccess())
        {
            // 如果已经在UI线程上，直接执行.
            action();
        }
        else
        {
            // 如果在后台线程，则异步封送到UI线程.
            this.dispatcher.Invoke(action, priority);
        }
    }

    /// <inheritdoc />
    public void BeginRunOnUIThread(Action action, DispatcherPriority priority)
    {
        // 从后台线程异步派发
        _ = dispatcher.BeginInvoke(action, priority);
    }

    /// <inheritdoc/>
    public Task RunOnUIThreadAsync(Action action, DispatcherPriority priority)
    {
        if (this.dispatcher.CheckAccess())
        {
            // 如果已经在UI线程上，直接执行.
            action();
            return Task.CompletedTask;
        }
        else
        {
            // 如果在后台线程，则异步封送到UI线程.
            return this.dispatcher.InvokeAsync(action, priority).Task;
        }
    }
}
