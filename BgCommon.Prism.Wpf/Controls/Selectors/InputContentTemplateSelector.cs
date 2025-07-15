using BgCommon.Prism.Wpf.Controls.Models;
using BgControls.Data;

namespace BgCommon.Prism.Wpf.Controls.Selectors;

/// <summary>
/// 根据 InputContent 的 TextType 选择合适的数据模板。
/// </summary>
public class InputContentTemplateSelector : DataTemplateSelector
{
    // 在 XAML 中为这些属性赋值
    public DataTemplate? CommonTemplate { get; set; }

    public DataTemplate? NumericTemplate { get; set; }

    public DataTemplate? DateTimeTemplate { get; set; }

    public DataTemplate? BooleanTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not InputContent inputContent)
        {
            return base.SelectTemplate(item, container);
        }

        DataTemplate? template = null;
        switch (inputContent.TextType)
        {
            case TextType.Common:
            case TextType.Phone:
            case TextType.Mail:
            case TextType.Url:
            case TextType.Ip:
                template = this.CommonTemplate;
                break;

            case TextType.Int:
            case TextType.NInt:
            case TextType.PInt:
            case TextType.Short:
            case TextType.NShort:
            case TextType.PShort:
            case TextType.Float:
            case TextType.NFloat:
            case TextType.PFloat:
            case TextType.Double:
            case TextType.PDouble:
            case TextType.NDouble:
                template = this.NumericTemplate;
                break;

            case TextType.DateTime:
            case TextType.Boolean:
                template = this.BooleanTemplate;
                break;

            default:
                break;
        }

        return template ?? base.SelectTemplate(item, container);
    }
}