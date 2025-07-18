using BgCommon.Prism.Wpf.Controls.Services;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// Prism DialogService 相关的扩展方法.
/// </summary>
public static partial class DialogServiceExtensions
{
    private const string NonModalParameterKey = "nonModal";
    private const string WindowNameParameterKey = "windowName";

    private static readonly ConcurrentDictionary<string, Window?> OpenNonModelDialogs = new ConcurrentDictionary<string, Window?>();

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show<TView>(this IDialogService? dialogService, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        dialogService?.Show<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show<TView>(this IDialogService? dialogService, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        dialogService?.Show(typeof(TView).Name, windowName, callback, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show(this IDialogService? dialogService, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        dialogService?.Show(viewName, string.Empty, callback, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void Show(this IDialogService? dialogService, string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        dialogService?.Show(viewName, windowName, false, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog<TView>(this IDialogService? dialogService, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        dialogService?.ShowDialog<TView>(string.Empty, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog<TView>(this IDialogService? dialogService, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        dialogService?.ShowDialog(typeof(TView).Name, windowName, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog(this IDialogService? dialogService, string viewName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        dialogService?.ShowDialog(viewName, string.Empty, callback, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static void ShowDialog(this IDialogService? dialogService, string viewName, string windowName, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        dialogService?.Show(viewName, windowName, true, callback, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowAsync<TView>(this IDialogService? dialogService, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await dialogService.ShowAsync<TView>(string.Empty, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowAsync<TView>(this IDialogService? dialogService, string windowName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await dialogService.ShowAsync(typeof(TView).Name, windowName, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowAsync(this IDialogService? dialogService, string viewName, Action<IDialogParameters>? config = null)
    {
        return await dialogService.ShowAsync(viewName, string.Empty, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowAsync(this IDialogService? dialogService, string viewName, string windowName, Action<IDialogParameters>? config = null)
    {
        return await dialogService.ShowAsync(viewName, windowName, false, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowDialogAsync<TView>(this IDialogService? dialogService, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await dialogService.ShowDialogAsync<TView>(string.Empty, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <typeparam name="TView">弹窗界面的类型</typeparam>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowDialogAsync<TView>(this IDialogService? dialogService, string windowName, Action<IDialogParameters>? config = null)
           where TView : UserControl
    {
        return await dialogService.ShowDialogAsync(typeof(TView).Name, windowName, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowDialogAsync(this IDialogService? dialogService, string viewName, Action<IDialogParameters>? config = null)
    {
        return await dialogService.ShowDialogAsync(viewName, string.Empty, config);
    }

    /// <summary>
    /// 模态弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    public static async Task<IDialogResult?> ShowDialogAsync(this IDialogService? dialogService, string viewName, string windowName, Action<IDialogParameters>? config = null)
    {
        return await dialogService.ShowAsync(viewName, windowName, true, config);
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="isModal">是否为模态窗口.</param>
    /// <param name="callback">弹窗完成后 回调函数.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    internal static void Show(this IDialogService? dialogService, string viewName, string windowName, bool isModal, Action<IDialogResult>? callback = null, Action<IDialogParameters>? config = null)
    {
        if (dialogService == null)
        {
            return;
        }

        if (!isModal)
        {
            if (TryGetNonModelWindow(viewName, out Window? openWin))
            {
                if (openWin != null)
                {
                    if (openWin.WindowState == WindowState.Minimized)
                    {
                        openWin.WindowState = WindowState.Normal;
                    }

                    _ = openWin.Activate();
                    openWin.Topmost = true;
                    openWin.Topmost = false;
                    return;
                }
            }

            _ = OpenNonModelDialogs.TryAdd(viewName, null);
            try
            {
                IDialogParameters parameter = new DialogParameters();
                parameter.Add(NonModalParameterKey, !isModal);
                if (!string.IsNullOrEmpty(windowName))
                {
                    parameter.Add(WindowNameParameterKey, windowName);
                }

                config?.Invoke(parameter);
                dialogService?.ShowDialog(viewName, parameter, ret =>
                {
                    callback?.Invoke(ret);
                    if (!OpenNonModelDialogs.TryRemove(viewName, out _))
                    {
                        Debug.WriteLine($".openNonModelDialogs.TryRemove({viewName}) failure");
                    }
                });
            }
            catch (Exception)
            {
                _ = OpenNonModelDialogs.TryRemove(viewName, out _);
                throw;
            }
        }
        else
        {
            IDialogParameters parameter = new DialogParameters();
            if (!string.IsNullOrEmpty(windowName))
            {
                parameter.Add(WindowNameParameterKey, windowName);
            }

            config?.Invoke(parameter);
            dialogService?.ShowDialog(viewName, parameter, callback);
        }
    }

    /// <summary>
    /// 弹窗提示.
    /// </summary>
    /// <param name="dialogService">弹窗服务.</param>
    /// <param name="viewName">弹窗视图名称.</param>
    /// <param name="windowName">自定义弹窗名称.</param>
    /// <param name="isModal">是否为模态窗口.</param>
    /// <param name="config">配置弹窗显示内容.</param>
    internal static async Task<IDialogResult?> ShowAsync(this IDialogService? dialogService, string viewName, string windowName, bool isModal, Action<IDialogParameters>? config = null)
    {
        if (Application.Current.Dispatcher.CheckAccess())
        {
            if (dialogService == null)
            {
                return null;
            }

            if (!isModal)
            {
                if (TryGetNonModelWindow(viewName, out Window? openWin))
                {
                    if (openWin != null)
                    {
                        if (openWin.WindowState == WindowState.Minimized)
                        {
                            openWin.WindowState = WindowState.Normal;
                        }

                        _ = openWin.Activate();
                        openWin.Topmost = true;
                        openWin.Topmost = false;
                        return null;
                    }
                }

                _ = OpenNonModelDialogs.TryAdd(viewName, null);
                try
                {
                    IDialogParameters parameter = new DialogParameters();
                    parameter.Add(NonModalParameterKey, !isModal);
                    if (!string.IsNullOrEmpty(windowName))
                    {
                        parameter.Add(WindowNameParameterKey, windowName);
                    }

                    config?.Invoke(parameter);
                    IDialogResult? result = await dialogService.ShowDialogAsync(viewName, parameter);
                    if (!OpenNonModelDialogs.TryRemove(viewName, out _))
                    {
                        Debug.WriteLine($".openNonModelDialogs.TryRemove({viewName}) failure ");
                    }

                    return result;
                }
                catch (Exception)
                {
                    _ = OpenNonModelDialogs.TryRemove(viewName, out _);
                    throw;
                }
            }
            else
            {
                IDialogParameters parameter = new DialogParameters();

                if (!string.IsNullOrEmpty(windowName))
                {
                    parameter.Add(WindowNameParameterKey, windowName);
                }

                config?.Invoke(parameter);
                return await dialogService.ShowDialogAsync(viewName, parameter);
            }
        }
        else
        {
            // 不在UI线程，调度到UI线程
            DispatcherOperation<Task<IDialogResult?>> operation = Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                return await dialogService.ShowAsync(viewName, windowName, isModal, config);
            });

            Task<IDialogResult?> task = await operation;
            return await task;
        }
    }

    private static bool TryGetNonModelWindow(string viewName, out Window? openWindow)
    {
        openWindow = null;
        if (OpenNonModelDialogs.ContainsKey(viewName))
        {
            if (OpenNonModelDialogs.TryGetValue(viewName, out Window? openWin))
            {
                if (openWin == null)
                {
                    openWin = Application.Current.Windows.OfType<Window>().FirstOrDefault(
                        w => w.Content != null && w.Content.GetType().Name == viewName);

                    if (openWin != null)
                    {
                        OpenNonModelDialogs[viewName] = openWin;
                    }
                }

                openWindow = openWin;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 创建一个流畅的输入对话框构建器。
    /// </summary>
    /// <example>
    /// <code>
    /// var result = await new InputDialogBuilder(dialogService)
    ///                        .WithTitle("请输入用户信息")
    ///                        .AddTextInputs("姓名", "地址") // 批量添加文本框
    ///                        .AddIntInputs(new Dictionary&lt;string, int&gt; { { "年龄", 18 }, { "积分", 100 } }) // 批量添加整数框
    ///                        .AddBooleanInput("是否为会员", true) // 单独添加一个复选框
    ///                        .ShowDialogAsync();
    /// </code>
    /// </example>
    /// <param name="dialogService">Prism 对话框服务。</param>
    /// <param name="title">对话框的初始标题。</param>
    /// <returns>一个新的 InputDialogBuilder 实例。</returns>
    public static InputDialogBuilder CreateInputDialog(this IDialogService dialogService, string title)
    {
        return new InputDialogBuilder(dialogService).WithTitle(title);
    }
}