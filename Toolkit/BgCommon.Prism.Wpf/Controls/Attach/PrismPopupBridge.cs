namespace BgCommon.Prism.Wpf.Controls.Attach;

/// <summary>
/// 桥接 Prism RegionManager 到 Popup 控件的工具类.
/// </summary>
public static class PrismPopupBridge
{
    /// <summary>
    /// 标识是否启用桥接功能的附加属性.
    /// </summary>
    public static readonly DependencyProperty IsBridgeEnabledProperty =
        DependencyProperty.RegisterAttached("IsBridgeEnabled", typeof(bool), typeof(PrismPopupBridge), new PropertyMetadata(false, OnIsBridgeEnabledChanged));

    /// <summary>
    /// 标识目标区域控件名称的附加属性.
    /// </summary>
    public static readonly DependencyProperty RegionControlNameProperty =
        DependencyProperty.RegisterAttached("RegionControlName", typeof(string), typeof(PrismPopupBridge), new PropertyMetadata(null));

    /// <summary>
    /// 获取区域控件的名称.
    /// </summary>
    /// <param name="obj">拥有此属性的依赖对象.</param>
    /// <returns>控件名称字符串.</returns>
    public static string GetRegionControlName(DependencyObject obj)
    {
        return (string)obj.GetValue(RegionControlNameProperty);
    }

    /// <summary>
    /// 设置区域控件的名称.
    /// </summary>
    /// <param name="obj">拥有此属性的依赖对象.</param>
    /// <param name="value">控件名称值.</param>
    public static void SetRegionControlName(DependencyObject obj, string value)
    {
        obj.SetValue(RegionControlNameProperty, value);
    }

    /// <summary>
    /// 获取是否启用桥接功能的值.
    /// </summary>
    /// <param name="obj">拥有此属性的依赖对象.</param>
    /// <returns>是否启用的布尔值.</returns>
    public static bool GetIsBridgeEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsBridgeEnabledProperty);
    }

    /// <summary>
    /// 设置是否启用桥接功能的值.
    /// </summary>
    /// <param name="obj">拥有此属性的依赖对象.</param>
    /// <param name="value">是否启用.</param>
    public static void SetIsBridgeEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsBridgeEnabledProperty, value);
    }

    /// <summary>
    /// 当 IsBridgeEnabled 属性改变时的回调处理.
    /// </summary>
    /// <param name="dependencyObject">发生改变的依赖对象.</param>
    /// <param name="args">事件参数.</param>
    private static void OnIsBridgeEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        // 确保目标对象是 Popup 控件
        if (dependencyObject is Popup popupControl)
        {
            if ((bool)args.NewValue)
            {
                // 订阅打开和卸载事件
                popupControl.Opened += OnPopupOpened;
                popupControl.Unloaded += OnPopupUnloaded;
            }
            else
            {
                // 取消订阅事件
                popupControl.Opened -= OnPopupOpened;
                popupControl.Unloaded -= OnPopupUnloaded;
            }
        }
    }

    /// <summary>
    /// 当 Popup 打开时的处理逻辑.
    /// </summary>
    /// <param name="sender">事件发送者.</param>
    /// <param name="args">事件参数.</param>
    private static void OnPopupOpened(object? sender, EventArgs args)
    {
        var popupControl = sender as Popup;
        if (popupControl == null || popupControl.PlacementTarget == null)
        {
            return;
        }

        // 获取预设的控件名称
        string targetControlName = GetRegionControlName(popupControl);
        if (string.IsNullOrEmpty(targetControlName))
        {
            return;
        }

        // 从放置目标（如主窗口按钮）获取当前的 RegionManager
        var regionManager = GetRegionManagerFromAncestors(popupControl.PlacementTarget);
        if (regionManager == null)
        {
            return;
        }

        // 在 Popup 的 Child 内容中递归寻找指定名称的控件
        var foundControl = FindChildByName(popupControl.Child, targetControlName);
        if (foundControl != null)
        {
            // 核心桥接：将主程序的 RegionManager 注入到 Popup 内部控件中
            RegionManager.SetRegionManager(foundControl, regionManager);
        }
    }

    /// <summary>
    /// 当 Popup 卸载时的处理逻辑.
    /// </summary>
    /// <param name="sender">事件发送者.</param>
    /// <param name="args">路由事件参数.</param>
    private static void OnPopupUnloaded(object sender, RoutedEventArgs args)
    {
        var popupControl = sender as Popup;
        if (popupControl != null)
        {
            // 移除事件监听防止内存泄漏
            popupControl.Opened -= OnPopupOpened;
            popupControl.Unloaded -= OnPopupUnloaded;

            // 清理内部控件对 RegionManager 的引用
            if (popupControl.Child != null)
            {
                string targetControlName = GetRegionControlName(popupControl);
                var foundControl = FindChildByName(popupControl.Child, targetControlName);
                if (foundControl != null)
                {
                    // 辅助 GC 回收，清除附加属性值
                    foundControl.ClearValue(RegionManager.RegionManagerProperty);
                }
            }
        }
    }

    /// <summary>
    /// 从指定对象的视觉树向上查找 RegionManager.
    /// </summary>
    /// <param name="target">起始查找对象.</param>
    /// <returns>找到的 IRegionManager 实例，若未找到则返回 null.</returns>
    private static IRegionManager? GetRegionManagerFromAncestors(DependencyObject target)
    {
        DependencyObject? currentObject = target;

        while (currentObject != null)
        {
            // 尝试获取当前节点上挂载的 RegionManager
            var regionManager = RegionManager.GetRegionManager(currentObject);
            if (regionManager != null)
            {
                return regionManager;
            }

            // 沿着视觉树向上查找
            currentObject = VisualTreeHelper.GetParent(currentObject);
        }

        return null;
    }

    /// <summary>
    /// 递归寻找指定名称的子控件.
    /// </summary>
    /// <param name="parent">父级对象.</param>
    /// <param name="name">目标控件名称.</param>
    /// <returns>找到的依赖对象，若未找到则返回 null.</returns>
    private static DependencyObject? FindChildByName(DependencyObject parent, string name)
    {
        if (parent == null)
        {
            return null;
        }

        // 遍历视觉树子节点
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var childObject = VisualTreeHelper.GetChild(parent, i);

            // 匹配控件名称
            if (childObject is FrameworkElement frameworkElement && frameworkElement.Name == name)
            {
                return frameworkElement;
            }

            // 递归搜索
            var recursiveResult = FindChildByName(childObject, name);
            if (recursiveResult != null)
            {
                return recursiveResult;
            }
        }

        return null;
    }
}
