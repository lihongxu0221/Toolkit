namespace BgCommon.Text.Json.Converters;

/// <summary>
/// 可空日期格式Json转换器
/// </summary>
public class NullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    /// <summary>
    /// 日期格式
    /// </summary>
    private readonly string _format;

    /// <summary>
    /// 初始化可空日期格式Json转换器
    /// </summary>
    public NullableDateTimeJsonConverter()
        : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    /// <summary>
    /// 初始化可空日期格式Json转换器
    /// </summary>
    /// <param name="format">日期格式,默认值: yyyy-MM-dd HH:mm:ss</param>
    public NullableDateTimeJsonConverter(string format)
    {
        _format = format;
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string date = Extensions.ToLocalTime(value.Value).ToString(_format);
        writer.WriteStringValue(date);
    }
}