namespace BgControls.Tools.Converter;

public class TypeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Type type && type != null)
        {
            // 拼接类型名称和命名空间
            return type.FullName ?? type.Name;
        }
        else if (value is string str)
        {
            return str;
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}