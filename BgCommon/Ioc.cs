using BgCommon.Localization;
using BgCommon.Localization.DependencyInjection;

namespace BgCommon;

/// <summary>
/// Ioc容器帮助类
/// </summary>
public sealed class Ioc
{
    private static readonly object LockObj = new object();
    private static Ioc? instance;

    /// <summary>
    /// Gets Ioc容器帮助类实例
    /// </summary>
    public static Ioc Instance
    {
        get
        {
            lock (LockObj)
            {
                if (instance == null)
                {
                    if (Application.Current.MainWindow == null ||
                        DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
                    {
                        return new Ioc(null);
                    }
                    else
                    {
                        throw new InvalidProgramException("Ioc.Instanc is null, please call method IContainerRegistry.RegisterIocSingleton");
                    }
                }
            }

            return instance!;
        }
    }

    private readonly IContainerExtension container;
    private readonly IEventAggregator? eventAggregator;
    private readonly IRegionManager? regionManager;
    private readonly IDialogService? dialogService;

    /// <summary>
    /// Gets 容器实例.
    /// </summary>
    public IContainerExtension Container => container;

    /// <summary>
    /// Gets 事件聚合.
    /// </summary>
    public IEventAggregator? EventAggregator => this.eventAggregator;

    /// <summary>
    /// Gets 模块区域管理.
    /// </summary>
    public IRegionManager? RegionManager => this.regionManager;

    /// <summary>
    /// Gets 弹窗服务.
    /// </summary>
    public IDialogService? DialogService => this.dialogService;

    private Ioc(IContainerExtension? container)
    {
        if (container == null)
        {
            ArgumentNullException.ThrowIfNull(nameof(container));
            return;
        }

        this.container = container;
        if (this.container != null)
        {
            this.regionManager = this.container.Resolve<IRegionManager>();
            this.eventAggregator = this.container.Resolve<IEventAggregator>();
            this.dialogService = this.container.Resolve<IDialogService>();
            instance = this;

            // 0.注入多语言
            _ = this.container.AddStringLocalizer(b =>
            {
                // BgCommon
                b.FromResource<BgCommon.Assets.Localization.TranslationsEnum>(new CultureInfo("zh-CN"), true);
                b.FromResource<BgCommon.Assets.Localization.TranslationsEnum>(new CultureInfo("zh-TW"), true);
                b.FromResource<BgCommon.Assets.Localization.TranslationsEnum>(new CultureInfo("en-US"), true);
                b.FromResource<BgCommon.Assets.Localization.TranslationsEnum>(new CultureInfo("vi"), true);

                b.FromResource<BgCommon.Assets.Localization.Translations>(new CultureInfo("zh-CN"), true);
                b.FromResource<BgCommon.Assets.Localization.Translations>(new CultureInfo("zh-TW"), true);
                b.FromResource<BgCommon.Assets.Localization.Translations>(new CultureInfo("en-US"), true);
                b.FromResource<BgCommon.Assets.Localization.Translations>(new CultureInfo("vi"), true);
            });
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="container">IOC容器实例</param>
    public static void Intialize(IContainerExtension container)
    {
        lock (LockObj)
        {
            if (instance == null)
            {
                instance = new Ioc(container);
            }
        }
    }

    /// <summary>
    /// 从指定名称的区域中移除当前活动的View实例.
    /// </summary>
    /// <param name="regionName">区域名称.</param>
    /// <param name="viewName">视图名称.</param>
    public void RemoveView(string regionName, string viewName)
    {
        if (string.IsNullOrEmpty(regionName))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(nameof(regionName));
        }

        if (string.IsNullOrEmpty(viewName))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(nameof(viewName));
        }

        if (this.RegionManager != null)
        {
            if (this.RegionManager.Regions.ContainsRegionWithName(regionName))
            {
                IRegion region = this.RegionManager.Regions[regionName];
                object? view = region.ActiveViews.FirstOrDefault(t => t.GetType().Name == viewName);
                if (view != null)
                {
                    region.Remove(view);
                }
            }
            else
            {
                Trace.TraceWarning($"Region with the specified name {regionName} not found in RegionManager");
            }
        }
    }

    /// <summary>
    /// 向指定名称的区域中添加指定名称View实例.
    /// </summary>
    /// <typeparam name="TViewType">View类型.</typeparam>
    /// <param name="regionName">区域名称.</param>
    /// <param name="configura">配置传递的参数.</param>
    public void RequestNavigate<TViewType>(string regionName, Action<NavigationParameters>? configura = null)
    {
        this.RequestNavigate(regionName, typeof(TViewType).Name, configura);
    }

    /// <summary>
    /// 向指定名称的区域中添加指定名称View实例<br/>
    /// 导航完毕后，从区域中移除该View实例
    /// </summary>
    /// <param name="regionName">区域名称.</param>
    /// <param name="viewName">实例名称.</param>
    /// <param name="configura">配置参数回调.</param>
    /// <param name="callBack">导航回调.</param>
    public void RequestNavigate(string regionName, string viewName, Action<NavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        if (RegionManager != null)
        {
            NavigationParameters keys = new NavigationParameters();
            configura?.Invoke(keys);
            RegionManager.RequestNavigate(
                regionName,
                viewName,
                ret => callBack?.Invoke(ret),
                keys);
        }
    }

    /// <summary>
    /// 向指定名称的区域中添加指定名称View实例<br/>
    /// 导航完毕后，从区域中移除该View实例
    /// </summary>
    /// <param name="regionName">区域名称.</param>
    /// <param name="viewName">实例名称.</param>
    /// <param name="configura">配置参数回调.</param>
    /// <param name="callBack">导航回调.</param>
    public void RequestNavigateWithRemoveView(string regionName, string viewName, Action<NavigationParameters>? configura = null, Action<NavigationResult?>? callBack = null)
    {
        RequestNavigate(regionName, viewName, configura, callBack);

        // 完毕后销毁进度条或载入界面
        RemoveView(regionName, viewName);
    }

    /// <summary>
    /// 向指定名称的区域中添加指定名称View实例<br/>
    /// 导航完毕后，从区域中移除该View实例
    /// </summary>
    /// <param name="regionName">区域名称.</param>
    /// <param name="viewName">实例名称.</param>
    /// <param name="configura">配置参数回调.</param>
    /// <param name="callBack">导航回调.</param>
    public Task<NavigationResult> RequestNavigateAsync(string regionName, string viewName, Action<NavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        var tcs = new TaskCompletionSource<NavigationResult>();
        RequestNavigate(regionName, viewName, configura, result =>
        {
            tcs.SetResult(result);
            callBack?.Invoke(result);
        });

        return tcs.Task;
    }

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="service">service.</param>
    /// <returns>returns.</returns>
    public bool TryResolve<TIServiceType>(out TIServiceType? service)
        where TIServiceType : class
    {
        service = null;

        if (container.IsRegistered(typeof(TIServiceType)))
        {
            service = container.Resolve<TIServiceType>();
            return true;
        }

        throw new InvalidOperationException($"{typeof(TIServiceType)} not registered.");
    }

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="service">service.</param>
    /// <param name="instanceName">示例名称.</param>
    /// <returns>returns.</returns>
    public bool TryResolve<TIServiceType>(out TIServiceType? service, string instanceName)
        where TIServiceType : class
    {
        service = null;

        if (container.IsRegistered(typeof(TIServiceType), instanceName))
        {
            service = container.Resolve<TIServiceType>(instanceName);
            return true;
        }

        throw new InvalidOperationException($"{typeof(TIServiceType)} not registered.");
    }

    /// <summary>
    /// TryResolve.
    /// </summary>
    /// <typeparam name="TIServiceType">TIServiceType.</typeparam>
    /// <param name="serviceName">serviceName.</param>
    /// <param name="service">service.</param>
    /// <returns>returns.</returns>
    public bool TryResolve<TIServiceType>(string serviceName, out TIServiceType? service)
        where TIServiceType : class
    {
        service = null;
        try
        {
            service = this.container.Resolve<TIServiceType>(serviceName);
            return true;
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// 获取多语言字符串资源。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    public string GetString(string key, params object[] args)
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
    public string GetString(string? assemblyName, string key, params object[] args)
    {
        return LocalizationProviderFactory.GetString(assemblyName, key, args);
    }

    /// <summary>
    /// Publishes the <see cref="PubSubEvent{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPlayLoad">Message to pass to the subscribers type.</typeparam>
    /// <param name="value">Message to pass to the subscribers.</param>
    public void Publish<TPlayLoad>(TPlayLoad value)
    {
        if (EventAggregator != null)
        {
            EventAggregator.GetEvent<PubSubEvent<TPlayLoad>>().Publish(value);
        }
    }

    /// <summary>
    /// Publishes the <see cref="PubSubEvent{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPlayLoad">Message to pass to the subscribers type.</typeparam>
    /// <param name="value">Message to pass to the subscribers.</param>
    public async Task PublishAsync<TPlayLoad>(TPlayLoad value)
    {
        await Task.Run(() =>
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<PubSubEvent<TPlayLoad>>().Publish(value);
            }
        });
    }

    /// <summary>
    /// Subscribes a delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
    /// <see cref="PubSubEvent{TPayload}"/> will maintain a <see cref="WeakReference"/> to the target of the supplied <paramref name="action"/> delegate.
    /// </summary>
    /// <typeparam name="TPlayLoad">target of the supplied</typeparam>
    /// <param name="action">The delegate that gets executed when the event is published.</param>
    /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
    /// <remarks>
    /// The PubSubEvent collection is thread-safe.
    /// </remarks>
    public SubscriptionToken? Subscribe<TPlayLoad>(Action<TPlayLoad>? action)
    {
        SubscriptionToken? token = null;
        if (EventAggregator != null)
        {
            token = EventAggregator.GetEvent<PubSubEvent<TPlayLoad>>().Subscribe(action);
        }

        return token;
    }

    /// <summary>
    /// Removes the first subscriber matching <see cref="Action{TPayload}"/> from the subscribers' list.
    /// </summary>
    /// <typeparam name="TPlayLoad">target of the supplied</typeparam>
    /// <param name="subscriber">The <see cref="Action{TPayload}"/> used when subscribing to the event.</param>
    public void Unsubscribe<TPlayLoad>(Action<TPlayLoad>? subscriber)
    {
        if (EventAggregator != null)
        {
            EventAggregator.GetEvent<PubSubEvent<TPlayLoad>>().Unsubscribe(subscriber);
        }
    }

    /// <summary>
    /// Publishes the <see cref="PubSubEvent{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPlayLoad">Message to pass to the subscribers type.</typeparam>
    /// <param name="value">Message to pass to the subscribers.</param>
    public void PublishEx<TPlayLoad>(TPlayLoad value)
        where TPlayLoad : PubSubEvent<TPlayLoad>, new()
    {
        if (EventAggregator != null)
        {
            EventAggregator.GetEvent<TPlayLoad>().Publish(value);
        }
    }

    /// <summary>
    /// Publishes the <see cref="PubSubEvent{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPlayLoad">Message to pass to the subscribers type.</typeparam>
    /// <param name="value">Message to pass to the subscribers.</param>
    public async Task PublishAsyncEx<TPlayLoad>(TPlayLoad value)
        where TPlayLoad : PubSubEvent<TPlayLoad>, new()
    {
        await Task.Run(() =>
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<TPlayLoad>().Publish(value);
            }
        });
    }

    /// <summary>
    /// Subscribes a delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
    /// <see cref="PubSubEvent{TPayload}"/> will maintain a <see cref="WeakReference"/> to the target of the supplied <paramref name="action"/> delegate.
    /// </summary>
    /// <typeparam name="TPlayLoad">target of the supplied</typeparam>
    /// <param name="action">The delegate that gets executed when the event is published.</param>
    /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
    /// <remarks>
    /// The PubSubEvent collection is thread-safe.
    /// </remarks>
    public SubscriptionToken? SubscribeEx<TPlayLoad>(Action<TPlayLoad>? action)
        where TPlayLoad : PubSubEvent<TPlayLoad>, new()
    {
        SubscriptionToken? token = null;
        if (EventAggregator != null)
        {
            token = EventAggregator.GetEvent<TPlayLoad>().Subscribe(action);
        }

        return token;
    }

    /// <summary>
    /// Removes the first subscriber matching <see cref="Action{TPayload}"/> from the subscribers' list.
    /// </summary>
    /// <typeparam name="TPlayLoad">target of the supplied</typeparam>
    /// <param name="subscriber">The <see cref="Action{TPayload}"/> used when subscribing to the event.</param>
    public void UnsubscribeEx<TPlayLoad>(Action<TPlayLoad>? subscriber)
        where TPlayLoad : PubSubEvent<TPlayLoad>, new()
    {
        if (EventAggregator != null)
        {
            EventAggregator.GetEvent<TPlayLoad>().Unsubscribe(subscriber);
        }
    }
}

