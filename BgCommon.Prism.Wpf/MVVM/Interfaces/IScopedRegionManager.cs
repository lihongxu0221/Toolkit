namespace BgCommon.Prism.Wpf.MVVM;

/// <summary>
/// 定义了一个接口，用于为实现了此接口的类（通常是 ViewModel）提供一个作用域或局部的 <see cref="IRegionManager"/> 实例。
/// 这个接口主要用于解决在独立窗口或隔离视图中，需要访问其自身而非全局区域管理器的问题.
/// </summary>
public interface IScopedRegionManagerAware
{
    /// <summary>
    /// 为当前实例设置一个 <see cref="IRegionManager"/>.
    /// 这个方法通常在关联的视图（View）被加载（Loaded）时，由视图的 Code-Behind 调用，
    /// 并传入该视图视觉树范围内的 <see cref="IRegionManager"/>.
    /// </summary>
    /// <param name="scopedRegionManager">
    /// 作用域内的区域管理器实例。它可以是全局管理器，也可以是专门为某个视觉树（如对话框）创建的局部管理器.
    /// </param>
    void SetScopedRegionManager(IRegionManager scopedRegionManager);
}
