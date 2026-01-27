namespace BgCommon.Prism.Wpf.Localization.Markup;

/// <summary>
/// 嵌入资源枚举绑定扩展<br/>
/// 1. 绑定单枚举值<br/>
/// 2. 获取枚举全部值
/// </summary>
[MarkupExtensionReturnType(typeof(BindingExpression))]
public class EnumResourceExtension : MarkupBindableBaseExtension
{
    private object? key;

    /// <summary>
    /// Gets or sets 绑定的键。
    /// </summary>
    [ConstructorArgument("key")]
    public object? Key
    {
        get => key;
        set => _ = SetProperty(ref key, value);
    }

    public EnumResourceExtension()
        : this(string.Empty)
    {
    }

    public EnumResourceExtension(object? key)
    {
        Key = key;

    }

    protected override void Initial(DependencyObject owner)
    {
        if (owner is ComboBox comboBox && comboBox != null)
        {
            comboBox.DisplayMemberPath = nameof(EnumModel.Display);
            comboBox.SelectedValuePath = nameof(EnumModel.Value);
            return;
        }
    }

    ///  <inheritdoc/>
    protected override void SetValue()
    {
        Value = null;

        // 绑定单枚举值
        if (Key is Enum enumValue)
        {
            EnumModel @enum = enumValue.GetEnumModel();
            Value = @enum.Display;
            return;
        }

        // 获取枚举全部值
        if (Key is Type type)
        {
            Type? enumType = Nullable.GetUnderlyingType(type) ?? type;
            if (enumType.IsEnum)
            {
                Value = enumType.GetEnumModels();
            }

            return;
        }
    }
}