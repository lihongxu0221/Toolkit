using BgCommon;
using BgCommon.DependencyInjection;
using BgCommon.Localization;
using BgCommon.MVVM;
using CommunityToolkit.Mvvm.Input;
using Prism.Dialogs;

namespace BgControls.Controls;

/// <summary>
/// MessageBoxView.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Dialog, typeof(MessageBoxViewModel))]
public partial class MessageBoxView : UserControl, IRegistration
{
    public MessageBoxView()
    {
        InitializeComponent();
    }
}

/// <summary>
/// MessageBoxViewModel.cs .
/// </summary>
public partial class MessageBoxViewModel : DialogViewModelBase
{
    private string content = string.Empty;
    private string cancel = string.Empty;
    private string confirm = string.Empty;
    private int icon = 0;
    private bool isShowCopy2Clipboard = false;

    /// <summary>
    /// Gets or sets 消息主体
    /// </summary>
    public string Content
    {
        get => content;
        set => SetProperty(ref content, value);
    }

    /// <summary>
    /// Gets or sets 取消按钮文字
    /// </summary>
    public string Cancel
    {
        get => cancel;
        set => SetProperty(ref cancel, value);
    }

    /// <summary>
    /// Gets or sets 确认按钮文字
    /// </summary>
    public string Confirm
    {
        get => confirm;
        set => SetProperty(ref confirm, value);
    }

    /// <summary>
    /// Gets or sets 字体图标.
    /// </summary>
    public int Icon
    {
        get => icon;
        set => SetProperty(ref icon, value);
    }

    /// <summary>
    /// Gets or sets 字体图标.
    /// </summary>
    public bool IsShowCopy2Clipboard
    {
        get => isShowCopy2Clipboard;
        set => SetProperty(ref isShowCopy2Clipboard, value);
    }

    public MessageBoxViewModel(IContainerExtension container)
        : base(container)
    {
    }

    public override void OnDialogClosed()
    {

    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        if (parameters.TryGetValue(nameof(Title), out string? title) && !string.IsNullOrEmpty(title))
        {
            Title = title;
        }

        if (parameters.TryGetValue(nameof(Content), out string? content))
        {
            Content = content ?? string.Empty;
        }

        if (parameters.TryGetValue(nameof(Confirm), out string? confirm) && !string.IsNullOrEmpty(confirm))
        {
            Confirm = confirm ?? string.Empty;
        }

        if (parameters.TryGetValue(nameof(Cancel), out string? cancel) && !string.IsNullOrEmpty(cancel))
        {
            Cancel = cancel;
        }

        if (parameters.TryGetValue(nameof(Icon), out int? icon) && icon != null)
        {
            Icon = icon.Value;
        }

        IsShowCopy2Clipboard = Icon == 2 || Icon == 3;
    }

    /// <summary>
    /// 复制到剪贴板
    /// </summary>
    [RelayCommand]
    private void OnCopy()
    {
        if (string.IsNullOrEmpty(Content))
        {
            return;
        }

        // 复制到剪贴板
        Clipboard.SetText(Content);
    }
}

/// <summary>
/// MessageBox 扩展方法类
/// </summary>
public static partial class MessageBoxViewExtension
{
    /// <summary>
    /// 定义消息框中显示的图标类型。
    /// </summary>
    public enum DialogIcon
    {
        Info = 0,
        Question = 1,
        Warning = 2,
        Error = 3
    }

