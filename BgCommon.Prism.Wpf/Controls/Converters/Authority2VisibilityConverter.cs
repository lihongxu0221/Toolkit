namespace BgCommon.Prism.Wpf.Controls.Converters;

/// <summary>
/// 权限校验转换器.
/// 如果绑定的用户角色权限 (value) 大于等于参数指定的角色权限 (parameter)，则可见，否则折叠.
/// </summary>
public class Authority2VisibilityConverter : IValueConverter
{
    /// <summary>
    /// 执行转换.
    /// </summary>
    /// <param name="value">绑定的值，应为当前用户的 <see cref="SystemRole"/>.</param>
    /// <param name="targetType">目标类型，应为 <see cref="Visibility"/>.</param>
    /// <param name="parameter">转换器参数，应为要求的最低 <see cref="SystemRole"/>.</param>
    /// <param name="culture">区域信息.</param>
    /// <returns>如果权限足够则返回 <see cref="Visibility.Visible"/>，否则返回 <see cref="Visibility.Collapsed"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 确保绑定的值和参数都不是null
        if (value == null || parameter == null)
        {
            return Visibility.Collapsed;
        }

        // 尝试将绑定的值转换为当前用户的角色
        if (!Enum.TryParse(value.ToString(), out SystemRole currentUserRole))
        {
            return Visibility.Collapsed;
        }

        // 尝试将参数转换为所需角色
        if (!Enum.TryParse(parameter.ToString(), out SystemRole requiredRole))
        {
            return Visibility.Collapsed;
        }

        // 比较角色枚举的整数值
        // 如果当前用户的角色等级大于或等于要求的角色等级，则可见
        if ((int)currentUserRole >= (int)requiredRole)
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 这是一个单向转换器，所以我们不需要实现ConvertBack
        throw new NotImplementedException();
    }
}
