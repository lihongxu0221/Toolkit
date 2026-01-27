namespace BgCommon.Prism.Wpf.Controls.ViewModels;

/// <summary>
/// 消息弹窗视图.
/// </summary>
public partial class MessageViewModel : DialogViewModelBase
{
    private string content = string.Empty;
    private string cancel = string.Empty;
    private string confirm = string.Empty;
    private int icon = 0;
    private Visibility copyToClipboardVisibility = Visibility.Collapsed;

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
    /// Gets or sets a value indicating whether 是否显示复制提示信息的按钮.
    /// </summary>
    public Visibility CopyToClipboardVisibility
    {
        get => copyToClipboardVisibility;
        set => SetProperty(ref copyToClipboardVisibility, value);
    }

    public MessageViewModel(IContainerExtension container)
        : base(container)
    {
    }

    public override void OnDialogClosed() { }

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

        if (Icon == 2 || Icon == 3)
        {
            CopyToClipboardVisibility = Visibility.Visible;
        }
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