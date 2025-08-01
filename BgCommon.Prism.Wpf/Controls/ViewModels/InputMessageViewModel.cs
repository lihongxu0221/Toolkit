using BgCommon.Prism.Wpf.Controls.Models;
using BgCommon.Prism.Wpf.MVVM;
using BgControls.Data;
using BgControls.Tools;

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
    /// Gets or sets 消息主体
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<InputContent> contents = new ObservableCollection<InputContent>();

    /// <summary>
    /// Gets a value indicating whether gets 是否有子项
    /// </summary>
    public bool HasItems => Contents.Count > 0;

    public InputMessageViewModel(IContainerExtension container)
        : base(container)
    {
    }

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

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        string title = parameters.GetValue<string>(Constraints.Title) ?? "请输入参数";
        string okButtonText = parameters.GetValue<string>(Constraints.OkButtonText) ?? "确定";
        string cancelButtonText = parameters.GetValue<string>(Constraints.CancelButtonText) ?? "取消";

        Title = GetString(title);
        OkButtonText = GetString(okButtonText);
        CancelButtonText = GetString(cancelButtonText);

        if (parameters.TryGetValue(Constraints.Parameters, out InputContent[]? inputItems) && inputItems != null)
        {
            // 使用 LINQ 和 ObservableCollection 的构造函数来简化代码
            // 创建参数的深拷贝，以防止对话框内外互相影响
            Contents = new ObservableCollection<InputContent>(
                inputItems.Select(item => new InputContent(item)) // 假设InputContent有拷贝构造函数
            );
        }
        else
        {
            // 如果没有参数传入，创建一个默认的输入项
            Contents.Add(new InputContent(1, $"{GetString("参数")} 1", TextType.Common, string.Empty));
        }

        // 触发 HasItems 属性的变更通知
        OnPropertyChanged(nameof(HasItems));
    }
}