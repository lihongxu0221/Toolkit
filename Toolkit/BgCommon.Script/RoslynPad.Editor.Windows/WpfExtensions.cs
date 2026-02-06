#pragma warning disable IDE0060 // 移除未使用的参数

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RoslynPad.Editor;

/// <summary>
/// WPF 相关组件的扩展方法静态类.
/// </summary>
internal static class WpfExtensions
{
    /// <summary>
    /// 获取指定 UI 元素所属的窗口.
    /// </summary>
    /// <param name="uiElement">目标 UI 元素.</param>
    /// <returns>返回所属的窗口实例.</returns>
    public static Window GetWindow(this UIElement uiElement)
    {
        return Window.GetWindow(uiElement);
    }

    /// <summary>
    /// 获取调度器对象关联的调度器.
    /// </summary>
    /// <param name="dispatcherObject">调度器对象.</param>
    /// <returns>返回关联的调度器实例接口.</returns>
    public static Dispatcher GetDispatcher(this DispatcherObject dispatcherObject)
    {
        return dispatcherObject.Dispatcher;
    }

    /// <summary>
    /// 获取元素的渲染尺寸.
    /// </summary>
    /// <param name="uiElement">目标 UI 元素.</param>
    /// <returns>返回当前的渲染尺寸.</returns>
    public static Size GetRenderSize(this UIElement uiElement)
    {
        return uiElement.RenderSize;
    }

    /// <summary>
    /// 挂载加载与卸载的动作回调.
    /// </summary>
    /// <param name="frameworkElement">框架元素.</param>
    /// <param name="loadAction">加载或卸载时执行的动作，参数为 true 表示加载，false 表示卸载.</param>
    public static void HookupLoadedUnloadedAction(this FrameworkElement frameworkElement, Action<bool> loadAction)
    {
        // 如果当前已经加载，立即触发一次动作.
        if (frameworkElement.IsLoaded)
        {
            loadAction?.Invoke(true);
        }

        // 订阅加载事件.
        frameworkElement.Loaded += (sender, eventArgs) => loadAction?.Invoke(true);

        // 订阅卸载事件.
        frameworkElement.Unloaded += (sender, eventArgs) => loadAction?.Invoke(false);
    }

    /// <summary>
    /// 附加窗口位置改变的事件处理器.
    /// </summary>
    /// <param name="window">目标窗口.</param>
    /// <param name="eventHandler">事件处理器.</param>
    public static void AttachLocationChanged(this Window window, EventHandler eventHandler)
    {
        window.LocationChanged += eventHandler;
    }

    /// <summary>
    /// 移除窗口位置改变的事件处理器.
    /// </summary>
    /// <param name="window">目标窗口.</param>
    /// <param name="eventHandler">要移除的事件处理器.</param>
    public static void DetachLocationChanged(this Window window, EventHandler eventHandler)
    {
        window.LocationChanged -= eventHandler;
    }

    /// <summary>
    /// 将可冻结对象设为冻结状态并返回.
    /// </summary>
    /// <typeparam name="T">Freezable 的子类型.</typeparam>
    /// <param name="freezableObject">要冻结的对象.</param>
    /// <returns>返回冻结后的对象实例.</returns>
    public static T AsFrozen<T>(this T freezableObject)
        where T : Freezable
    {
        freezableObject.Freeze();
        return freezableObject;
    }

    /// <summary>
    /// 在几何图形上下文中开始一段新的图形.
    /// </summary>
    /// <param name="geometryContext">几何上下文.</param>
    /// <param name="startPoint">起始点.</param>
    /// <param name="isFilled">是否填充该图形.</param>
    public static void BeginFigure(this StreamGeometryContext geometryContext, Point startPoint, bool isFilled)
    {
        geometryContext.BeginFigure(startPoint, isFilled, isClosed: false);
    }

    /// <summary>
    /// 设置控件的边框粗细.
    /// </summary>
    /// <param name="control">目标控件.</param>
    /// <param name="thicknessValue">粗细数值.</param>
    public static void SetBorderThickness(this Control control, double thicknessValue)
    {
        control.BorderThickness = new Thickness(thicknessValue);
    }