public static class IocExtensions
{
    /// <summary>
    /// 通过反射机制注册 Ioc 单例实例
    /// <para>—— 此方法通过反射调用 Ioc 类的非公开构造函数创建实例，并将其注册为容器单例 ——</para>
    /// </summary>
    /// <param name="container">目标容器实例（需确保已完成基础配置）</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="container"/> 为 null 时隐式抛出</exception>
    /// <remarks>
    /// !!! 关键注意事项 !!!
    /// <list type="bullet">
    /// <item>依赖 Ioc 类必须存在接受 <see cref="IContainerRegistry"/> 参数的构造函数（可非公开）</item>
    /// <item>若目标构造函数不存在，本方法将静默失败（无异常抛出）</item>
    /// <item>注册操作具有幂等性，重复调用不会覆盖已注册实例</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// 典型注册场景：
    /// <code>
    /// var container = new ContainerRegistry();
    /// container.RegisterIocSingleton(); // 执行注册
    /// var ioc = container.Resolve<Ioc>(); // 获取单例
    /// </code> 
    /// </example>
    public static void RegisterIocSingleton(this IContainerRegistry container, Action<Ioc>? configure = null)
    {
        if (container.IsRegistered<Ioc>())
        {
            return;
        }

        // 反射获取非公开构造函数：要求构造函数签名匹配 (IContainerRegistry)
        // BindingFlags 组合说明：
        // - NonPublic：包含私有、内部、保护构造函数
        // - Instance：排除静态构造函数
        ConstructorInfo? constructor = typeof(Ioc).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(IContainerExtension) },
            null
        );

        Ioc? instance = null;
        if (constructor != null)
        {
            // 通过反射创建实例（可能触发构造函数异常）
            instance = (Ioc?)constructor.Invoke(new object[] { (container as IContainerExtension) });

            // 防御性空检查：防止构造函数返回 null（尽管非常规情况）
            if (instance != null)
            {
                // 注册单例：使用工厂委托延迟实例化
                // 注意：RegisterSingleton 的泛型重载可能更安全，此处使用 Type 版本保持灵活性
                _ = container.RegisterSingleton(typeof(Ioc), () => instance);

                // Ioc 创建完成后，执行可选的配置操作
                configure?.Invoke(instance);
                return;
            }
        }

        // 可选的调试辅助代码（发布时可移除）
