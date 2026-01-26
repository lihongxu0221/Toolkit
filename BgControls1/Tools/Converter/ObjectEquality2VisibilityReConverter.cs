namespace BgControls.Tools.Converter;

/// <summary>
/// 将值和参数进行比较是否相等的转换器
/// </summary>
public class ObjectEquality2VisibilityReConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 处理 null 值情况
        if (value == null && parameter == null)
        {
            return Visibility.Collapsed;
        }
        else if (value == null || parameter == null)
        {
            return Visibility.Visible;
        }

        // 如果值和参数的类型不匹配，直接返回 false
        if (value.GetType() != parameter.GetType())
        {
            return Visibility.Visible;
        }

        try
        {
            // 尝试将参数转换为绑定值的类型
            var convertedParameter = System.Convert.ChangeType(parameter, value.GetType(), culture);
            if (value.Equals(convertedParameter))
            {
                return Visibility.Collapsed;
            }
        }
        catch
        {
            // 类型转换失败时比较字符串表示
            if (value.ToString()!.Equals(parameter.ToString(), StringComparison.Ordinal))
            {
                return Visibility.Collapsed;
            }
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return parameter;
        }

        return Binding.DoNothing;
    }
}