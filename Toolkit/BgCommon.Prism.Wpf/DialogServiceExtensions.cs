namespace BgCommon.Prism.Wpf;

/// <summary>
/// 对话框服务扩展类.
/// </summary>
public static partial class DialogServiceExtensions
{
    /// <summary>
    /// 在对话框宿主中显示对话框.
    /// </summary>
    /// <param name="dialogService">对话框服务实例.</param>
    /// <param name="name">对话框视图的名称.</param>
    /// <param name="parameters">传递给对话框的参数.</param>
    /// <param name="callback">对话框关闭后的回调操作.</param>
    public static void ShowDialogHost(
        this IDialogService dialogService,
        string name,
        IDialogParameters parameters,
        Action<IDialogResult>? callback = null)
    {
        // 尝试将通用对话框服务转换为具体的 MaterialDialogService 类型.
        if (dialogService is MaterialDialogService materialService)
        {
            // 调用具体的 Material 风格对话框显示逻辑.
            materialService.ShowDialogHost(name, parameters, callback);
        }
    }

    /// <summary>
    /// 在指定的对话框宿主中显示对话框.
    /// </summary>
    /// <param name="dialogService">对话框服务实例.</param>
    /// <param name="name">对话框视图的名称.</param>
    /// <param name="windowName">目标窗口或对话框宿主的名称.</param>
    /// <param name="parameters">传递给对话框的参数.</param>
    /// <param name="callback">对话框关闭后的回调操作.</param>
    public static void ShowDialogHost(
        this IDialogService dialogService,
        string name,
        string windowName,
        IDialogParameters parameters,
        Action<IDialogResult>? callback = null)
    {
        // 尝试将通用对话框服务转换为具体的 MaterialDialogService 类型.
        if (dialogService is MaterialDialogService materialService)
        {
            // 调用带有宿主标识参数的显示逻辑.
            materialService.ShowDialogHost(name, windowName, parameters, callback);
        }
    }
}