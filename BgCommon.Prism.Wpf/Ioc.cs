using BgCommon;
using BgCommon.Prism.Wpf.Controls.Services;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// 提供一组全局可用的、用于与 Prism 核心服务交互的静态快捷方法。
/// 这是应用程序业务逻辑与 Prism 框架交互的最顶层、最简化的 API。
/// 所有方法都通过底层的 Ioc 类来访问已初始化的核心服务。
/// </summary>
public static class Ioc
{
    private static IContainerExtension? _container;

    public static IContainerExtension? Container => _container;

    public static IDialogService? DialogService { get; set; }

    public static IEventAggregator? EventAggregator { get; set; }

    public static IRegionManager? RegionManager { get; set; }

    /// <summary>
    /// 初始化静态服务定位器。此方法应在应用程序启动时调用一次。
    /// </summary>
    /// <param name="container">Prism 的依赖注入容器实例。</param>
    public static void Initialize(IContainerExtension container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));

        // --- 在初始化时，手动解析并赋值 ---
        DialogService = _container.Resolve<IDialogService>();
        EventAggregator = _container.Resolve<IEventAggregator>();
        RegionManager = _container.Resolve<IRegionManager>();
    }

    /// <summary>
    /// 解析指定类型的服务实例。
    /// 推荐优先使用便捷的静态属性，仅在需要解析不常用服务时使用此方法。
    /// </summary>
    /// <typeparam name="T">要解析的服务类型。</typeparam>
    /// <returns>解析出的服务实例。</returns>
    public static T Resolve<T>()
    {
        if (_container is null)
        {
            throw new InvalidOperationException("IoC container has not been initialized.");
        }

        return _container.Resolve<T>();
    }

    // 也可以提供一个带名称的解析方法
    public static T Resolve<T>(string name)
    {
        if (_container is null)
        {
            throw new InvalidOperationException("IoC container has not been initialized.");
        }

        return _container.Resolve<T>(name);
    }

    #region Dialog Service API (对话框服务)

    /// <summary>
    /// 显示一个非模态对话框。
    /// </summary>
    /// <typeparam name="TView">对话框的视图类型。</typeparam>
    public static void Show<TView>(Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.Show<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 使用自定义窗口名称，显示一个非模态对话框。
    /// </summary>
    public static void Show<TView>(string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.Show<TView>(windowName, callback, config);
    }

    public static void Show(string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.Show(viewName, string.Empty, callback, config);
    }

    public static void Show(string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.Show(viewName, windowName, callback, config);
    }

    /// <summary>
    /// 显示一个模态对话框。
    /// </summary>
    public static void ShowDialog<TView>(Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.ShowDialog<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 使用自定义窗口名称，显示一个模态对话框。
    /// </summary>
    public static void ShowDialog<TView>(string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.ShowDialog<TView>(windowName, callback, config);
    }

    public static void ShowDialog(string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.ShowDialog(viewName, string.Empty, callback, config);
    }

    public static void ShowDialog(string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        DialogService.ShowDialog(viewName, windowName, callback, config);
    }

    /// <summary>
    /// 异步显示一个非模态对话框，并等待其关闭。
    /// </summary>
    public static Task<IDialogResult?> ShowAsync<TView>(Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ShowAsync<TView>(config);
    }

    public static Task<IDialogResult?> ShowAsync<TView>(string windowName, Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ShowAsync<TView>(windowName, config);
    }

    /// <summary>
    /// 异步显示一个模态对话框，并等待其关闭。
    /// </summary>
    public static Task<IDialogResult?> ShowDialogAsync<TView>(Action<IDialogParameters>? config = null)
        where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ShowDialogAsync<TView>(config);
    }

    public static Task<IDialogResult?> ShowDialogAsync(string viewName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ShowDialogAsync(viewName, config);
    }

    public static Task<IDialogResult?> ShowDialogAsync(string viewName, string windowName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ShowDialogAsync(viewName, windowName, config);
    }

    #endregion

    #region Region Manager API (区域导航服务)

    /// <summary>
    /// 通过视图类型导航到指定区域。
    /// </summary>
    public static void Navigate<TView>(string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.RequestNavigate<TView>(regionName, configura, callBack);
    }

    /// <summary>
    /// 通过视图名称导航到指定区域。
    /// </summary>
    public static void Navigate(string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.RequestNavigate(regionName, viewName, configura, callBack);
    }

    /// <summary>
    /// 异步导航到指定区域，并等待导航完成。
    /// </summary>
    public static Task<NavigationResult> NavigateAsync<TView>(string regionName, Action<INavigationParameters>? configura = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        return RegionManager.RequestNavigateAsync<TView>(regionName, configura);
    }

    public static Task<NavigationResult> NavigateAsync(string regionName, string viewName, Action<INavigationParameters>? configura = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        return RegionManager.RequestNavigateAsync(regionName, viewName, configura);
    }

    /// <summary>
    /// 导航到指定视图。如果视图已存在则激活它，否则创建新实例。
    /// </summary>
    public static void NavigateAndActivate<TView>(string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.RequestNavigateAndActivate<TView>(regionName, configura, callBack);
    }

    public static void NavigateAndActivate(string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.RequestNavigateAndActivate(regionName, viewName, configura, callBack);
    }

    /// <summary>
    /// 清空指定区域内的所有视图。
    /// </summary>
    public static void ClearRegion(string regionName)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.ClearRegion(regionName);
    }

    /// <summary>
    /// 从指定区域中移除特定类型的所有视图实例。
    /// </summary>
    public static void ClearRegionView<TView>(string regionName)
        where TView : class
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        RegionManager.ClearRegionView<TView>(regionName);
    }

    /// <summary>
    /// 获取指定区域内的所有视图。
    /// </summary>
    public static IEnumerable<object> GetViews(string regionName)
    {
        ArgumentNullException.ThrowIfNull(RegionManager, nameof(RegionManager));
        return RegionManager.GetViews(regionName);
    }

    #endregion

    #region EventAggregator API (事件聚合服务)

    /// <summary>
    /// 发布一个基于载荷（Payload）的事件。
    /// </summary>
    public static void Publish<TPayload>(TPayload payload)
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));

        // 假设我们已经为 IEventAggregator 创建了扩展方法
        EventAggregator.Publish(payload);
    }

    /// <summary>
    /// 发布一个自定义事件类。
    /// </summary>
    public static void PublishEx<TEvent>(TEvent payload)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));
        EventAggregator.PublishEx(payload);
    }

    /// <summary>
    /// 订阅一个基于载荷（Payload）的事件。
    /// </summary>
    public static SubscriptionToken? Subscribe<TPayload>(Action<TPayload> action)
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));
        return EventAggregator.Subscribe(action);
    }

    /// <summary>
    /// 订阅一个自定义事件类。
    /// </summary>
    public static SubscriptionToken? SubscribeEx<TEvent>(Action<TEvent> action)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));
        return EventAggregator.SubscribeEx(action);
    }

    public static void Unsubscribe<TPayload>(Action<TPayload> subscriber)
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));
        EventAggregator.Unsubscribe(subscriber);
    }

    public static void UnsubscribeEx<TEvent>(Action<TEvent> subscriber)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(EventAggregator, nameof(EventAggregator));
        EventAggregator.UnsubscribeEx(subscriber);
    }

    #endregion

    #region Container Ioc容器相关服务方法.

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="service">service.</param>
    /// <returns>returns.</returns>
    public static bool TryResolve<TIServiceType>(out TIServiceType? service)
        where TIServiceType : class
    {
        service = default; // 默认初始化 out 参数

        if (_container is null)
        {
            throw new InvalidOperationException("IoC container has not been initialized.");
        }

        if (_container.IsRegistered(typeof(TIServiceType)))
        {
            service = _container.Resolve<TIServiceType>();
            return true;
        }

        return false;
    }

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="serviceName">注入服务的名称.</param>
    /// <param name="service">输出的服务实例.</param>
    /// <returns>returns.</returns>
    public static bool TryResolve<TIServiceType>(string serviceName, out TIServiceType? service)
        where TIServiceType : class
    {
        service = default; // 默认初始化 out 参数

        if (_container is null)
        {
            throw new InvalidOperationException("IoC container has not been initialized.");
        }

        if (_container.IsRegistered(typeof(TIServiceType), serviceName))
        {
            service = _container.Resolve<TIServiceType>(serviceName);
            return true;
        }

        return false;
    }
    #endregion

    #region 多语言

    /// <summary>
    /// 获取多语言字符串资源。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    public static string GetString(string key, params object[] args)
    {
        Assembly? assembly = Assembly.GetCallingAssembly();
        return GetString(assembly?.GetName()?.Name, key, args);
    }

    /// <summary>
    /// 获取多语言字符串资源
    /// </summary>
    /// <param name="assemblyName">资源包所在程序集名称</param>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    public static string GetString(string? assemblyName, string key, params object[] args)
    {
        return Lang.GetString(assemblyName, key, args);
    }

    #endregion

    #region 自定义消息弹窗

    public static IDialogResult? Error(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.Error(content, title);
    }

    public static IDialogResult? Warn(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.Warn(content, title);
    }

    public static IDialogResult? Question(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.Question(content, title);
    }

    public static IDialogResult? Info(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.Info(content, title);
    }

    public static Task<IDialogResult?> ErrorAsync(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.ErrorAsync(content, title);
    }

    public static Task<IDialogResult?> WarnAsync(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.WarnAsync(content, title);
    }

    public static Task<IDialogResult?> QuestionAsync(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.QuestionAsync(content, title);
    }

    public static Task<IDialogResult?> InfoAsync(string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return DialogService.InfoAsync(content, title);
    }
    #endregion

    #region 自定义输入弹窗

    /// <summary>
    /// 创建一个流畅的输入对话框构建器。
    /// </summary>
    /// <param name="vm">Prism 对话框服务。</param>
    /// <param name="title">对话框的初始标题。</param>
    /// <returns>一个新的 InputDialogBuilder 实例。</returns>
    public static InputDialogBuilder CreateInputDialog(string title)
    {
        ArgumentNullException.ThrowIfNull(DialogService, nameof(DialogService));
        return new InputDialogBuilder(DialogService).WithTitle(title);
    }

    #endregion
}