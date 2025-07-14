using BgCommon.Wpf.Prism.MVVM;

namespace BgCommon.Wpf.Prism;

/// <summary>
/// ViewModelBase.DialogService 扩展方法.
/// </summary>
public static partial class ViewModelBaseExtensions
{
    #region Dialog Service Extensions

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show<TView>(this ViewModelBase vm, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.Show<TView>(callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show<TView>(this ViewModelBase vm, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.Show<TView>(windowName, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show(this ViewModelBase vm, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.Show(viewName, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show(this ViewModelBase vm, string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.Show(viewName, windowName, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog<TView>(this ViewModelBase vm, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.ShowDialog<TView>(callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog<TView>(this ViewModelBase vm, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.ShowDialog<TView>(windowName, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog(this ViewModelBase vm, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.ShowDialog(viewName, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog(this ViewModelBase vm, string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        vm.DialogService.ShowDialog(viewName, windowName, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowAsync<TView>(this ViewModelBase vm, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowAsync<TView>(config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowAsync<TView>(this ViewModelBase vm, string windowName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowAsync<TView>(windowName, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowAsync(this ViewModelBase vm, string viewName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowAsync(viewName, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowAsync(this ViewModelBase vm, string viewName, string windowName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowAsync(viewName, windowName, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowDialogAsync<TView>(this ViewModelBase vm, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowDialogAsync<TView>(config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型.</typeparam>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowDialogAsync<TView>(this ViewModelBase vm, string windowName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowDialogAsync(windowName, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowDialogAsync(this ViewModelBase vm, string viewName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowDialogAsync(viewName, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="vm">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static Task<IDialogResult?> ShowDialogAsync(this ViewModelBase vm, string viewName, string windowName, Action<IDialogParameters>? config = null)
    {
        ArgumentNullException.ThrowIfNull(vm.DialogService, nameof(vm.DialogService));
        return vm.DialogService.ShowDialogAsync(viewName, windowName, config);
    }
    #endregion

    #region Event Aggregator Extensions

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 发布一个基于载荷类型的事件.
    /// 订阅者通过订阅相同的载荷类型 TPayload 来接收消息.
    /// </summary>
    /// <typeparam name="TPayload">要传递的消息载荷的数据类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="payload">要发布的消息实例.</param>
    public static void Publish<TPayload>(this ViewModelBase vm, TPayload payload)
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        vm.EventAggregator.GetEvent<PubSubEvent<TPayload>>().Publish(payload);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 订阅一个基于载荷类型的事件.
    /// </summary>
    /// <typeparam name="TPayload">要订阅的消息载荷的数据类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="action">当事件发布时执行的回调委托.</param>
    /// <returns>一个唯一的订阅令牌，可用于取消订阅.</returns>
    public static SubscriptionToken? Subscribe<TPayload>(this ViewModelBase vm, Action<TPayload>? action)
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        return vm.EventAggregator.GetEvent<PubSubEvent<TPayload>>().Subscribe(action);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 取消订阅一个基于载荷类型的事件.
    /// </summary>
    /// <typeparam name="TPayload">要取消订阅的消息载荷的数据类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="subscriber">订阅时使用的回调委托.</param>
    public static void Unsubscribe<TPayload>(this ViewModelBase vm, Action<TPayload>? subscriber)
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        vm.EventAggregator.GetEvent<PubSubEvent<TPayload>>().Unsubscribe(subscriber);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 以异步方式发布基于载荷类型的事件.
    /// 注意：此方法是同步执行的，但返回一个 Task.CompletedTask 以便在 async 方法中流畅调用.
    /// 事件的发布和订阅者的处理仍在调用线程上进行.
    /// </summary>
    /// <typeparam name="TPayload">要传递的消息载荷的数据类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="payload">要发布的消息实例.</param>
    public static Task PublishAsync<TPayload>(this ViewModelBase vm, TPayload payload)
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        vm.EventAggregator.GetEvent<PubSubEvent<TPayload>>().Publish(payload);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 发布一个自定义事件.
    /// 事件类型 TEvent 本身就是事件的标识.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="payload">要发布的事件实例（即消息载荷）.</param>
    public static void PublishEx<TEvent>(this ViewModelBase vm, TEvent payload)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));

        // 注意：这里GetEvent的泛型参数是TEvent自身，payload也是TEvent类型
        // 这是为了支持像 MyCustomEvent.Publish(new MyCustomEvent{...}) 这样的调用
        // 但Prism的本意是 TEvent : PubSubEvent<TPayload>
        // 为了保持与您原始代码一致，这里保留了 TEvent : PubSubEvent<TEvent> 的约束
        vm.EventAggregator.GetEvent<TEvent>().Publish(payload);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 订阅一个自定义事件.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="action">当事件发布时执行的回调委托.</param>
    /// <returns>一个唯一的订阅令牌，可用于取消订阅.</returns>
    public static SubscriptionToken? SubscribeEx<TEvent>(this ViewModelBase vm, Action<TEvent>? action)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        return vm.EventAggregator.GetEvent<TEvent>().Subscribe(action);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 取消订阅一个自定义事件.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="subscriber">订阅时使用的回调委托.</param>
    public static void UnsubscribeEx<TEvent>(this ViewModelBase vm, Action<TEvent>? subscriber)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        vm.EventAggregator.GetEvent<TEvent>().Unsubscribe(subscriber);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 以异步方式发布自定义事件.
    /// 注意：此方法是同步执行的，但返回一个 Task.CompletedTask 以便在 async 方法中流畅调用.
    /// 事件的发布和订阅者的处理仍在调用线程上进行.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="payload">要发布的事件实例.</param>
    public static Task PublishAsyncEx<TEvent>(this ViewModelBase vm, TEvent payload)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(vm.EventAggregator, nameof(vm.EventAggregator));
        vm.EventAggregator.GetEvent<TEvent>().Publish(payload);
        return Task.CompletedTask;
    }

    #endregion

    #region Region Manager Extensions

    /// <summary>
    /// 通过视图类型导航到指定区域.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigate<TView>(this ViewModelBase vm, string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.RequestNavigate(regionName, typeof(TView).Name, configura, callBack);
    }

    /// <summary>
    /// 通过视图名称导航到指定区域，并可选择传递导航参数.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">要导航到的视图的名称（在DI容器中注册的名称）.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigate(this ViewModelBase vm, string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.RequestNavigate(regionName, viewName, configura, callBack);
    }

    /// <summary>
    /// 以异步方式通过视图类型导航到指定区域，并等待导航完成.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configura">要传递给目标视图的导航参数（可选）.</param>
    /// <returns>一个表示异步导航操作的任务，其结果是导航结果 <see cref="NavigationResult"/>.</returns>
    public static Task<NavigationResult> RequestNavigateAsync<TView>(this ViewModelBase vm, string regionName, Action<INavigationParameters>? configura = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        return vm.RegionManager.RequestNavigateAsync<TView>(regionName, configura);
    }

    /// <summary>
    /// 以异步方式导航到指定区域，并等待导航完成.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">要导航到的视图的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <returns>一个表示异步导航操作的任务，其结果是导航结果 <see cref="NavigationResult"/>.</returns>
    public static Task<NavigationResult> RequestNavigateAsync(this ViewModelBase vm, string regionName, string viewName, Action<INavigationParameters>? configura = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        return vm.RegionManager.RequestNavigateAsync(regionName, viewName, configura);
    }

    /// <summary>
    /// 导航到指定视图.如果该视图类型的实例已存在于区域中，则激活它；否则，创建新实例并导航.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigateAndActivate<TView>(this ViewModelBase vm, string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.RequestNavigateAndActivate<TView>(regionName, configura, callBack);
    }

    /// <summary>
    /// 导航到指定视图.如果该视图类型的实例已存在于区域中，则激活它；否则，创建新实例并导航.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">视图的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigateAndActivate(this ViewModelBase vm, string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.RequestNavigateAndActivate(regionName, viewName, configura, callBack);
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    public static void ClearRegion(this ViewModelBase vm, string regionName)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.ClearRegion(regionName);
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    public static void ClearRegionView<TView>(this ViewModelBase vm, string regionName)
        where TView : class
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.ClearRegionView<TView>(regionName);
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    /// <param name="viewName">要清空的视图的名称.</param>
    public static void ClearRegionView(this ViewModelBase vm, string regionName, string viewName)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        vm.RegionManager.ClearRegionView(regionName, viewName);
    }

    /// <summary>
    /// 获取指定区域内的所有视图的集合.
    /// </summary>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="regionName">区域的名称.</param>
    /// <returns>一个包含区域内所有视图对象的只读集合，如果区域不存在则返回空集合.</returns>
    public static IEnumerable<object> GetViews(this ViewModelBase vm, string regionName)
    {
        ArgumentNullException.ThrowIfNull(vm.RegionManager);
        return vm.RegionManager.GetViews(regionName);
    }

    #endregion

    #region Container Extensions

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="service">service.</param>
    /// <returns>returns.</returns>
    public static bool TryResolve<TIServiceType>(this ViewModelBase vm, out TIServiceType? service)
        where TIServiceType : class
    {
        service = default; // 默认初始化 out 参数
        ArgumentNullException.ThrowIfNull(vm.Container);

        if (vm.Container.IsRegistered(typeof(TIServiceType)))
        {
            service = vm.Container.Resolve<TIServiceType>();
            return true;
        }

        return false;
    }

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="vm">视图模型实例.</param>
    /// <param name="serviceName">注入服务的名称.</param>
    /// <param name="service">输出的服务实例.</param>
    /// <returns>returns.</returns>
    public static bool TryResolve<TIServiceType>(this ViewModelBase vm, string serviceName, out TIServiceType? service)
        where TIServiceType : class
    {
        service = default; // 默认初始化 out 参数
        ArgumentNullException.ThrowIfNull(vm.Container);

        if (vm.Container.IsRegistered(typeof(TIServiceType), serviceName))
        {
            service = vm.Container.Resolve<TIServiceType>(serviceName);
            return true;
        }

        return false;
    }

    #endregion
}
