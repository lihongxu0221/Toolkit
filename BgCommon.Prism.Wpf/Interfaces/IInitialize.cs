namespace BgCommon.Prism.Wpf;

/// <summary>
/// 初始化接口.
/// </summary>
public interface IInitialize
{
    /// <summary>
    /// 初始化方法.
    /// </summary>
    /// <param name="parameters">导航参数.</param>
    void Initialize(INavigationParameters? parameters = null);
}