    public static IDialogResult? Error(this Ioc ioc, string content, string title = "")
    {
        Trace.TraceError(content);
        return ioc.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 3);
    }

    public static IDialogResult? Warn(this Ioc ioc, string content, string title = "")
    {
        Trace.TraceWarning(content);
        return ioc.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 2);
    }

    public static IDialogResult? Question(this Ioc ioc, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return ioc.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 1);
    }

    public static IDialogResult? Info(this Ioc ioc, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return ioc.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 0);
    }

    public static IDialogResult? Error(this ViewModelBase vm, string content, string title = "")
    {
        Trace.TraceError(content);
        return vm.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 3);
    }

    public static IDialogResult? Warn(this ViewModelBase vm, string content, string title = "")
    {
        Trace.TraceWarning(content);
        return vm.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 2);
    }

    public static IDialogResult? Question(this ViewModelBase vm, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return vm.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 1);
    }

    public static IDialogResult? Info(this ViewModelBase vm, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return vm.DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 0);
    }

    public static IDialogResult? Error(this IDialogService DialogService, string content, string title = "")
    {
        Trace.TraceError(content);
        return DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 3);
    }

    public static IDialogResult? Warn(this IDialogService DialogService, string content, string title = "")
    {
        Trace.TraceWarning(content);
        return DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 2);
    }

    public static IDialogResult? Question(this IDialogService DialogService, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 1);
    }

    public static IDialogResult? Info(this IDialogService DialogService, string content, string title = "")
    {
        Trace.TraceInformation(content);
        return DialogService?.ShowDialog(content, title, string.Empty, string.Empty, 0);
    }

    private static IDialogResult? ShowDialog(this IDialogService dialogService, string content, string title, string confirm, string cancel, int icon)
    {
        IDialogResult? result = null;
        dialogService.ShowDialog<MessageBoxView>(
        ret =>
        {
            result = ret;
        },
        keys =>
        {
            if (string.IsNullOrEmpty(title))
            {
                switch (icon)
                {
                    case 2:
                    case 3:
                        title = Ioc.Instance.GetString("SystemWarning");
                        break;
                    case 0:
                    case 1:
                    default:
                        title = Ioc.Instance.GetString("SystemPrompt");
                        break;
                }
            }

            if (string.IsNullOrEmpty(cancel))
            {
                cancel = Ioc.Instance.GetString("Cancel");
            }

            if (string.IsNullOrEmpty(confirm))
            {
                confirm = Ioc.Instance.GetString("Confirm");
            }

            keys.Add(nameof(MessageBoxViewModel.Title), title);
            keys.Add(nameof(MessageBoxViewModel.Content), content);
            keys.Add(nameof(MessageBoxViewModel.Confirm), confirm);
            keys.Add(nameof(MessageBoxViewModel.Cancel), cancel);
            keys.Add(nameof(MessageBoxViewModel.Icon), icon);
        });

        return result;
    }

    public static Task<IDialogResult?> ErrorAsync(this Ioc ioc, string content, string title = "")
        => ioc.DialogService!.ErrorAsync(content, title);

    public static Task<IDialogResult?> WarnAsync(this Ioc ioc, string content, string title = "")
        => ioc.DialogService!.WarnAsync(content, title);

    public static Task<IDialogResult?> QuestionAsync(this Ioc ioc, string content, string title = "")
        => ioc.DialogService!.QuestionAsync(content, title);

    public static Task<IDialogResult?> InfoAsync(this Ioc ioc, string content, string title = "")
        => ioc.DialogService!.InfoAsync(content, title);

    public static Task<IDialogResult?> ErrorAsync(this ViewModelBase vm, string content, string title = "")
        => vm.DialogService!.ErrorAsync(content, title);

    public static Task<IDialogResult?> WarnAsync(this ViewModelBase vm, string content, string title = "")
        => vm.DialogService!.WarnAsync(content, title);

    public static Task<IDialogResult?> QuestionAsync(this ViewModelBase vm, string content, string title = "")
        => vm.DialogService!.QuestionAsync(content, title);

    public static Task<IDialogResult?> InfoAsync(this ViewModelBase vm, string content, string title = "") 
        => vm.DialogService!.InfoAsync(content, title);

    /// <summary>
    /// 显示一个错误对话框。
    /// </summary>
    /// <param name="dialogService">对话框服务实例。</param>
    /// <param name="content">要显示的消息内容。</param>
    /// <param name="title">对话框的标题。如果为空，则使用系统预设值。</param>
    public static Task<IDialogResult?> ErrorAsync(this IDialogService dialogService, string content, string title = "")
    {
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Error);
    }

    /// <summary>
    /// 显示一个警告对话框。
    /// </summary>
    public static Task<IDialogResult?> WarnAsync(this IDialogService dialogService, string content, string title = "")
    {
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Warning);
    }

    /// <summary>
    /// 显示一个问题对话框。
    /// </summary>
    public static Task<IDialogResult?> QuestionAsync(this IDialogService dialogService, string content, string title = "")
    {
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Question);
    }

    /// <summary>
    /// 显示一个信息对话框。
    /// </summary>
    public static Task<IDialogResult?> InfoAsync(this IDialogService dialogService, string content, string title = "")
    {
        return dialogService.ShowDialogInternalAsync(content, title, DialogIcon.Info);
    }

    /// <summary>
    /// 显示对话框的核心实现，包含了特定的日志记录和默认值处理。
    /// </summary>
    private static async Task<IDialogResult?> ShowDialogInternalAsync(this IDialogService dialogService, string content, string title, DialogIcon icon)
    {
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
        IDialogResult? result = await dialogService.ShowDialogAsync<MessageBoxView>(
            keys =>
            {
                // 如果未提供标题，则根据图标类型设置默认标题
                if (string.IsNullOrEmpty(title))
                {
                    switch (icon)
                    {
                        case DialogIcon.Warning:
                        case DialogIcon.Error:
                            title = GetString("SystemWarning"); // 系统警告
                            break;
                        case DialogIcon.Info:
                        case DialogIcon.Question:
                        default:
                            title = GetString("SystemPrompt"); // 系统提示
                            break;
                    }
                }

                // 为对话框的 ViewModel 准备参数
                keys.Add(nameof(MessageBoxViewModel.Title), title);
                keys.Add(nameof(MessageBoxViewModel.Content), content);
                keys.Add(nameof(MessageBoxViewModel.Icon), (int)icon); // 将枚举作为整数传递

                // 设置默认按钮文本 (假设这些对话框类型的按钮文本总是相同的)
                keys.Add(nameof(MessageBoxViewModel.Confirm), GetString("Confirm")); // 确认
                keys.Add(nameof(MessageBoxViewModel.Cancel), GetString("Cancel"));   // 取消
            });

        return result;
    }

    /// <summary>
    /// 获取多语言字符串资源。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    private static string GetString(string key, params object[] args)
    {
        Assembly? assembly = Assembly.GetCallingAssembly();
        return GetString(assembly?.GetName()?.Name, key, args);
    }

    /// <summary>
    /// 获取多语言字符串资源
    /// </summary>
    /// <param name="assemblyName">资源包所在程序集名称</param>
    /// <param name="key">关键字</param>
    /// <param name="args">可变格式化字符串参数</param>
    /// <returns>多语言字符串资源</returns>
    private static string GetString(string? assemblyName, string key, params object[] args)
    {
        return LocalizationProviderFactory.GetString(assemblyName, key, args);
    }
}