namespace BgControls.Tools.Converter;

public class Bit2CurrentMaxUnitStringConverter : IValueConverter
{
    // 支持的十进制单位（1 Kbit = 1000 bit）
    private static readonly string[] DecimalUnits = { "bit", "Kbit", "Mbit", "Gbit", "Tbit", "Pbit", "Ebit", "Zbit", "Ybit" };

    // 支持的二进制单位（1 Kibit = 1024 bit，可选）
    private static readonly string[] BinaryUnits = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && value is long bytes)
        {
            if (bytes < 0)
            {
                return "0 B";
            }

            int level = 0;
            double temp = bytes * 1.0d;
            while (temp > 1024)
            {
                temp /= 1024;
                level++;
            }

            return $"{Math.Round(temp, 2)} {BinaryUnits[level]}";
        }

        return "0 B";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}