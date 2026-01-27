using BgCommon.Prism.Wpf.Modules.Parameters.Models;

namespace BgCommon.Prism.Wpf.Modules.Parameters.Converters;

/// <summary>
/// 用于在设置最低权限时，通过MultiBinding传递参数的载体。
/// </summary>
public class RoleAssignmentParameter
{
    public ERole? Role { get; set; }

    public IEnumerable<SystemParameterDisplay>? Parameters { get; set; }
}

public class RoleAssignmentConverter : IMultiValueConverter
{
    /// <summary>
    /// 将来自绑定的多个值转换为一个目标对象。
    /// </summary>
    /// <param name="values">绑定源生成的值数组。第一个值应为 ERole，第二个应为 SystemParameterDisplay。</param>
    /// <param name="targetType">绑定目标属性的类型。</param>
    /// <param name="parameter">要使用的转换器参数。</param>
    /// <param name="culture">要在转换器中使用的区域性。</param>
    /// <returns>一个转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // 安全检查
        if (values == null || values.Length < 2)
        {
            return default;
        }

        // 第一个值是点击的 Role
        var role = values[0] as ERole;

        // 第二个值是 DataGrid.SelectedItems 集合，它是一个 IList
        var selectedItems = values[1] as System.Collections.IList;

        if (role == null || selectedItems == null)
        {
            return default;
        }

        // 使用 LINQ 的 OfType<T>() 方法安全地将 object 集合转换为强类型集合
        var parameters = selectedItems.OfType<SystemParameterDisplay>();

        // 创建并返回包含集合的参数对象
        return new RoleAssignmentParameter
        {
            Role = role,
            Parameters = parameters
        };
    }

    /// <summary>
    /// 将目标值转换回源值。我们这里是单向绑定，不需要实现。
    /// </summary>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // 这是用于双向绑定的，在命令参数场景下不需要，直接抛出异常即可。
        throw new NotImplementedException();
    }
}