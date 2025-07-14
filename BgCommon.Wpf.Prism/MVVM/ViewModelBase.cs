using BgCommon.Localization;
using CommunityToolkit.Mvvm.Input;

namespace BgCommon.Wpf.Prism.MVVM;

/// <summary>
/// VM基类.
/// </summary>
public abstract class ViewModelBase : ObservableValidator, IActiveAware
{
    private readonly IContainerExtension container;
    private bool isActive = false;
    private IAsyncRelayCommand? loadedCommand = null;
    private Visibility visibility = Visibility.Visible;

    public ViewModelBase(IContainerExtension container)
    {
        ArgumentNullException.ThrowIfNull(nameof(container));
        this.container = container;

        if (this.TryResolve(out IEventAggregator? eventAggregator))
        {
            EventAggregator = eventAggregator;
        }

        if (this.TryResolve(out IRegionManager? regionManager))
        {
            RegionManager = regionManager;
        }

        if (this.TryResolve(out IDialogService? dialogService))
        {
            DialogService = dialogService;
        }
    }

    /// <summary>
    /// Gets 容器实例.
    /// </summary>
    public IContainerExtension Container => container;

    /// <summary>
    /// Gets 事件聚合.
    /// </summary>
    public IEventAggregator? EventAggregator { get; }

    /// <summary>
    /// Gets 模块区域管理.
    /// </summary>
    public IRegionManager? RegionManager { get; }

    /// <summary>
    /// Gets 弹窗服务.
    /// </summary>
    public IDialogService? DialogService { get; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or Sets IsActive.
    /// </summary>
    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            if (SetProperty(ref isActive, value))
            {
                if (value)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }
            }
        }
    }

    public event EventHandler? IsActiveChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the View is visible in the UI.
    /// </summary>
    public Visibility Visibility
    {
        get => visibility;
        set => SetProperty(ref visibility, value);
    }

    public IAsyncRelayCommand LoadedCommand
    {
        get
        {
            return loadedCommand ??= new AsyncRelayCommand<object>(OnLoadedAsync);
        }
    }

    /// <summary>
    /// 界面加载完成后执行的方法.(异步)
    /// </summary>
    /// <param name="parameter">参数</param>
    protected virtual Task OnLoadedAsync(object? parameter)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 注册事件.
    /// </summary>
    protected virtual void OnActivated()
    {
        // 订阅区域语言发生变化
        _ = this.Subscribe<ILocalizationProvider?>(OnCultureChanged);
    }

    /// <summary>
    /// 注销事件.
    /// </summary>
    protected virtual void OnDeactivated()
    {
        // 取消订阅区域语言变化
        this.Unsubscribe<ILocalizationProvider?>(OnCultureChanged);
    }

    /// <summary>
    /// 语言发生变化，用于接收多语言发生变更的消息.
    /// </summary>
    /// <param name="provider">多语言实例</param>
    protected virtual void OnCultureChanged(ILocalizationProvider? provider) { }

    /// <summary>
    /// 获取多语言字符串资源。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    protected string GetString(string key, params object[] args)
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
    protected string GetString(string? assemblyName, string key, params object[] args)
    {
        return Lang.GetString(assemblyName, key, args);
    }
}