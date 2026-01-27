using Microsoft.Expression.Interactions;

namespace BgCommon.Prism.Wpf.MVVM;

/// <summary>
/// 视图激活行为，用于开发者手动解析（container.Resolve()）的视图.
/// </summary>
public class ViewActivationBehavior : Behavior<FrameworkElement>
{
    /// <summary>
    /// 当Behavior被附加到View上时调用.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        // 订阅事件
        WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler(
            this.AssociatedObject,
            nameof(this.AssociatedObject.Loaded),
            this.OnViewLoaded);
        WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler(
            this.AssociatedObject,
            nameof(this.AssociatedObject.Unloaded),
            this.OnViewUnloaded);
    }

    /// <summary>
    /// 当Behavior从View上分离时调用.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        // 取消订阅，防止内存泄漏
        WeakEventManager<FrameworkElement, RoutedEventArgs>.RemoveHandler(
            this.AssociatedObject,
            nameof(this.AssociatedObject.Loaded),
            this.OnViewLoaded);
        WeakEventManager<FrameworkElement, RoutedEventArgs>.RemoveHandler(
            this.AssociatedObject,
            nameof(this.AssociatedObject.Unloaded),
            this.OnViewUnloaded);
    }

    private void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        // 当View加载时，尝试激活ViewModel
        if (this.AssociatedObject.DataContext is IActiveAware viewModel)
        {
            // 防御性检查：只有在VM尚未被激活时才激活它。
            // 这使得此行为可以与Prism的激活机制（它会先激活VM）和平共存。
            if (!viewModel.IsActive)
            {
                viewModel.IsActive = true;
            }
        }
    }

    private void OnViewUnloaded(object? sender, RoutedEventArgs e)
    {
        // 当View卸载时，尝试失活ViewModel
        if (this.AssociatedObject.DataContext is IActiveAware viewModel)
        {
            if (viewModel.IsActive)
            {
                viewModel.IsActive = false;
            }
        }
    }
}