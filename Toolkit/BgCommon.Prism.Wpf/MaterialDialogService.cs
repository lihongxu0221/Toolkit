using BgControls.Windows.Controls;
using System.Windows.Interop;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// 基于 Material Design 的对话框服务实现类.
/// </summary>
public class MaterialDialogService : DialogService
{
    // 容器扩展字段，不以下划线开头.
    private readonly IContainerExtension container;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialDialogService"/> class.
    /// </summary>
    /// <param name="container">Ioc容器.</param>
    public MaterialDialogService(IContainerExtension container)
        : base(container)
    {
        // 显式引用当前类的字段.
        this.container = container;
    }

    /// <summary>
    /// 在默认对话框宿主中显示对话框.
    /// </summary>
    /// <param name="name">对话框视图注册名称.</param>
    /// <param name="parameters">对话框参数.</param>
    /// <param name="callback">对话框关闭后的回调动作.</param>
    public void ShowDialogHost(string name, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        // 调用重载方法，传递空字符串作为默认宿主名称.
        this.ShowDialogHost(name, string.Empty, parameters, callback);
    }

    /// <summary>
    /// 在指定的对话框宿主中显示对话框.
    /// </summary>
    /// <param name="name">对话框视图注册名称.</param>
    /// <param name="dialogHostName">DialogHost 控件的标识名称.</param>
    /// <param name="parameters">对话框参数.</param>
    /// <param name="callback">对话框关闭后的回调动作.</param>
    public void ShowDialogHost(string name, string dialogHostName, IDialogParameters parameters, Action<IDialogResult> callback)
    {
        // 如果参数为空，则创建一个新的对话框参数实例.
        if (parameters == null)
        {
            parameters = (IDialogParameters)new DialogParameters();
        }

        // 从容器中解析视图实例.
        if (!(IContainerProviderExtensions.Resolve<object>((IContainerProvider)(object)this.container, name) is FrameworkElement dialogView))
        {
            throw new NullReferenceException("对话框的内容必须是 FrameworkElement 类型.");
        }

        // 自动绑定视图模型.
        AutowireViewModel(dialogView);

        // 获取视图的数据上下文并转换为 IDialogAware 接口.
        object viewModel = dialogView.DataContext;
        IDialogAware? dialogAware = viewModel as IDialogAware;

        if (dialogAware == null)
        {
            throw new ArgumentException("对话框的 ViewModel 必须实现 IDialogAware 接口.");
        }

        // 创建对话框打开时的事件处理程序.
        var openedHandler = new DialogOpenedEventHandler((sender, args) =>
        {
            // 触发 ViewModel 的打开通知.
            dialogAware.OnDialogOpened(parameters);
        });

        // 创建对话框关闭时的事件处理程序.
        var closedHandler = new DialogClosedEventHandler((sender, args) =>
        {
            // 触发 ViewModel 的关闭通知.
            dialogAware.OnDialogClosed();
        });

        // 初始化对话框监听器逻辑.
        DialogUtilities.InitializeListener(dialogAware, dialogResult =>
        {
            // 如果对话框当前处于打开状态，则执行回调并关闭对话框.
            if (DialogHost.IsDialogOpen(dialogHostName))
            {
                callback.Invoke(dialogResult);
                DialogHost.Close(dialogHostName);
            }
        });

        // 创建调度器帧以模拟模态效果.
        DispatcherFrame modalFrame = new DispatcherFrame();

        // 根据宿主标识符显示对话框.
        if (dialogHostName == null)
        {
            DialogHost.ShowAsync(dialogView, openedHandler, null, closedHandler)
                      .ContinueWith(task => modalFrame.Continue = false);
        }
        else
        {
            DialogHost.ShowAsync(dialogView, dialogHostName, openedHandler, null, closedHandler)
                      .ContinueWith(task => modalFrame.Continue = false);
        }

        try
        {
            // 推送模态状态并启动帧循环.
            ComponentDispatcher.PushModal();
            Dispatcher.PushFrame(modalFrame);
        }
        finally
        {
            // 弹出模态状态.
            ComponentDispatcher.PopModal();
        }

        // 对话框结束后清理监听器.
        DialogUtilities.InitializeListener(dialogAware, null);
    }

    /// <summary>
    /// 自动为视图绑定 ViewModel.
    /// </summary>
    /// <param name="viewOrViewModel">视图或视图模型对象.</param>
    private static void AutowireViewModel(object viewOrViewModel)
    {
        // 检查是否需要通过 ViewModelLocator 自动挂载.
        if (viewOrViewModel is FrameworkElement { DataContext: null } viewElement &&
            !ViewModelLocator.GetAutoWireViewModel((DependencyObject)viewElement).HasValue)
        {
            ViewModelLocator.SetAutoWireViewModel((DependencyObject)viewElement, true);
        }
    }
}