using BgCommon.Localization;
using BgCommon.Localization.Attributes;

namespace BgCommon;

/// <summary>
/// 枚举的扩展方法.
/// </summary>
public static partial class EnumModelExtensions
{
    /// <summary>
    /// 获取枚举值.
    /// </summary>
    /// <param name="instance">枚举实例.</param>
    public static int? Value(this System.Enum instance)
    {
        if (instance == null)
        {
            return null;
        }

        return GetValue(instance.GetType(), instance);
    }

    /// <summary>
    /// 获取枚举值
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    /// <param name="instance">枚举实例</param>
    public static TResult? Value<TResult>(this System.Enum instance)
    {
        if (instance == null)
        {
            return default;
        }

        return BgConvert.To<TResult>(Value(instance));
    }

    /// <summary>
    /// 获取枚举实体列表
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <returns>枚举实体列表</returns>
    public static ObservableCollection<EnumModel> GetEnumModels(this Type? enumType)
    {
        ObservableCollection<EnumModel> models = new ObservableCollection<EnumModel>();
        if (enumType == null)
        {
            ArgumentNullException.ThrowIfNull(nameof(enumType));
            return models;
        }

        if (!enumType.IsEnum)
        {
            throw new ArgumentException($"Type must be an Enum ,but is {enumType.Name}");
        }

        Array values = Enum.GetValues(enumType);
        foreach (Enum value in values)
        {
            EnumModel? model = value.GetEnumModel();
            if (model != null)
            {
                models.Add(model);
            }
        }

        return models;
    }

    /// <summary>
    /// 获取枚举实体类
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>枚举带多语言的实体</returns>
    public static EnumModel GetEnumModel(this Enum value)
    {
        EnumModel model = new EnumModel();
        model.Value = value;
        Type enumType = value.GetType();
        FieldInfo? field = enumType.GetField(value.ToString());
        if (field != null)
        {
            model.Name = field.Name;
            model.Display = string.Empty;
            model.Description = value.ToString();

            EnableAttribute? enable = field.GetCustomAttributes(typeof(EnableAttribute), false)
                                           .FirstOrDefault() as EnableAttribute;
            model.IsEnable = enable == null || enable.IsEnabled;

            ILocalizationProvider? provider = LocalizationProviderFactory.GetInstance();
            if (provider != null)
            {
                DisplayAttribute? attribute = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                                                   .FirstOrDefault() as DisplayAttribute;
                if (attribute != null)
                {
                    model.LangKey = attribute.Name;
                    model.Display = provider.GetString(attribute.Name)?.Replace("|", Environment.NewLine);
                    model.Description = provider.GetString(attribute.Description)?.Replace("|", Environment.NewLine);
                }
                else
                {
                    DisplayNameAttribute? attribute1 = field.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                                           .FirstOrDefault() as DisplayNameAttribute;
                    if (attribute1 != null)
                    {
                        model.LangKey = attribute1.DisplayName;
                        model.Display = provider.GetString(attribute1.DisplayName)?.Replace("|", Environment.NewLine);
                    }
                }
            }
        }

        return model;
    }

    /// <summary>
    /// 获取枚举对应的多语言文本
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>枚举对应的多语言文本</returns>
    public static string GetLocalizationString(this Enum value)
    {
        return value.GetEnumModel()?.Display ?? value.ToString();
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="member">成员名或值,范例:Enum1枚举有成员A=0,则传入"A"或"0"获取 Enum1.A</param>
    public static TEnum? Parse<TEnum>(object member)
    {
        string value = member.SafeString();
        if (string.IsNullOrWhiteSpace(value))
        {
            if (typeof(TEnum).IsGenericType)
            {
                return default;
            }

            throw new ArgumentNullException(nameof(member));
        }

        return (TEnum)System.Enum.Parse(Common.GetType<TEnum>(), value, true);
    }

    /// <summary>
    /// 获取成员名
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="member">成员名、值、实例均可,范例:Enum1枚举有成员A=0,则传入Enum1.A或0,获取成员名"A"</param>
    public static string GetName<TEnum>(object member)
    {
        return GetName(Common.GetType<TEnum>(), member);
    }

    /// <summary>
    /// 获取成员名
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="member">成员名、值、实例均可</param>
    public static string GetName(Type type, object member)
    {
        if (type == null)
        {
            return string.Empty;
        }

        if (member == null)
        {
            return string.Empty;
        }

        if (member is string value)
        {
            return value;
        }

        if (type.GetTypeInfo().IsEnum == false)
        {
            return string.Empty;
        }

        return System.Enum.GetName(type, member) ?? string.Empty;
    }

    /// <summary>
    /// 获取成员值
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="member">成员名、值、实例均可，范例:Enum1枚举有成员A=0,可传入"A"、0、Enum1.A，获取值0</param>
    public static int? GetValue<TEnum>(object member)
    {
        return GetValue(Common.GetType<TEnum>(), member);
    }

    /// <summary>
    /// 获取成员值
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="member">成员名、值、实例均可</param>
    public static int? GetValue(Type type, object member)
    {
        string value = member.SafeString();
        if (value.IsEmpty())
        {
            return null;
        }

        object result = System.Enum.Parse(type, value, true);
        return BgConvert.To<int?>(result);
    }

    /// <summary>
    /// 获取名称集合
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    public static List<string> GetNames<TEnum>()
    {
        return GetNames(typeof(TEnum));
    }

    /// <summary>
    /// 获取名称集合
    /// </summary>
    /// <param name="type">枚举类型</param>
    public static List<string> GetNames(Type type)
    {
        type = Common.GetType(type);
        if (type.IsEnum == false)
        {
            string errorMsg = LocalizationProviderFactory.GetString("TypeNotEnum", type);
            throw new InvalidOperationException(errorMsg);
        }

        var result = new List<string>();
        foreach (FieldInfo field in type.GetFields())
        {
            if (!field.FieldType.IsEnum)
            {
                continue;
            }

            result.Add(field.Name);
        }

        return result;
    }
}