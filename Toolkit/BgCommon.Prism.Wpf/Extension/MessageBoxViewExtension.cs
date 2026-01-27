using BgCommon.Prism.Wpf.Controls;
using BgCommon.Prism.Wpf.Controls.ViewModels;

namespace BgCommon.Prism.Wpf;

public static class MessageBoxViewExtension
{
    /// <summary>
    /// 定义消息框中显示的图标类型。
    /// </summary>
    private enum DialogIcon
    {
        Info = 0,
        Question = 1,
        Warning = 2,
        Error = 3
    }

    public static IDialogResult? Error(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternal(content, title, DialogIcon.Error);
    }

    public static IDialogResult? Warn(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternal(content, title, DialogIcon.Warning);
    }

    public static IDialogResult? Question(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternal(content, title, DialogIcon.Question);
    }

    public static IDialogResult? Info(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternal(content, title, DialogIcon.Info);
    }

    private static IDialogResult? ShowDialogInternal(this IDialogService dialogService, string content, string title, DialogIcon icon)
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        IDialogResult? result = null;
        dialogService.ShowDialog<MessageView>(
        ret =>
        {
            result = ret;
        },
        keys =>
        {
            // 如果未提供标题，则根据图标类型设置默认标题
            if (string.IsNullOrEmpty(title))
            {
                switch (icon)
                {
                    case DialogIcon.Warning:
                    case DialogIcon.Error:
                        title = Ioc.GetString("系统警告");
                        break;
                    case DialogIcon.Info:
                    case DialogIcon.Question:
                    default:
                        title = Ioc.GetString("系统提示");
                        break;
                }
            }

            // 为对话框的 ViewModel 准备参数
            keys.Add(nameof(MessageViewModel.Title), title);
            keys.Add(nameof(MessageViewModel.Content), content);
            keys.Add(nameof(MessageViewModel.Icon), (int)icon); // 将枚举作为整数传递

            // 设置默认按钮文本 (假设这些对话框类型的按钮文本总是相同的)
            keys.Add(nameof(MessageViewModel.Confirm), Ioc.GetString("Confirm")); // 确认
            keys.Add(nameof(MessageViewModel.Cancel), Ioc.GetString("Cancel"));   // 取消
        });

        return result;
    }

    /// <summary>
    /// 显示一个错误对话框。
    /// </summary>
    /// <param name="dialogService">对话框服务实例。</param>
    /// <param name="content">要显示的消息内容。</param>
    /// <param name="title">对话框的标题。如果为空，则使用系统预设值。</param>
    public static Task<IDialogResult?> ErrorAsync(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Error);
    }

    /// <summary>
    /// 显示一个警告对话框。
    /// </summary>
    public static Task<IDialogResult?> WarnAsync(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Warning);
    }

    /// <summary>
    /// 显示一个问题对话框。
    /// </summary>
    public static Task<IDialogResult?> QuestionAsync(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Question);
    }

    /// <summary>
    /// 显示一个信息对话框。
    /// </summary>
    public static Task<IDialogResult?> InfoAsync(this IDialogService dialogService, string content, string title = "")
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Info);
    }

    /// <summary>
    /// 显示对话框的核心实现，包含了特定的日志记录和默认值处理。
    /// </summary>
    private static async Task<IDialogResult?> ShowDialogInternalAsync(this IDialogService dialogService, string content, string title, DialogIcon icon)
    {
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        // 1. 集中处理日志记录
        switch (icon)
        {
            case DialogIcon.Error:
                Trace.TraceError(content);
                break;
            case DialogIcon.Warning:
                Trace.TraceWarning(content);
                break;
            case DialogIcon.Info:
            case DialogIcon.Question:
            default:
                Trace.TraceInformation(content);
                break;
        }

        // 2. 集中处理对话框参数构建逻辑
        IDialogResult? result = await dialogService.ShowDialogAsync<MessageView>(
            keys =>
            {
                // 如果未提供标题，则根据图标类型设置默认标题
                if (string.IsNullOrEmpty(title))
                {
                    switch (icon)
                    {
                        case DialogIcon.Warning:
                        case DialogIcon.Error:
                            title = Ioc.GetString("系统警告");
                            break;
                        case DialogIcon.Info:
                        case DialogIcon.Question:
                        default:
                            title = Ioc.GetString("系统提示");
                            break;
                    }
                }

                // 为对话框的 ViewModel 准备参数
                keys.Add(nameof(MessageViewModel.Title), title);
                keys.Add(nameof(MessageViewModel.Content), content);
                keys.Add(nameof(MessageViewModel.Icon), (int)icon); // 将枚举作为整数传递

                // 设置默认按钮文本 (假设这些对话框类型的按钮文本总是相同的)
                keys.Add(nameof(MessageViewModel.Confirm), Ioc.GetString("Confirm")); // 确认
                keys.Add(nameof(MessageViewModel.Cancel), Ioc.GetString("Cancel"));   // 取消
            });

        return result;
    }
}