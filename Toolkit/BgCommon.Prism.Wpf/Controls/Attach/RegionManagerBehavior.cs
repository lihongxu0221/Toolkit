using Prism.Navigation.Regions;
using System.Collections.Specialized;
using System.Drawing;

namespace BgCommon.Prism.Wpf.Controls.Attach;

/// <summary>
/// RegionManagerAwareBehavior.cs<br/>
/// 1.RegionName：为 Popup 提供区域支持.由于Popup的特殊性，需要直接在其上指定RegionName.<br/>
/// 2.DialogAttach：为普通对话框/FrameworkElement提供区域支持.<br/>
/// 优化版：修复了内存泄漏，增强了健壮性，并增加了自动清理功能.<br/>
/// </summary>
public static class RegionManagerBehavior
{
    /// <summary>
    /// 定义附加属性 RegionName，用于为 Popup 提供区域支持. 该属性的值类型为字符串.
    /// </summary>
    public static readonly DependencyProperty RegionNameProperty =
        DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionManagerBehavior), new PropertyMetadata(null, OnRegionNameChanged));

    /// <summary>
    /// 定义附加属性 DialogAttach，用于为普通对话框/FrameworkElement提供区域支持. 该属性的值类型为布尔值.
    /// </summary>
    public static readonly DependencyProperty DialogAttachProperty =
       DependencyProperty.RegisterAttached("DialogAttach", typeof(bool), typeof(RegionManagerBehavior), new PropertyMetadata(false, OnDialogAttachPropertyChanged));

    /// <summary>
    /// 定义附加属性 AttachCompletedCommand，用于在对话框/FrameworkElement附加完成后执行命令. 该属性的值类型为 ICommand.
    /// </summary>
    public static readonly DependencyProperty AttachCompletedCommandProperty =
        DependencyProperty.RegisterAttached("AttachCompletedCommand", typeof(ICommand), typeof(RegionManagerBehavior), null);

    /// <summary>
    /// 定义附加属性 AttachCompletedCommandParameter，用于在对话框/FrameworkElement附加完成后执行命令时传递参数. 该属性的值类型为 object.
    /// </summary>
    public static readonly DependencyProperty AttachCompletedCommandParameterProperty =
        DependencyProperty.RegisterAttached("AttachCompletedCommandParameter", typeof(object), typeof(RegionManagerBehavior), null);

    /// <summary>
    /// 设置 DialogAttach 附加属性的值.
    /// </summary>
    /// <param name="dp">要设置属性的依赖对象.</param>
    /// <param name="value">要设置的值，指示是否为普通对话框/FrameworkElement提供区域支持.</param>
    public static void SetDialogAttach(DependencyObject dp, bool value)
    {
        dp.SetValue(DialogAttachProperty, value);
    }

    /// <summary>
    /// 获取 DialogAttach 附加属性的值.
    /// </summary>
    /// <param name="dp">要获取属性值的依赖对象.</param>
    /// <returns>指示是否为普通对话框/FrameworkElement提供区域支持的值.</returns>
    public static bool GetDialogAttach(DependencyObject dp)
    {
        return (bool)dp.GetValue(DialogAttachProperty);
    }

    /// <summary>
    /// 设置 AttachCompletedCommand 附加属性的值.
    /// </summary>
    /// <param name="element">要设置属性的依赖对象.</param>
    /// <param name="value">要设置的值，指示在对话框/FrameworkElement附加完成后执行的命令.</param>
    public static void SetAttachCompletedCommand(DependencyObject element, ICommand value) => element.SetValue(AttachCompletedCommandProperty, value);

    /// <summary>
    /// 获取 AttachCompletedCommand 附加属性的值.
    /// </summary>
    /// <param name="element">要获取属性值的依赖对象.</param>
    /// <returns>指示在对话框/FrameworkElement附加完成后执行的命令.</returns>
    public static ICommand GetAttachCompletedCommand(DependencyObject element) => (ICommand)element.GetValue(AttachCompletedCommandProperty);

    /// <summary>
    /// 设置 AttachCompletedCommandParameter 附加属性的值.
    /// </summary>
    /// <param name="element">要设置属性的依赖对象.</param>
    /// <param name="value">要设置的值，指示在对话框/FrameworkElement附加完成后执行命令时传递的参数.</param>
    public static void SetAttachCompletedCommandParameter(DependencyObject element, object value) => element.SetValue(AttachCompletedCommandParameterProperty, value);

    /// <summary>
    /// 获取 AttachCompletedCommandParameter 附加属性的值.
    /// </summary>
    /// <param name="element">要获取属性值的依赖对象.</param>
    /// <returns>指示在对话框/FrameworkElement附加完成后执行命令时传递的参数.</returns>
    public static object GetAttachCompletedCommandParameter(DependencyObject element) => (object)element.GetValue(AttachCompletedCommandParameterProperty);

    /// <summary>
    /// 获取 RegionName 附加属性的值.
    /// </summary>
    /// <param name="obj">要获取属性值的依赖对象.</param>
    /// <returns>指示为 Popup 提供区域支持的区域名称.</returns>
    public static string GetRegionName(DependencyObject obj)
    {
        return (string)obj.GetValue(RegionNameProperty);
    }

    /// <summary>
    /// 设置 RegionName 附加属性的值.
    /// </summary>
    /// <param name="obj">要设置属性的依赖对象.</param>
    /// <param name="value">要设置的值，指示为 Popup 提供区域支持的区域名称.</param>
    public static void SetRegionName(DependencyObject obj, string value)
    {
        obj.SetValue(RegionNameProperty, value);
    }

    /// <summary>
    /// 当 RegionName 附加属性的值发生变化时调用.
    /// </summary>
    /// <param name="d">发生变化的依赖对象.</param>
    /// <param name="e">包含旧值和新值的事件数据.</param>
    private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Popup popup)
        {
            popup.Loaded -= OnPopupLoaded;
            if (e.NewValue is string regionName && !string.IsNullOrEmpty(regionName))
            {
                popup.Loaded += OnPopupLoaded;
            }
        }
    }

    /// <summary>
    /// 当 DialogAttach 附加属性的值发生变化时调用.
    /// </summary>
    /// <param name="d">发生变化的依赖对象.</param>
    /// <param name="e">包含旧值和新值的事件数据.</param>
    private static void OnDialogAttachPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement owner)
        {
            owner.Loaded -= OnFrameworkElementLoaded;
            owner.Unloaded -= OnFrameworkElementUnloaded;
            if ((bool)e.NewValue)
            {
                owner.Loaded += OnFrameworkElementLoaded;
                owner.Unloaded += OnFrameworkElementUnloaded;
            }
        }
    }

    /// <summary>
    /// 当 FrameworkElement 加载时调用.
    /// </summary>
    /// <param name="sender">事件源，Window 实例.</param>
    /// <param name="e">事件数据.</param>
    private static void OnFrameworkElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement owner)
        {
            // 立即取消订阅，防止重入
            owner.Loaded -= OnFrameworkElementLoaded;
            if (Ioc.Container == null)
            {
                // 如果仍然是null，可以考虑添加日志记录来帮助调试
                Debug.WriteLine("Ioc.Container is null on FrameworkElement Loaded, please call Ioc.Initialize .");
                return;
            }

            // // 获取全局的 RegionManager 实例
            // IRegionManager regionManager = Ioc.Resolve<IRegionManager>();
            // RegionManager.SetRegionManager(owner, regionManager);

            // 获取全局的 RegionManager 实例, 注册临时区域
            IRegionManager regionManager = Ioc.Resolve<IRegionManager>();
            IRegionManager scopedRegionManager = regionManager.CreateRegionManager();
            RegionManager.SetRegionManager(owner, scopedRegionManager);
            if (owner.DataContext is IScopedRegionManagerAware regionManagerAware)
            {
                regionManagerAware.SetScopedRegionManager(scopedRegionManager);
            }

            ExecuteAttachCompletedCommand(owner);
        }
    }

    /// <summary>
    /// 当 FrameworkElement 卸载时调用.
    /// </summary>
    /// <param name="sender">事件源，Window 实例.</param>
    /// <param name="e">事件数据.</param>
    private static void OnFrameworkElementUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement view)
        {
            // 立即取消订阅，防止重入
            view.Unloaded -= OnFrameworkElementUnloaded;
            IRegionManager? regionManager = RegionManager.GetRegionManager(view);
            if (regionManager == null)
            {
                return;
            }

            List<IRegion> regions = new List<IRegion>();
            FindRegionsRecursive(view, regionManager, regions);

            foreach (IRegion region in regions)
            {
                region.RemoveAll();
                regionManager.Regions.Remove(region.Name);
            }
        }
    }

    /// <summary>
    /// 当 Popup 加载时调用.
    /// </summary>
    /// <param name="sender">事件源，Popup 实例.</param>
    /// <param name="e">事件数据.</param>
    private static void OnPopupLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Popup popup)
        {
            // 立即移除，防止内存泄漏
            popup.Loaded -= OnPopupLoaded;

            if (popup.Child is not FrameworkElement child)
            {
                return;
            }

            string regionName = GetRegionName(popup);
            if (string.IsNullOrEmpty(regionName))
            {
                return;
            }

            // 关键优化 2: 使用最可靠的方式查找 RegionManager
            Window? window = Window.GetWindow(popup) ?? Application.Current?.MainWindow;
            if (window == null)
            {
                return;
            }

            IRegionManager? scopedRegionManager = RegionManager.GetRegionManager(window);
            if (scopedRegionManager == null)
            {
                return;
            }

            // 检查区域是否已存在
            if (!scopedRegionManager.Regions.ContainsRegionWithName(regionName))
            {
                RegionManager.SetRegionName(child, regionName);
                RegionManager.SetRegionManager(child, scopedRegionManager);

                // RegionManager.UpdateRegions();
            }

            ExecuteAttachCompletedCommand(popup);

            // 增加自动清理功能
            popup.Unloaded += OnPopupUnloaded;
        }
    }

    /// <summary>
    /// 当 Popup 卸载时调用.
    /// </summary>
    /// <param name="sender">事件源，Popup 实例.</param>
    /// <param name="e">事件数据.</param>
    private static void OnPopupUnloaded(object sender, RoutedEventArgs e)
    {
        var popup = (Popup)sender;
        popup.Unloaded -= OnPopupUnloaded; // 清理自身

        string regionName = GetRegionName(popup);
        if (string.IsNullOrEmpty(regionName))
        {
            return;
        }

        Window? window = Window.GetWindow(popup) ?? Application.Current?.MainWindow;
        if (window == null)
        {
            return;
        }

        IRegionManager? scopedRegionManager = RegionManager.GetRegionManager(window);
        if (scopedRegionManager != null && scopedRegionManager.Regions.ContainsRegionWithName(regionName))
        {
            _ = scopedRegionManager.Regions.Remove(regionName);
        }
    }

    private static void ExecuteAttachCompletedCommand(FrameworkElement element)
    {
        ICommand command = GetAttachCompletedCommand(element);
        object commandParameter = GetAttachCompletedCommandParameter(element);

        if (command != null && command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    /// <summary>
    /// 在指定的WPF视图（或任何依赖对象）内部查找所有已注册的Prism区域。
    /// </summary>
    /// <param name="view">要搜索的视图，例如Window, UserControl等。</param>
    /// <returns>一个包含找到的IRegion实例的列表。</returns>
    public static List<IRegion> FindRegionsInView(DependencyObject view)
    {
        var regions = new List<IRegion>();
        if (view == null)
        {
            return regions;
        }

        // 1. 获取管理此视图的RegionManager。
        //    这很关键，因为它可能是全局管理器，也可能是一个作用域管理器。
        var regionManager = RegionManager.GetRegionManager(view);
        if (regionManager == null)
        {
            // 如果此视图的任何部分都未与RegionManager关联，则不可能有区域。
            return regions;
        }

        // 2. 启动递归查找
        FindRegionsRecursive(view, regionManager, regions);

        return regions;
    }

    private static void FindRegionsRecursive(DependencyObject parent, IRegionManager regionManager, List<IRegion> foundRegions)
    {
        // 检查当前对象本身是否定义了一个区域
        var regionName = RegionManager.GetRegionName(parent);
        if (!string.IsNullOrEmpty(regionName))
        {
            // 如果找到了区域名称，就从管理器中获取其实例
            if (regionManager.Regions.ContainsRegionWithName(regionName))
            {
                var region = regionManager.Regions[regionName];
                if (!foundRegions.Contains(region))
                {
                    foundRegions.Add(region);
                }
            }
        }

        // 遍历所有子元素
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            FindRegionsRecursive(child, regionManager, foundRegions);
        }
    }
}
