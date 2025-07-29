using BgCommon;
using BgCommon.Prism.Wpf.Common.Views;
using BgCommon.Prism.Wpf.MVVM;
using ModuleInfo = BgCommon.Prism.Wpf.Authority.Entities.ModuleInfo;
using Regions = Prism.Navigation.Regions;

namespace BgCommon.Prism.Wpf.Common.ViewModels;

/// <summary>
/// 模块宿主视图模型类 (Prism 9.0 / .NET 8.0).
/// 作为一个可重用的导航目标，负责加载、缓存和显示模块内容，
/// 并提供丰富的生命周期管理、错误处理和用户体验优化.
/// </summary>
public partial class ModuleHostViewModel : NavigationViewModelBase
{
    /// <summary>
    /// 线程安全的视图缓存，用于存储标记为 <see cref="ICachedView"/> 的视图实例以提高性能.
    /// </summary>
    private readonly ConcurrentDictionary<Type, FrameworkElement> viewCache = new();

    /// <summary>
    /// 为当前加载的视图创建的局部作用域RegionManager.
    /// </summary>
    private IRegionManager? scopedRegionManager;

    /// <summary>
    /// 当前显示的内容视图.
    /// </summary>
    private object? currentContent;

    /// <summary>
    /// 当前显示视图的模块信息.
    /// </summary>
    private ModuleInfo? currentView;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleHostViewModel"/> class.
    /// </summary>
    /// <param name="container">Prism 依赖注入容器.</param>
    public ModuleHostViewModel(IContainerExtension container)
        : base(container)
    {
        // 初始化时显示加载视图，提供即时反馈
        SetCurrentContent(Container.Resolve<LoadingView>());
    }

    /// <summary>
    /// Gets 获取当前显示的内容视图.此属性的更新是线程安全的.
    /// </summary>
    public object? CurrentContent
    {
        get => this.currentContent;
        private set => SetProperty(ref this.currentContent, value);
    }

    /// <summary>
    /// 当导航完成并进入到此宿主视图时调用.
    /// 这是模块加载的核心逻辑，负责处理视图解析、初始化和显示.
    /// </summary>
    /// <param name="navigationContext">包含导航所需参数的上下文.</param>
    public override async void OnNavigatedTo(NavigationContext navigationContext)
    {
        var sw = Stopwatch.StartNew();

        // 从导航参数中安全地提取所需信息
        Type viewType = navigationContext.Parameters.GetValue<Type>(Constraints.TargetView);
        ModuleInfo currentView = navigationContext.Parameters.GetValue<ModuleInfo>(Constraints.CurrentView);

        // 防止对同一视图的重复导航，避免不必要的刷新
        if (currentView?.Equals(this.currentView) == true)
        {
            sw.Stop();
            this.PublishEx(new ModuleHostViewChanged(false, currentView, null, null));
            return;
        }

        string regionName = navigationContext.Parameters.GetValue<string>(Constraints.RegionName) ?? "MainContentRegion";
        this.PublishEx(new ModuleHostViewChanged(true, currentView, null, null));

        // 使用CancellationTokenSource来管理延迟加载指示器和可能的提前取消
        using var cts = new CancellationTokenSource();
        _ = ShowLoadingIndicatorAfterDelay(cts.Token);

        try
        {
            // 在加载新视图前，清理上一个视图及其资源
            CleanupPreviousView();
            this.currentView = currentView;

            // 为本次导航创建一个全新的作用域RegionManager.
            // 这确保了无论是新视图还是缓存视图，都能获得一个干净、独立的区域环境.
            this.scopedRegionManager = this.RegionManager?.CreateRegionManager();

            if (viewType is null)
            {
                LogRun.Warn("Navigation failed: TargetView parameter is missing.");
                SetCurrentContent(null);
                return;
            }

            // 解析或从缓存获取视图，并进行初始化
            FrameworkElement? view = await ResolveViewAsync(viewType, navigationContext, this.scopedRegionManager);
            if (view is null)
            {
                var error = new InvalidOperationException($"无法解析或初始化视图 {viewType.Name}.");
                LogRun.Error(error, error.Message);
                DisplayErrorView(error, navigationContext, currentView, regionName);
                return;
            }

            // 成功加载后，将最终视图设置到内容区
            SetCurrentContent(view);

            // 发布视图变更事件，通知其他部分
            this.PublishEx(new ModuleHostViewChanged(false, currentView, view.DataContext as ViewModelBase, null));

            // LogRun.Trace($"{currentView?.ModuleName} 导航到区域 {regionName} 成功，耗时 {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            LogRun.Error(ex, $"An unhandled exception occurred during navigation to {viewType?.Name}.");
            DisplayErrorView(ex, navigationContext, currentView, regionName);
        }
        finally
        {
            // 无论成功或失败，都取消延迟任务并停止计时
            cts.Cancel();
            sw.Stop();
        }
    }

