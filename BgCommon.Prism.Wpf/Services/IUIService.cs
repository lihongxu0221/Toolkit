namespace BgCommon.Prism.Wpf.Services;

/// <summary>
/// 提供在UI线程上执行操作的能力.
/// 这是一个抽象接口，使其在非UI环境中易于测试和模拟.
/// </summary>
public interface IUIService
{
    /// <summary>
    /// 同步在UI线程上执行一个委托.
    /// </summary>
    /// <param name="action">要在UI线程上执行的操作.</param>
    /// <param name="priority">在UI线程上执行的优先级.</param>
    void RunOnUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Background);

    /// <summary>
    /// 异步地将一个操作派发到UI线程队列中，并立即返回，不等待其执行。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    /// <param name="priority">在UI线程上执行的优先级.</param>
    void BeginRunOnUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Background);

    /// <summary>
    /// 异步地在UI线程上执行一个委托.
    /// </summary>
    /// <param name="action">要在UI线程上执行的操作.</param>
    /// <param name="priority">在UI线程上执行的优先级.</param>
    /// <returns>一个表示操作排队和执行的任务.</returns>
    Task RunOnUIThreadAsync(Action action, DispatcherPriority priority = DispatcherPriority.Background);
}