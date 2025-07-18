namespace BgCommon.Prism.Wpf.MVVM;

/// <summary>
/// 为参与 Prism 区域导航的 ViewModel 提供生命周期支持的基类.
/// 实现了 INavigationAware, IConfirmNavigationRequest, 和 IDestructible 接口.
/// <code>触发时机    导航过程中（前/后)</code>
/// <code>主要用途    数据初始化、状态保存</code>
/// <code>关键方法    OnNavigatedTo、OnNavigatedFrom</code>
/// <code>是否可取消导航 否（只能在 OnNavigatedFrom 中提示</code>
/// </summary>
public abstract partial class NavigationViewModelBase : ViewModelBase, INavigationAware, IConfirmNavigationRequest, IDestructible, IRegionMemberLifetime
{

    protected NavigationViewModelBase(IContainerExtension container)
        : base(container)
    {
    }

    /// <summary>
    /// Gets a value indicating whether gets <br/>
    ///     true（推荐）：当视图变为非激活状态时，区域管理器会保留这个视图实例。下次再次导航到该视图时，不会创建新实例，而是直接重用现有的实例。这完美地解决了重复加载和内存增长的问题. <br/>
    ///     false：当视图变为非激活状态时，区域管理器会将其从 Views 集合中移除，并且如果没有其他地方引用它，垃圾回收器（GC）最终会回收它。这可以节省内存，但下次导航时需要重新创建实例. <br/>
    /// </summary>
    public virtual bool KeepAlive => true;

    /// <summary>
    /// 当导航完成并进入到此视图时调用.适合进行数据初始化和刷新.
    /// </summary>
    /// <param name="navigationContext">包含导航参数的上下文.</param>
    public virtual void OnNavigatedTo(NavigationContext navigationContext)
    {
    }

    /// <summary>
    /// 当导航即将离开此视图时调用.适合进行状态保存.
    /// 注意：此方法无法取消导航.如需取消，请使用 IConfirmNavigationRequest.
    /// </summary>
    /// <param name="navigationContext">包含导航参数的上下文.</param>
    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    /// <summary>
    /// 决定当再次导航到此视图时，是否重用当前的 ViewModel 实例.
    /// 默认返回 true，以优化性能.
    /// **注意：对于需要根据导航参数显示不同内容的视图（如详情页），必须重写此方法，
    /// 并比较当前上下文的ID与新导航上下文的ID，以决定是否创建新实例.**
    /// </summary>
    /// <param name="navigationContext">导航上下文.</param>
    /// <returns>如果重用实例，则为 true；否则为 false.</returns>
    public virtual bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return true;
    }

    /// <summary>
    /// （异步版本）进行实际导航之前进行一些确认操作。
    /// 返回一个代表确认结果的任务。
    /// </summary>
    /// <param name="navigationContext">导航上下文</param>
    /// <returns>任务的结果为 true 表示确认导航，false 表示取消导航。</returns>
    protected virtual Task<bool> OnConfirmNavigationRequestAsync(NavigationContext navigationContext)
    {
        // 对于简单的同步场景，直接返回一个已完成的任务
        return Task.FromResult(true);
    }

    /// <summary>
    /// 在导航开始前调用，用于确认是否允许离开当前视图.
    /// 这是实现“未保存提醒”并中止导航的最佳位置.
    /// **重要：必须调用 continuationCallback 以完成导航流程.**
    /// </summary>
    /// <param name="navigationContext">导航上下文.</param>
    /// <param name="continuationCallback">回调.传入 true 继续导航，传入 false 中止导航.</param>
    public virtual async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
        // 调用异步版本并等待结果
        bool canNavigate = await OnConfirmNavigationRequestAsync(navigationContext);

        // 默认行为：直接允许导航.
        continuationCallback(canNavigate);
    }

    /// <summary>
    /// 当 ViewModel 被销毁时（例如，从区域移除或 IsNavigationTarget 返回 false 后），此方法被调用.
    /// 这是释放资源、取消事件总线订阅等的最终清理场所，以防止内存泄漏.
    /// <code>触发时机   视图 / ViewModel 被完全销毁时（从区域移除后）</code>
    /// <code>主要用途  释放资源、取消订阅</code>
    /// <code>是否可恢复 否（对象已从区域移除）</code>
    /// <code>执行次数  每次销毁时仅执行一次</code>
    /// 1. Prism 会自动检测 IDestructible 实现并调用 Destroy 方法，但需确保对象通过 Prism 的区域管理器（RegionManager）管理.<br/>
    /// 2. 在 Destroy 方法中使用 null 检查或 Dispose 模式，防止资源被多次释放.<br/>
    /// 3. IDestructible 是 Prism 特有的接口，专注于导航生命周期中的资源管理.<br/>
    /// 4. 若需要兼容标准的 using 语句或依赖注入容器的释放机制，可同时实现 IDisposable.<br/>
    /// 5. 若 ViewModel 是单例（Singleton），Destroy 可能不会被调用（因实例不会被销毁）.<br/>
    /// 6. 对于单例服务，建议在应用关闭时手动清理资源.<br/>
    /// </summary>
    public virtual void Destroy()
    {
    }
}