    /// <summary>
    /// 在导航离开前进行确认.此实现将确认请求代理给当前承载的视图模型.
    /// </summary>
    /// <param name="navigationContext">导航上下文.</param>
    /// <param name="continuationCallback">回调，传入 true 继续导航，false 中止.</param>
    public override void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
        // 检查当前是否存在一个正在处理的导航票据
        if (NavigationGate.CurrentNavigationTicket.Value.HasValue)
        {
            // 如果有，说明这是一个在确认流程中发起的递归导航（如弹出对话框）.
            // 我们直接放行，不进行任何确认.
            continuationCallback(true);
            return;
        }

        if (this.CurrentContent is FrameworkElement { DataContext: IConfirmNavigationRequest confirmable })
        {
            // 这是首次进入确认流程，我们生成一个新的、唯一的导航票据
            var ticket = Guid.NewGuid();
            NavigationGate.CurrentNavigationTicket.Value = ticket;

            // 创建一个包装后的回调函数
            Action<bool> wrappedCallback = (canNavigate) =>
            {
                try
                {
                    // 在执行原始回调之前，先“作废”当前的导航票据
                    // 确保无论后续发生什么，票据都会被清理
                    NavigationGate.CurrentNavigationTicket.Value = null;

                    // 执行原始的回调
                    continuationCallback(canNavigate);
                }
                catch (Exception ex)
                {
                    // 如果原始回调中发生异常，也要确保票据被清理
                    LogRun.Error(ex, "Error in navigation continuation callback.");
                    NavigationGate.CurrentNavigationTicket.Value = null;
                }
            };

            try
            {
                // 将包装后的回调传递给子ViewModel
                confirmable.ConfirmNavigationRequest(navigationContext, wrappedCallback);
            }
            catch (Exception ex)
            {
                // 如果调用子ViewModel的确认方法本身就抛出异常，也要清理票据
                LogRun.Error(ex, "Error while calling ConfirmNavigationRequest on the child view model.");
                NavigationGate.CurrentNavigationTicket.Value = null;

                // 默认中止导航
                continuationCallback(false);
            }
        }
        else
        {
            // 否则，直接允许导航
            continuationCallback(true);
        }
    }

    /// <summary>
    /// 当此宿主ViewModel被销毁时调用，负责清理所有托管的资源，包括视图缓存.
    /// </summary>
    public override void Destroy()
    {
        CleanupPreviousView();

        // 遍历并销毁所有缓存的视图
        foreach (FrameworkElement view in this.viewCache.Values)
        {
            if (view.DataContext is IDestructible destructible)
            {
                destructible.Destroy();
            }

            if (view is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.viewCache.Clear();

        // 清理所有字段引用，帮助垃圾回收
        this.currentContent = null;
        this.currentView = null;
        this.scopedRegionManager = null;

        base.Destroy();
    }

    /// <summary>
    /// 异步解析并初始化视图.如果视图在缓存中，则直接返回；否则，创建新实例并初始化.
    /// </summary>
    /// <param name="viewType">要解析的视图类型.</param>
    /// <param name="navigationContext">导航上下文，用于初始化ViewModel.</param>
    /// <param name="newScopedRegionManager">接收新创建的 scopedRegionManager.</param>
    /// <returns>成功创建并初始化的视图实例，否则为 null.</returns>
    private async Task<FrameworkElement?> ResolveViewAsync(
        Type viewType,
        NavigationContext navigationContext,
        IRegionManager? newScopedRegionManager)
    {
        // 步骤 1: 检查视图缓存
        if (this.viewCache.TryGetValue(viewType, out FrameworkElement? cachedView))
        {
            LogRun.Trace($"View {viewType.Name} loaded from cache.");

            // 即使是缓存视图，也确保其 ViewModel 感知到当前的 scope
            if (cachedView.DataContext is IScopedRegionManagerAware scopedAware)
            {
                scopedAware.SetScopedRegionManager(newScopedRegionManager);
                Regions.RegionManager.SetRegionManager(cachedView, newScopedRegionManager);
                Regions.RegionManager.UpdateRegions(); // 确保区域被重新发现
            }

            return cachedView;
        }

        // 步骤 2: 从DI容器解析新视图
        var sw = new Stopwatch();
        sw.Start();
        var view = Container.Resolve(viewType) as FrameworkElement;
        Debug.WriteLine($"【Resolve】{viewType.Name} cost {sw.ElapsedMilliseconds} ms");
        if (view is null)
        {
            return null;
        }

        // 步骤 3: 为新视图创建和设置作用域RegionManager
        this.scopedRegionManager = this.RegionManager?.CreateRegionManager();
        if (view.DataContext is IScopedRegionManagerAware scopedAwareViewModel)
        {
            scopedAwareViewModel.SetScopedRegionManager(this.scopedRegionManager);
        }

        Regions.RegionManager.SetRegionManager(view, this.scopedRegionManager);

        // 步骤 4: 初始化ViewModel
        await InitializeViewModelAsync(view.DataContext, navigationContext.Parameters);

        // 步骤 5: 更新区域以确保新视图内的Region被正确识别
        Regions.RegionManager.UpdateRegions();

        // 步骤 6: 如果视图是可缓存的，添加到缓存中
        if (view is ICachedView)
        {
            _ = this.viewCache.TryAdd(viewType, view);
        }

        Debug.WriteLine($"【Total】{viewType.Name} cost {sw.ElapsedMilliseconds} ms");
        sw.Stop();
        return view;
    }

    /// <summary>
    /// 统一处理ViewModel的异步 (<see cref="IInitializeAsync"/>) 和同步 (<see cref="IInitialize"/>) 初始化.
    /// </summary>
    /// <param name="viewModel">要初始化的ViewModel实例.</param>
    /// <param name="parameters">初始化所需的导航参数.</param>
    private async Task InitializeViewModelAsync(object? viewModel, INavigationParameters parameters)
    {
        if (viewModel is IInitializeAsync initializeAsync)
        {
            await initializeAsync.InitializeAsync(parameters);
        }
        else if (viewModel is IInitialize initialize)
        {
            initialize.Initialize(parameters);
        }
    }

    /// <summary>
    /// 清理上一个显示的视图及其相关资源，除非它被缓存.
    /// </summary>
    private void CleanupPreviousView()
    {
        var oldContent = this.currentContent;
        if (oldContent is null)
        {
            return;
        }

        // 释放作用域RegionManager的引用，以便垃圾回收
        this.scopedRegionManager = null;

        // 如果视图被缓存，则跳过清理，以保留其状态
        if (oldContent is ICachedView)
        {
            return;
        }

        // 清理ViewModel（如果它实现了IDestructible且非持久化）
        if (oldContent is FrameworkElement { DataContext: object viewModel })
        {
            // 防御某个视图没有正确设定ViewModel 导致死循环
            if (ReferenceEquals(viewModel, this))
            {
                return;
            }

            if (viewModel is IDestructible destructible && viewModel is not IPersistAcrossNavigation)
            {
                destructible.Destroy();
            }
        }

        // 清理视图自身（如果它实现了IDisposable）
        if (oldContent is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// 线程安全地更新UI内容.
    /// </summary>
    /// <param name="content">要显示的新内容.</param>
    private void SetCurrentContent(object? content)
    {
        // 确保UI更新操作在主线程上执行，避免跨线程访问异常
        _ = Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            this.CurrentContent = content;
        });
    }

    /// <summary>
    /// 延迟指定时间（200毫秒）后显示加载指示器，以避免在快速导航时出现UI闪烁.
    /// </summary>
    /// <param name="token">用于取消延迟任务的CancellationToken.</param>
    private async Task ShowLoadingIndicatorAfterDelay(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200), token);

            // 如果延迟任务未被取消，说明导航时间较长，此时显示加载视图
            SetCurrentContent(Container.Resolve<LoadingView>());
        }
        catch (TaskCanceledException)
        {
            // 任务被取消是正常行为（因为导航已完成），无需处理
        }
    }

    /// <summary>
    /// 显示一个统一的错误视图，并提供可选的重试功能.
    /// </summary>
    /// <param name="ex">发生的异常.</param>
    /// <param name="context">原始的导航上下文.</param>
    /// <param name="module">发生错误的模块信息.</param>
    /// <param name="regionName">目标区域的名称.</param>
    private void DisplayErrorView(Exception ex, NavigationContext context, ModuleInfo? module, string regionName)
    {
        // 创建一个重试操作，它会重新发起原始的导航请求
        Action retryAction = () => this.RegionManager?.RequestNavigate(
            regionName,
            context.Uri.ToString(),
            context.Parameters);

        var errorParams = new NavigationParameters
        {
            { "error", ex },
            { "retryAction", retryAction }
        };

        // 手动解析ErrorView并初始化其ViewModel，这是Prism中手动创建带参数视图的标准模式
        ErrorView errorView = Container.Resolve<ErrorView>();
        if (errorView?.DataContext is IInitialize viewModel)
        {
            viewModel.Initialize(errorParams);
        }

        SetCurrentContent(errorView);

        // 发布错误事件
        this.PublishEx(new ModuleHostViewChanged(false, module, null, ex));
    }
}

