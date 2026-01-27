namespace BgCommon.Prism.Wpf.Localization.Markup;

/// <summary>
/// 枚举列表标记扩展
/// </summary>
public class EnumBindingSourceExtension : MarkupExtension
{
    /// <summary>
    /// Gets 绑定的键。
    /// </summary>
    [ConstructorArgument("enumType")]
    public Type? EnumType { get; private set; }

    public EnumBindingSourceExtension(Type? enumType)
    {
        EnumType = enumType;

        if (enumType == null)
        {
            ArgumentNullException.ThrowIfNull(enumType);
        }

        if (!enumType.IsEnum)
        {
            throw new ArgumentException("The type must be enum!");
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType == null)
        {
            throw new InvalidOperationException("The EnumType must be specified.");
        }

        ObservableCollection<EnumModel> enumModels = EnumType.GetEnumModels();

        List<Enum> list = new List<Enum>();

        for (int i = 0; i < enumModels.Count; i++)
        {
            if (enumModels[i].IsEnable)
            {
                list.Add(enumModels[i].Value);
            }
        }

        return list;

        // Type? actualEnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
        // Array enumValues = Enum.GetValues(actualEnumType);
        //
        // if (actualEnumType == EnumType)
        // {
        //     return enumValues;
        // }
        //
        // Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
        // enumValues.CopyTo(tempArray, 1);
        // return tempArray;
    }
}