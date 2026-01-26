namespace BgCommon.Prism.Wpf;

/// <summary>
/// 定义视图的加载状态，用于UI展示。
/// </summary>
public enum ViewState
{
    /// <summary>
    /// 初始状态或正在加载。
    /// </summary>
    Loading,

    /// <summary>
    /// 数据加载成功。
    /// </summary>
    Loaded,

    /// <summary>
    /// 数据加载失败。
    /// </summary>
    Error
}
