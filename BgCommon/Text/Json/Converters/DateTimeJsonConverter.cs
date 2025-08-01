namespace BgCommon.Text.Json.Converters;

/// <summary>
/// 日期格式Json转换器
/// </summary>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// 日期格式
    /// </summary>
    private readonly string _format;

    /// <summary>
    /// 初始化日期格式Json转换器
    /// </summary>
    public DateTimeJsonConverter()
        : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    /// <summary>
    /// 初始化日期格式Json转换器
    /// </summary>
    /// <param name="format">日期格式,默认值: yyyy-MM-dd HH:mm:ss</param>
    public DateTimeJsonConverter(string format)
    {
        _format = format;
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return Extensions.ToLocalTime(BgConvert.ToDateTime(reader.GetString()));
        }

        if (reader.TryGetDateTime(out DateTime date))
        {
            return Extensions.ToLocalTime(date);
        }

        return DateTime.MinValue;
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        string date = Extensions.ToLocalTime(value).ToString(_format);
        writer.WriteStringValue(date);
    }
}