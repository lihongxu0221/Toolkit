namespace BgCommon.Prism.Wpf.Modules.Logging.Models;

/// <summary>
/// 日志显示实例
/// </summary>
public partial class LogEntry : ObservableObject
{
    private long _id;
    private DateTime _timestamp;
    private string _source = string.Empty;
    private string _level = string.Empty;
    private string _loggerName = string.Empty;
    private string _message = string.Empty;
    private string _exceptionInfo = string.Empty;

    /// <summary>
    /// Gets or Sets 唯一标识。
    /// </summary>
    public long Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    /// <summary>
    /// Gets or Sets 时间戳。
    /// </summary>
    public DateTime Timestamp
    {
        get => _timestamp;
        set => SetProperty(ref _timestamp, value);
    }

    /// <summary>
    /// Gets or Sets 日志来源。
    /// </summary>
    public string Source
    {
        get
        {
            return _source;
        }

        set
        {
            if (SetProperty(ref _source, value))
            {
                OnPropertyChanged(nameof(SourceEnum));
            }
        }
    }

    /// <summary>
    /// Gets or Sets 日志级别。
    /// </summary>
    public string Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    /// <summary>
    /// Gets or Sets 记录器名称。
    /// </summary>
    public string LoggerName
    {
        get => _loggerName;
        set => SetProperty(ref _loggerName, value);
    }

    /// <summary>
    /// Gets or Sets 日志消息内容。
    /// </summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>
    /// Gets or Sets 异常信息。
    /// </summary>
    public string ExceptionInfo
    {
        get => _exceptionInfo;
        set => SetProperty(ref _exceptionInfo, value);
    }

    /// <summary>
    /// Gets 日志源枚举。
    /// </summary>
    public BgLoggerSource SourceEnum
    {
        get
        {

            if (Enum.TryParse(typeof(BgLoggerSource), Source, true, out object? enumValue) && enumValue != null)
            {
                return (BgLoggerSource)enumValue;
            }

            return BgLoggerSource.UnKnowm;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is LogEntry logEntry)
        {
            return Id == logEntry.Id &&
                   Timestamp == logEntry.Timestamp &&
                   string.Equals(Source, logEntry.Source, StringComparison.Ordinal) &&
                   string.Equals(Level, logEntry.Level, StringComparison.Ordinal) &&
                   string.Equals(LoggerName, logEntry.LoggerName, StringComparison.Ordinal) &&
                   string.Equals(Message, logEntry.Message, StringComparison.Ordinal) &&
                   string.Equals(ExceptionInfo, logEntry.ExceptionInfo, StringComparison.Ordinal);
        }

        return false;
    }
}