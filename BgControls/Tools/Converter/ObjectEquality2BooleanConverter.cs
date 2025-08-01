namespace BgControls.Tools.Converter;

/// <summary>
/// 将值和参数进行比较是否相等的转换器
/// </summary>
public class ObjectEquality2BooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 处理 null 值情况
        if (value == null && parameter == null)
        {
            return true;
        }
        else if (value == null || parameter == null)
        {
            return false;
        }

        // 如果值和参数的类型不匹配，直接返回 false
        if (value.GetType() != parameter.GetType())
        {
            return false;
        }

        try
        {
            // 尝试将参数转换为绑定值的类型
            var convertedParameter = System.Convert.ChangeType(parameter, value.GetType(), culture);
            return value.Equals(convertedParameter);
        }
        catch
        {
            // 类型转换失败时比较字符串表示
            return value.ToString()!.Equals(parameter.ToString(), StringComparison.Ordinal);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked)
        {
            return parameter;
        }

        return Binding.DoNothing;
    }
}