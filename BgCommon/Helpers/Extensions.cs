namespace BgCommon;

/// <summary>
/// 公共扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 安全获取值，当值为 null时，不会抛出异常
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="value">可空值</param>
    public static T SafeValue<T>(this T? value)
        where T : struct
    {
        return value ?? default;
    }

    /// <summary>
    /// 获取枚举值
    /// </summary>
    /// <param name="instance">枚举实例</param>
    public static int? Value(this System.Enum instance)
    {
        if (instance == null)
        {
            return null;
        }

        return GetValue(instance.GetType(), instance);
    }

    /// <summary>
    /// 获取枚举值
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    /// <param name="instance">枚举实例</param>
    public static TResult? Value<TResult>(this System.Enum instance)
    {
        if (instance == null)
        {
            return default;
        }

        return BgConvert.To<TResult>(Value(instance));
    }

    /// <summary>
    /// 转换为用分隔符连接的字符串
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="list">集合</param>
    /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
    /// <param name="separator">分隔符，默认使用逗号分隔</param>
    public static string Join<T>(this IEnumerable<T> list, string quotes = "", string separator = ",")
    {
        return Join(list, quotes, separator);
    }
}