using BgControls.Data;
using hc = HandyControl.Controls;

namespace BgCommon.Prism.Wpf.Controls.Models;

/// <summary>
/// 代表一个输入参数的数据模型。这是一个纯粹的数据类，不包含UI逻辑。
/// </summary>
public class InputContent : ObservableObject
{
    // 定义一个委托用于简化 TryParse 的调用
    private delegate bool TryParseDelegate<T>(string s, out T result)
        where T : struct;

    private object? value = null;
    private string key = string.Empty;
    private TextType textType = TextType.Common;
    private double? minimum = null;
    private double? maximum = null;

    /// <summary>Gets 索引</summary>
    public int Index { get; }

    /// <summary>Gets 小数点后面的位数</summary>
    public int DecimalPlaces { get; }

    /// <summary>Gets or sets 标签/键</summary>
    public string Key
    {
        get => key;
        set => SetProperty(ref key, value);
    }

    /// <summary>Gets or sets 输入类型</summary>
    public TextType TextType
    {
        get => textType;
        set => SetProperty(ref textType, value);
    }

    /// <summary>Gets or sets 绑定的原始值</summary>
    public object? Value
    {
        get => value;
        set => SetProperty<object>(ref this.value, value);
    }

    public double? Minimum
    {
        get => minimum;
        set => SetProperty(ref minimum, value);
    }

    public double? Maximum
    {
        get => maximum;
        set => SetProperty(ref maximum, value);
    }

    #region Value Converters (类型转换属性)

    public string ValueString => Value?.ToString() ?? string.Empty;

    // 使用辅助方法来减少重复代码
    private T? TryParseValue<T>(TryParseDelegate<T> tryParse)
        where T : struct
    {
        return tryParse(ValueString, out T result) ? result : null;
    }

    public bool? ValueBool => TryParseValue<bool>(bool.TryParse);

    public int? ValueInt => TryParseValue<int>(int.TryParse);

    public uint? ValueUInt => TryParseValue<uint>(uint.TryParse);

    public double? ValueDouble => TryParseValue<double>(double.TryParse);

    public float? ValueFloat => TryParseValue<float>(float.TryParse);

    public short? ValueShort => TryParseValue<short>(short.TryParse);

    public ushort? ValueUShort => TryParseValue<ushort>(ushort.TryParse);

    public DateTime? ValueDateTime
    {
        get
        {
            // DateTime的解析稍有不同
            if (Value is DateTime dt)
            {
                return dt;
            }

            return DateTime.TryParse(ValueString, out DateTime parsedDt) ? parsedDt : null;
        }
    }

    #endregion

    public InputContent(InputContent item)
    {
        Index = item.Index;
        DecimalPlaces = item.DecimalPlaces;
        Key = item.Key;
        TextType = item.TextType;
        Value = item.Value;
        Minimum = item.Minimum;
        Maximum = item.Maximum;
    }

    public InputContent(int index, string key, TextType textType, object? content, int decimalPlaces = 0)
    {
        Index = index;
        Key = key;
        TextType = textType;
        DecimalPlaces = decimalPlaces;

        // 对初始值进行更安全的处理
        if (textType.IsNumeric()) // 假设有这样一个扩展方法判断是否为数字类型
        {
            double numericValue = 0;
            if (content != null)
            {
                _ = double.TryParse(content.ToString(), out numericValue);
            }

            Value = Math.Round(numericValue, decimalPlaces);

            // 设置边界
            if (textType.IsPositive()) // 假设有 IsPositive() 扩展方法
            {
                Minimum = 0;
            }
            else if (textType.IsNegative()) // 假设有 IsNegative() 扩展方法
            {
                Maximum = 0;
            }
        }
        else
        {
            Value = content;
        }
    }

    public override string ToString() => $"{Key}: {ValueString}";
}

// 建议增加一个TextType的扩展方法，方便判断
public static class TextTypeExtensions
{
    public static bool IsNumeric(this TextType textType)
    {
        return textType switch
        {
            TextType.Int or TextType.NInt or TextType.PInt or
            TextType.Short or TextType.NShort or TextType.PShort or
            TextType.Float or TextType.NFloat or TextType.PFloat or
            TextType.Double or TextType.PDouble or TextType.NDouble => true,
            _ => false,
        };
    }

    public static bool IsPositive(this TextType textType)
    {
        return textType is TextType.PInt or TextType.PShort or TextType.PFloat or TextType.PDouble;
    }

    public static bool IsNegative(this TextType textType)
    {
        return textType is TextType.NInt or TextType.NShort or TextType.NFloat or TextType.NDouble;
    }
}