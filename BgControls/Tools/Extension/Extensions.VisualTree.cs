namespace BgControls;

/// <summary>
/// DependencyObject 帮助类。
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 获取可视化树父级。
    /// </summary>
    /// <param name="child">子控件</param>
    /// <param name="targetTypes">目标父级类型</param>
    /// <returns>可视化树父级</returns>
    private static DependencyObject? GetParent(this DependencyObject child, params Type[] targetTypes)
    {
        DependencyObject? parent = null;
        if (child is Visual)
        {
            parent = VisualTreeHelper.GetParent(child);

            // 递归向上查找，直到找到目标类型或没有父级
            while (parent != null)
            {
                if (targetTypes.Length > 0 && targetTypes.Any(t => t.IsInstanceOfType(parent)))
                {

                    return parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        return parent;
    }

    /// <summary>
    /// 获取逻辑树父级。
    /// </summary>
    /// <param name="child">子控件</param>
    /// <param name="targetTypes">目标父级类型</param>
    /// <returns>逻辑树父级</returns>
    private static DependencyObject? GetLogicalParent(this DependencyObject child, params Type[] targetTypes)
    {
        DependencyObject? parent = LogicalTreeHelper.GetParent(child);

        // 递归向上查找，直到找到目标类型或没有父级
        while (parent != null)
        {
            if (targetTypes.Length > 0 && targetTypes.Any(t => t.IsInstanceOfType(parent)))
            {
                return parent;
            }

            parent = LogicalTreeHelper.GetParent(parent);
        }

        return parent;
    }

    /// <summary>
    /// 查找指定类型的父级。
    /// </summary>
    /// <param name="child">子控件</param>
    /// <param name="targetTypes">目标父级类型</param>
    /// <returns>父级别</returns>
    public static DependencyObject? FindParent(this DependencyObject child, params Type[] targetTypes)
    {
        // 先尝试从可视化树查找
        DependencyObject? visualParent = child.GetParent(targetTypes);
        if (visualParent != null)
        {
            return visualParent;
        }

        // 如果可视化树未找到，再从逻辑树查找
        return child.GetLogicalParent(targetTypes);
    }

    /// <summary>
    /// 获取可视化树子控件
    /// </summary>
    /// <typeparam name="T">子控件类型</typeparam>
    /// <param name="parent">父控件</param>
    /// <param name="name">子控件名称</param>
    /// <returns>子控件</returns>
    public static T? FindVisualChild<T>(this DependencyObject parent, string? name = null)
        where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result &&
               (string.IsNullOrEmpty(name) || (result as FrameworkElement)?.Name == name))
            {
                return result;
            }
            else
            {
                T? descendant = FindVisualChild<T>(child, name);
                if (descendant != null)
                {
                    return descendant;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取可视化树子控件
    /// </summary>
    /// <param name="parent">父控件</param>
    /// <param name="childType">子控件类型</param>
    /// <param name="name">子控件名称</param>
    /// <returns>子控件</returns>
    public static DependencyObject? FindVisualChild(this DependencyObject parent, Type childType, string? name = null)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child.GetType() == childType &&
               (string.IsNullOrEmpty(name) || (child as FrameworkElement)?.Name == name))
            {
                return child;
            }
            else
            {
                DependencyObject? descendant = child.FindVisualChild(childType, name);
                if (descendant != null)
                {
                    return descendant;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 查找指定类型的父级。
    /// </summary>
    /// <typeparam name="T">父级类型</typeparam>
    /// <param name="obj">子项实例 </param>
    /// <returns>父级</returns>
    public static T? FindAncestor<T>(this DependencyObject obj)
        where T : DependencyObject
    {
        while (obj != null)
        {
            if (obj is T parent)
            {
                return parent;
            }

            obj = VisualTreeHelper.GetParent(obj);
        }
        return null;
    }
}