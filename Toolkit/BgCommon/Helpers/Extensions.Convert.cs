namespace BgCommon;

/// <summary>
/// 类型转换扩展.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 安全转换为字符串，去除两端空格，当值为 null时返回"".
    /// </summary>
    /// <param name="input">输入值.</param>
    public static string SafeString(this object input)
    {
        return input?.ToString()?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 转换为 bool.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static bool ToBool(this string obj)
    {
        return BgConvert.ToBool(obj);
    }

    /// <summary>
    /// 转换为可空 bool.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static bool? ToBoolOrNull(this string obj)
    {
        return BgConvert.ToBoolOrNull(obj);
    }

    /// <summary>
    /// 转换为 int.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static int ToInt(this string obj)
    {
        return BgConvert.ToInt(obj);
    }

    /// <summary>
    /// 转换为可空 int.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static int? ToIntOrNull(this string obj)
    {
        return BgConvert.ToIntOrNull(obj);
    }

    /// <summary>
    /// 转换为 long.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static long ToLong(this string obj)
    {
        return BgConvert.ToLong(obj);
    }

    /// <summary>
    /// 转换为可空 long.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static long? ToLongOrNull(this string obj)
    {
        return BgConvert.ToLongOrNull(obj);
    }

    /// <summary>
    /// 转换为 double.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static double ToDouble(this string obj)
    {
        return BgConvert.ToDouble(obj);
    }

    /// <summary>
    /// 转换为可空 double.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static double? ToDoubleOrNull(this string obj)
    {
        return BgConvert.ToDoubleOrNull(obj);
    }

    /// <summary>
    /// 转换为 decimal.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static decimal ToDecimal(this string obj)
    {
        return BgConvert.ToDecimal(obj);
    }

    /// <summary>
    /// 转换为可空 decimal.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static decimal? ToDecimalOrNull(this string obj)
    {
        return BgConvert.ToDecimalOrNull(obj);
    }

    /// <summary>
    /// 转换为日期.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static DateTime ToDateTime(this string obj)
    {
        return BgConvert.ToDateTime(obj);
    }

    /// <summary>
    /// 转换为可空日期.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static DateTime? ToDateTimeOrNull(this string obj)
    {
        return BgConvert.ToDateTimeOrNull(obj);
    }

    /// <summary>
    /// 转换为Guid.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static Guid ToGuid(this string obj)
    {
        return BgConvert.ToGuid(obj);
    }

    /// <summary>
    /// 转换为可空Guid.
    /// </summary>
    /// <param name="obj">数据.</param>
    public static Guid? ToGuidOrNull(this string obj)
    {
        return BgConvert.ToGuidOrNull(obj);
    }

    /// <summary>
    /// 转换为Guid集合.
    /// </summary>
    /// <param name="obj">数据,范例: "83B0233C-A24F-49FD-8083-1337209EBC9A,EAB523C6-2FE7-47BE-89D5-C6D440C3033A".</param>
    public static List<Guid> ToGuidList(this string obj)
    {
        return BgConvert.ToGuidList(obj);
    }

    /// <summary>
    /// 转换为Guid集合.
    /// </summary>
    /// <param name="obj">字符串集合.</param>
    public static List<Guid> ToGuidList(this IList<string> obj)
    {
        if (obj == null)
        {
            return new List<Guid>();
        }

        return obj.Select(t => t.ToGuid()).ToList();
    }
}