#if DEBUG
        if (constructor == null || instance == null)
        {
            Debug.WriteLine("[IocRegistration] 警告：未找到匹配的 Ioc 构造函数，请检查参数类型和访问修饰符");
        }
#endif
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void Show<TView>(this Ioc ioc, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ioc.Show<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void Show<TView>(this Ioc ioc, string winName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ioc.Show(typeof(TView).Name, winName, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void Show(this Ioc ioc, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ioc.Show(viewName, string.Empty, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void Show(this Ioc ioc, string viewName, string winName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ioc.DialogService?.Show(viewName, winName, false, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void ShowDialog<TView>(this Ioc ioc, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ioc.ShowDialog<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void ShowDialog<TView>(this Ioc ioc, string winName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        ioc.ShowDialog(typeof(TView).Name, winName, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void ShowDialog(this Ioc ioc, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ioc.ShowDialog(viewName, string.Empty, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="callback">弹窗完成后 回调函数</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static void ShowDialog(this Ioc ioc, string viewName, string winName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        ioc.DialogService?.Show(viewName, winName, true, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowAsync<TView>(this Ioc ioc, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await ioc.ShowAsync<TView>(string.Empty, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowAsync<TView>(this Ioc ioc, string winName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await ioc.ShowAsync(typeof(TView).Name, winName, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowAsync(this Ioc ioc, string viewName, Action<IDialogParameters>? config = null)
    {
        return await ioc.ShowAsync(viewName, string.Empty, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowAsync(this Ioc ioc, string viewName, string winName, Action<IDialogParameters>? config = null)
    {
        return await ioc.DialogService?.ShowAsync(viewName, winName, false, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowDialogAsync<TView>(this Ioc ioc, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await ioc.ShowDialogAsync<TView>(string.Empty, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="ioc">ioc容器</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowDialogAsync<TView>(this Ioc ioc, string winName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await ioc.ShowDialogAsync(typeof(TView).Name, winName, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowDialogAsync(this Ioc ioc, string viewName, Action<IDialogParameters>? config = null)
    {
        return await ioc.ShowDialogAsync(viewName, string.Empty, config);
    }

    /// <summary>
    /// 模态弹窗提示
    /// </summary>
    /// <param name="ioc">ioc容器</param>
    /// <param name="viewName">弹窗视图名称</param>
    /// <param name="winName">自定义弹窗名称</param>
    /// <param name="config">配置弹窗显示内容</param>
    public static async Task<IDialogResult?> ShowDialogAsync(this Ioc ioc, string viewName, string winName, Action<IDialogParameters>? config = null)
    {
        return await ioc.DialogService?.ShowAsync(viewName, winName, true, config);
    }

    public static bool IsRegistered(this Ioc ioc, Type type) => ioc.Container?.IsRegistered(type) ?? false;

    public static bool IsRegistered(this Ioc ioc, Type type, string name) => ioc.Container?.IsRegistered(type, name) ?? false;

    public static T? Resolve<T>(this Ioc ioc)
    {
        return (T?)ioc.Container?.Resolve(typeof(T));
    }

    public static T? Resolve<T>(this Ioc ioc, params (Type Type, object Instance)[] parameters)
    {
        return (T?)ioc.Container?.Resolve(typeof(T), parameters);
    }

    public static T? Resolve<T>(this Ioc ioc, string name, params (Type Type, object Instance)[] parameters)
    {
        return (T?)ioc.Container?.Resolve(typeof(T), name, parameters);
    }

    public static T? Resolve<T>(this Ioc ioc, string name)
    {
        return (T?)ioc.Container?.Resolve(typeof(T), name);
    }
}