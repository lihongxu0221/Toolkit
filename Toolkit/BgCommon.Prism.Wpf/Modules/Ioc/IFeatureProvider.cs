namespace BgCommon.Prism.Wpf.Modules;

/// <summary>
/// 定义一个功能提供者的契约，它能提供一个或多个功能树的根节点.
/// 任何需要向系统提供默认模块和权限的Prism模块都应实现此接口.
/// </summary>
public interface IFeatureProvider
{
    /// <summary>
    /// 获取此提供者定义的所有功能树的根节点.
    /// </summary>
    IEnumerable<FeatureSeedNode> GetFeatures();
}