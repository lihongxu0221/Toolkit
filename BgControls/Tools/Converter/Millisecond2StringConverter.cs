namespace BgControls.Tools.Converter;

public class Millisecond2StringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double totalMilliseconds)
        {
            // 直接从毫秒创建 TimeSpan，更精确
            TimeSpan span = TimeSpan.FromMilliseconds(totalMilliseconds);

            // 使用 TimeSpan 的 TotalDays 属性来判断是否超过一天
            if (span.TotalDays >= 1)
            {
                // 正确的格式：d.hh:mm:ss
                // @ 符号让 \ 不再是转义符，所以 \: 和 : 效果一样
                // 为了清晰，我们直接用 d.hh:mm:ss 或 d\ hh\:mm\:ss
                return span.ToString(@"d\.hh\:mm\:ss");
            }
            else
            {
                // 如果不足一天，则不显示天数部分
                return span.ToString(@"hh\:mm\:ss");
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}