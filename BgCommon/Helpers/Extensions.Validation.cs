namespace BgCommon;

/// <summary>
/// 验证扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 检测对象是否为 null,为 null则抛出<see cref="ArgumentNullException"/>异常
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="parameterName">参数名</param>
    public static void CheckNull(this object obj, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(parameterName);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this Guid value)
    {
        return value == Guid.Empty;
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty([NotNullWhen(false)] this Guid? value)
    {
        if (value == null)
        {
            return true;
        }

        return value == Guid.Empty;
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty<T>(this IEnumerable<T>? value)
    {
        if (value == null)
        {
            return true;
        }

        return !value.Any();
    }

    /// <summary>
    /// 是否默认值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="value">值</param>
    public static bool IsDefault<T>(this T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default);
    }
}