    /// <summary>
    /// 判断按键事件是否包含指定的组合键.
    /// </summary>
    /// <param name="keyEventArgs">按键事件参数.</param>
    /// <param name="modifierKeys">组合键类型.</param>
    /// <returns>如果包含则返回 true.</returns>
    public static bool HasModifiers(this KeyEventArgs keyEventArgs, ModifierKeys modifierKeys)
    {
        return (keyEventArgs.KeyboardDevice.Modifiers & modifierKeys) == modifierKeys;
    }

    /// <summary>
    /// 打开工具提示.
    /// </summary>
    /// <param name="toolTip">工具提示组件.</param>
    /// <param name="parentElement">父级元素参数.</param>
    public static void Open(this ToolTip toolTip, FrameworkElement parentElement)
    {
        toolTip.IsOpen = true;
    }

    /// <summary>
    /// 关闭工具提示.
    /// </summary>
    /// <param name="toolTip">工具提示组件.</param>
    /// <param name="parentElement">父级元素参数.</param>
    public static void Close(this ToolTip toolTip, FrameworkElement parentElement)
    {
        toolTip.IsOpen = false;
    }

    /// <summary>
    /// 设置工具提示的内容.
    /// </summary>
    /// <param name="toolTip">工具提示组件.</param>
    /// <param name="parentControl">父级控件.</param>
    /// <param name="content">要设置的内容对象.</param>
    public static void SetContent(this ToolTip toolTip, Control parentControl, object content)
    {
        toolTip.Content = content;
    }

    /// <summary>
    /// 在指定元素处打开右键菜单.
    /// </summary>
    /// <param name="contextMenu">右键菜单组件.</param>
    /// <param name="targetElement">定位的目标元素.</param>
    public static void Open(this ContextMenu contextMenu, FrameworkElement targetElement)
    {
        contextMenu.PlacementTarget = targetElement;
        contextMenu.IsOpen = true;
    }

    /// <summary>
    /// 打开右键菜单.
    /// </summary>
    /// <param name="contextMenu">右键菜单组件.</param>
    public static void Open(this ContextMenu contextMenu)
    {
        contextMenu.IsOpen = true;
    }

    /// <summary>
    /// 关闭右键菜单.
    /// </summary>
    /// <param name="contextMenu">右键菜单组件.</param>
    public static void Close(this ContextMenu contextMenu)
    {
        contextMenu.IsOpen = false;
    }

    /// <summary>
    /// 获取调度器的等待器实现.
    /// </summary>
    /// <param name="dispatcher">调度器实例.</param>
    /// <returns>返回调度器异步等待器.</returns>
    public static DispatcherYieldAwaiter GetAwaiter(this Dispatcher dispatcher)
    {
        return new DispatcherYieldAwaiter(dispatcher, DispatcherPriority.Normal);
    }

    /// <summary>
    /// 调度器异步等待器结构体.
    /// </summary>
    public readonly struct DispatcherYieldAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 调度器字段.
        /// </summary>
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// 优先级字段.
        /// </summary>
        private readonly DispatcherPriority priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherYieldAwaiter"/> struct.
        /// </summary>
        /// <param name="dispatcher">执行上下文调度器.</param>
        /// <param name="priority">任务执行优先级.</param>
        public DispatcherYieldAwaiter(Dispatcher dispatcher, DispatcherPriority priority)
        {
            // 初始化字段成员.
            this.dispatcher = dispatcher;
            this.priority = priority;
        }

        /// <summary>
        /// Gets a value indicating whether 当前是否已在调度器线程中完成.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                // 检查当前线程是否有权访问该调度器.
                return this.dispatcher.CheckAccess();
            }
        }

        /// <summary>
        /// 结束等待并获取结果（用于验证线程访问）.
        /// </summary>
        public void GetResult()
        {
            // 验证线程访问权限.
            this.dispatcher.VerifyAccess();
        }

        /// <summary>
        /// 当任务完成时注册回调动作.
        /// </summary>
        /// <param name="continuation">待执行的后续动作.</param>
        public void OnCompleted(Action continuation)
        {
            // 将动作提交到调度器队列执行.
            this.dispatcher.InvokeAsync(continuation, this.priority);
        }

        /// <summary>
        /// 安全地当任务完成时注册回调动作（不捕获执行上下文）.
        /// </summary>
        /// <param name="continuation">待执行的后续动作.</param>
        public void UnsafeOnCompleted(Action continuation)
        {
            // 直接复用完成逻辑.
            this.OnCompleted(continuation);
        }
    }
}