using BgCommon.Prism.Wpf;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// 自定义异常，用于清晰地表示导航失败.
/// </summary>
public class NavigationFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationFailedException"/> class.
    /// </summary>
    /// <param name="message">描述导航失败的消息.</param>
    /// <param name="innerException">导致当前异常的内部异常（可选）.</param>
    public NavigationFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// 为 Prism 9.0 的 IRegionManager 提供一组便捷的扩展方法.
/// 此类旨在简化区域导航、视图管理和参数传递等常见操作，并充分利用现代 C# 特性.
/// </summary>
public static class RegionManagerExtensions
{
    /// <summary>
    /// 通过视图类型导航到指定区域.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigate<TView>(this IRegionManager regionManager, string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        regionManager.RequestNavigate(regionName, typeof(TView).Name, configura, callBack);
    }

    /// <summary>
    /// 通过视图名称导航到指定区域，并可选择传递导航参数.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">要导航到的视图的名称（在DI容器中注册的名称）.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigate(this IRegionManager regionManager, string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        INavigationParameters parameters = new NavigationParameters();
        configura?.Invoke(parameters);
        regionManager.RequestNavigate(regionName, viewName, callBack ?? (_ => { }), parameters);
    }

    /// <summary>
    /// 以异步方式通过视图类型导航到指定区域，并等待导航完成.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configure">设置要传递给目标视图的导航参数（可选）.</param>
    /// <returns>一个表示异步导航操作的任务，其结果是导航结果 <see cref="NavigationResult"/>.</returns>
    public static async Task<NavigationResult> RequestNavigateAsync<TView>(
        this IRegionManager regionManager,
        string regionName,
        Action<INavigationParameters>? configure = null)
    {
        return await regionManager.RequestNavigateAsync(regionName, typeof(TView).Name, configure);
    }

    /// <summary>
    /// 以异步方式导航到指定区域，并等待导航完成.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">要导航到的视图的名称.</param>
    /// <param name="configure">设置要传递给目标视图的导航参数（可选）.</param>
    /// <returns>一个表示异步导航操作的任务，其结果是导航结果 <see cref="NavigationResult"/>.</returns>
    public static Task<NavigationResult> RequestNavigateAsync(
        this IRegionManager regionManager,
        string regionName,
        string viewName,
        Action<INavigationParameters>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        ArgumentException.ThrowIfNullOrEmpty(regionName);
        ArgumentException.ThrowIfNullOrEmpty(viewName);

        INavigationParameters parameters = new NavigationParameters();
        configure?.Invoke(parameters);
        var tcs = new TaskCompletionSource<NavigationResult>();
        regionManager.RequestNavigate(
            regionName,
            viewName,
            navigationResult =>
            {
                // 关键：检查导航结果
                if (navigationResult.Success)
                {
                    // 成功时，完成任务
                    tcs.SetResult(navigationResult);
                }
                else
                {
                    Exception ex = navigationResult.Exception ?? new Exception("未知的导航错误");
                    tcs.SetException(new NavigationFailedException($"导航到 '{viewName}' 失败。", ex));
                }
            },
            parameters);

        return tcs.Task;
    }

    /// <summary>
    /// 导航到指定视图.如果该视图类型的实例已存在于区域中，则激活它；否则，创建新实例并导航.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigateAndActivate<TView>(this IRegionManager regionManager, string regionName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        regionManager.RequestNavigateAndActivate(regionName, typeof(TView).Name, configura, callBack);
    }

    /// <summary>
    /// 导航到指定视图.如果该视图类型的实例已存在于区域中，则激活它；否则，创建新实例并导航.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">目标区域的名称.</param>
    /// <param name="viewName">视图的名称.</param>
    /// <param name="configura">设置要传递给目标视图的导航参数（可选）.</param>
    /// <param name="callBack">导航完成后的回调函数（可选）.</param>
    public static void RequestNavigateAndActivate(this IRegionManager regionManager, string regionName, string viewName, Action<INavigationParameters>? configura = null, Action<NavigationResult>? callBack = null)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        if (string.IsNullOrEmpty(regionName) || string.IsNullOrEmpty(viewName))
        {
            return;
        }

        if (!regionManager.Regions.ContainsRegionWithName(regionName))
        {
            return;
        }

        IRegion region = regionManager.Regions[regionName];

        // 查找区域中是否已存在同名视图类型的实例
        var view = region.Views.FirstOrDefault(v => v.GetType().Name == viewName);

        if (view != null)
        {
            // 如果视图已存在，则直接激活它
            region.Activate(view);

            // 手动调用回调，确保行为一致
            var navigationContext = new NavigationContext(region.NavigationService, new Uri(viewName, UriKind.Relative));
            var result = new NavigationResult(navigationContext, true); // true 表示成功
            callBack?.Invoke(result);

            // 对于已存在的视图，如果它实现了 IRegionMemberLifetime 并且 KeepAlive 为 false，
            // 它可能已经被销毁.Prism 的 Activate 行为会处理基本的激活逻辑.
            // 如果需要向已激活的视图传递新信息，建议使用事件聚合器 (IEventAggregator).
        }
        else
        {
            // 如果视图不存在，则执行常规导航
            regionManager.RequestNavigate(regionName, viewName, configura, callBack);
        }
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    public static void ClearRegion(this IRegionManager regionManager, string regionName)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        if (!regionManager.Regions.ContainsRegionWithName(regionName))
        {
            return;
        }

        IRegion region = regionManager.Regions[regionName];

        // ToList() 创建一个副本，以避免在迭代时修改集合
        var viewsToRemove = region.Views.ToList();

        foreach (var view in viewsToRemove)
        {
            // 在Prism 9中，Remove操作会自动处理视图的销毁（如果实现了IDestructible）
            region.Remove(view);
        }
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <typeparam name="TView">要导航到的视图的类型.</typeparam>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    public static void ClearRegionView<TView>(this IRegionManager regionManager, string regionName)
        where TView : class
    {
        regionManager.ClearRegionView(regionName, typeof(TView).Name);
    }

    /// <summary>
    /// 清空指定区域内的所有视图.
    /// 此方法会从区域中移除所有视图，并会触发 `IRegionMemberLifetime.KeepAlive = false` 视图的销毁逻辑.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">要清空的区域的名称.</param>
    /// <param name="viewName">要清空的视图的名称.</param>
    public static void ClearRegionView(this IRegionManager regionManager, string regionName, string viewName)
    {
        ArgumentNullException.ThrowIfNull(regionManager);
        if (!regionManager.Regions.ContainsRegionWithName(regionName))
        {
            return;
        }

        IRegion region = regionManager.Regions[regionName];

        // ToList() 创建一个副本，以避免在迭代时修改集合
        var viewsToRemove = region.Views.Where(v => v.GetType().Name == viewName).ToList();
        foreach (var view in viewsToRemove)
        {
            // 在Prism 9中，Remove操作会自动处理视图的销毁（如果实现了IDestructible）
            region.Remove(view);
        }
    }

    /// <summary>
    /// 获取指定区域内的所有视图的集合.
    /// </summary>
    /// <param name="regionManager">区域管理器实例.</param>
    /// <param name="regionName">区域的名称.</param>
    /// <returns>一个包含区域内所有视图对象的只读集合，如果区域不存在则返回空集合.</returns>
    public static IEnumerable<object> GetViews(this IRegionManager regionManager, string regionName)
    {
        ArgumentNullException.ThrowIfNull(regionManager);

        if (regionManager.Regions.ContainsRegionWithName(regionName))
        {
            return regionManager.Regions[regionName].Views;
        }

        return Enumerable.Empty<object>();
    }
}