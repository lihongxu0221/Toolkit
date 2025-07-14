namespace BgCommon.Wpf.Prism.MVVM;

/// <summary>
/// NavigationViewModelBase.cs <br/>
/// <code>触发时机    导航过程中（前/后)</code>
/// <code>主要用途    数据初始化、状态保存</code>
/// <code>关键方法    OnNavigatedTo、OnNavigatedFrom</code>
/// <code>是否可取消导航 否（只能在 OnNavigatedFrom 中提示</code>
/// </summary>
public abstract partial class NavigationViewModelBase : ViewModelBase, INavigationAware, IDestructible
{
    protected NavigationViewModelBase(IContainerExtension container)
        : base(container)
    {
    }

    /// <summary>
    /// <code>触发时机   视图 / ViewModel 被完全销毁时（从区域移除后）</code>
    /// <code>主要用途  释放资源、取消订阅</code>
    /// <code>是否可恢复 否（对象已从区域移除）</code>
    /// <code>执行次数  每次销毁时仅执行一次</code>
    /// 1. Prism 会自动检测 IDestructible 实现并调用 Destroy 方法，但需确保对象通过 Prism 的区域管理器（RegionManager）管理。<br/>
    /// 2. 在 Destroy 方法中使用 null 检查或 Dispose 模式，防止资源被多次释放。<br/>
    /// 3. IDestructible 是 Prism 特有的接口，专注于导航生命周期中的资源管理。<br/>
    /// 4. 若需要兼容标准的 using 语句或依赖注入容器的释放机制，可同时实现 IDisposable。<br/>
    /// 5. 若 ViewModel 是单例（Singleton），Destroy 可能不会被调用（因实例不会被销毁）。<br/>
    /// 6. 对于单例服务，建议在应用关闭时手动清理资源。<br/>
    /// </summary>
    public virtual void Destroy()
    {
    }

    /// <summary>
    /// 决定是否重用现有实例
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual bool IsNavigationTarget(NavigationContext context)
    {
        return true;
    }

    /// <summary>
    /// 导航离开当前视图前执行
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual void OnNavigatedFrom(NavigationContext context)
    {
        context.Parameters.Add(GetType().Name, this);
    }

    /// <summary>
    /// 导航到当前视图后执行
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual void OnNavigatedTo(NavigationContext context)
    {
        context.Parameters.Add(GetType().Name, this);
    }
}