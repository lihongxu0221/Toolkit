namespace BgCommon.Prism.Wpf;

/// <summary>
/// 异步初始化接口.
/// </summary>
public interface IInitializeAsync
{
    /// <summary>
    /// 异步初始化方法.
    /// </summary>
    /// <param name="parameters">导航参数.</param>
    /// <returns>表示异步操作的任务.</returns>
    Task InitializeAsync(INavigationParameters? parameters = null);
}