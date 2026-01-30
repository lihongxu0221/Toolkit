using BgCommon.Prism.Wpf.Controls.Models;
using BgControls.Tools;
using BgControls.Windows.Datas;

namespace BgCommon.Prism.Wpf.Controls.ViewModels;

/// <summary>
/// InputMessageBoxViewModel.cs .
/// </summary>
public partial class InputMessageViewModel : DialogViewModelBase
{
    [ObservableProperty]
    private string okButtonText = "确定";

    [ObservableProperty]
    private string cancelButtonText = "取消";

    /// <summary>
    /// Gets or sets 消息主体.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<InputContent> contents = new ObservableCollection<InputContent>();

    /// <summary>
    /// Gets a value indicating whether gets 是否有子项.
    /// </summary>
    public bool HasItems => Contents.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputMessageViewModel"/> class.
    /// </summary>
    /// <param name="container">Ioc 容器.</param>
    public InputMessageViewModel(IContainerExtension container)
        : base(container)
    {
    }

    /// <inheritdoc/>
    protected override bool OnOK(object? parameter, ref IDialogParameters keys)
    {
        bool isAllValid = true;
        var builder = new StringBuilder();
        var rule = new RegexRule();
        for (int i = 0; i < Contents.Count; i++)
        {
            InputContent item = Contents[i];

            // 1. 非空验证 (对布尔类型跳过，因为它总是有值)
            if (item.TextType != TextType.Boolean && string.IsNullOrEmpty(item.ValueString))
            {
                _ = builder.AppendLine($"{item.Key}不能为空");
                isAllValid = false;
                continue;
            }

            // 2. 格式校验
            rule.Type = item.TextType;
            var vr = rule.Validate(item.ValueString, CultureInfo.CurrentUICulture);
            isAllValid &= vr.IsValid;
            if (!vr.IsValid)
            {
                _ = builder.AppendLine($"[{item.Key}] {vr.ErrorContent}");
            }
        }

        if (!isAllValid)
        {
            _ = this.Warn(builder.ToString());
            _ = builder.Clear();
            return false;
        }

        // 验证通过，将结果的深拷贝放入返回参数中
        keys.Add(Constraints.Results, Contents.Select(t => new InputContent(t)).ToArray());
        return true;
    }

    /// <inheritdoc/>
    public override void OnDialogOpened(IDialogParameters parameters)
    {
        string title = parameters.GetValue<string>(Constraints.Title) ?? "请输入参数";
        string okButtonText = parameters.GetValue<string>(Constraints.OkButtonText) ?? "确定";
        string cancelButtonText = parameters.GetValue<string>(Constraints.CancelButtonText) ?? "取消";

        Title = Ioc.GetString(title);
        OkButtonText = Ioc.GetString(okButtonText);
        CancelButtonText = Ioc.GetString(cancelButtonText);

        if (parameters.TryGetValue(Constraints.Parameters, out InputContent[]? inputItems) && inputItems != null)
        {
            // 创建参数的深拷贝，以防止对话框内外互相影响
            Contents = new ObservableCollection<InputContent>(
                inputItems.Select(item => new InputContent(item))
            );
        }
        else
        {
            // 如果没有参数传入，创建一个默认的输入项
            Contents.Add(new InputContent(1, $"{Ioc.GetString("参数")} 1", TextType.Common, string.Empty));
        }

        // 触发 HasItems 属性的变更通知
        OnPropertyChanged(nameof(HasItems));
    }
}