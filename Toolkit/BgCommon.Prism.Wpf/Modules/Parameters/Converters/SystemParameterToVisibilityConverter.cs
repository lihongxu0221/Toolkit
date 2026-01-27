using BgCommon.Prism.Wpf.Modules.Parameters.Models;

namespace BgCommon.Prism.Wpf.Modules.Parameters.Converters;

/// <summary>
/// SystemParameterDisplay属性到Visibility的转换器.
/// </summary>
public class SystemParameterToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SystemParameterDisplay display)
        {
            Type? propertyType = display.Descriptor?.PropertyType;
            if (propertyType != null && propertyType.IsNumberType())
            {
                if (nameof(display.Maximum).Equals(parameter) ||
                     nameof(display.Minimum).Equals(parameter) ||
                     nameof(display.Increment).Equals(parameter))
                {
                    return Visibility.Visible;
                }
                else if (nameof(display.DecimalPlaces).Equals(parameter))
                {
                    if (propertyType.IsFloatType(isBinaryFloat: true))
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
