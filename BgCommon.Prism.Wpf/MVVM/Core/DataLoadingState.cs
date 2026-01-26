namespace BgCommon.Prism.Wpf;

/// <summary>
/// 表示数据加载过程的各种状态.
/// </summary>
public enum DataLoadingState
{
    /// <summary>
    /// 初始状态，尚未开始加载.
    /// </summary>
    NotLoaded,

    /// <summary>
    /// 正在进行首次数据加载.
    /// </summary>
    LoadingInitial,

    /// <summary>
    /// 数据已成功加载且有内容.
    /// </summary>
    Loaded,

    /// <summary>
    /// 数据加载成功，但结果为空.
    /// </summary>
    Empty,

    /// <summary>
    /// 正在刷新现有数据.
    /// </summary>
    Refreshing,

    /// <summary>
    /// 数据加载或刷新过程中发生错误.
    /// </summary>
    Error
}