/// <summary>
/// ModuleHostViewChanged.cs .
/// </summary>
public class ModuleHostViewChanged : PubSubEvent<ModuleHostViewChanged>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleHostViewChanged"/> class.
    /// </summary>
    public ModuleHostViewChanged()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleHostViewChanged"/> class.
    /// </summary>
    /// <param name="isBusy"> 是否处于忙碌状态. </param>
    /// <param name="viewModel"> 视图模型. </param>
    /// <param name="module">模块信息.</param>
    /// <param name="ex">异常信息.</param>
    public ModuleHostViewChanged(bool isBusy, ModuleInfo? module, ViewModelBase? viewModel, Exception? ex)
    {
        IsBusy = isBusy;
        Module = module;
        ViewModel = viewModel;
        Exception = ex;
    }

    /// <summary>
    /// Gets 模块信息.
    /// </summary>
    public ModuleInfo? Module { get; }

    /// <summary>
    /// Gets 视图模型.
    /// </summary>
    public ViewModelBase? ViewModel { get; }

    /// <summary>
    /// Gets 异常信息.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether gets 子模块是否触摸忙碌状态.
    /// </summary>
    public bool IsBusy { get; }
}

public static class NavigationGate
{
    /// <summary>
    /// 来确保ID在异步调用链中正确传递.
    /// </summary>
    public static readonly AsyncLocal<Guid?> CurrentNavigationTicket = new();
}