using BgCommon.Prism.Wpf.Modules.Logging.Models;
using NLog.Common;
using NLog.Targets;

namespace BgCommon.Prism.Wpf.Modules.Logging;

/// <summary>
/// 一个NLog目标，将日志消息通过WPF的事件聚合器发布到UI.
/// </summary>
[Target(nameof(WpfEventAggregatorTarget))]
internal sealed class WpfEventAggregatorTarget : TargetWithContext // TargetWithLayout
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WpfEventAggregatorTarget"/> class.
    /// 构造函数.确保Layout有默认值，主要关注结构化数据.
    /// </summary>
    public WpfEventAggregatorTarget()
    {
        // 如果未配置Layout，则设置为简单的消息格式.
        // this.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:|${exception:format=tostring}}";
        this.Layout = "${message}";

        if (Ioc.EventAggregator == null)
        {
            NLog.Common.InternalLogger.Warn($"{nameof(WpfEventAggregatorTarget)}: EventAggregator is null. Log messages will not be published to UI.");
            return;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfEventAggregatorTarget"/> class.
    /// 构造函数，支持依赖注入事件聚合器.
    /// </summary>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    public WpfEventAggregatorTarget(IEventAggregator eventAggregator)
        : this()
    {
        if (Ioc.EventAggregator == null)
        {
            NLog.Common.InternalLogger.Warn($"{nameof(WpfEventAggregatorTarget)}: EventAggregator is null. Log messages will not be published to UI.");
            return;
        }
    }

    /// <summary>
    /// 写入日志事件.将日志通过事件聚合器发布到UI.
    /// </summary>
    /// <param name="logEvent">日志事件信息.</param>
    protected override void Write(LogEventInfo logEvent)
    {
        // var logEntry = new LogEntry
        // {
        //     Timestamp = logEvent.TimeStamp,
        //     Level = logEvent.Level.ToString(),
        //     Source = logEvent.LoggerName, // 日志来源
        //     Message = RenderLogEvent(Layout, logEvent), // 使用Layout渲染消息
        //     ExceptionInfo = logEvent.Exception?.ToString() ?? string.Empty,
        // };

        // // 现在，我们需要将这个消息安全地发给UI线程
        // // 使用Dispatcher是正确的做法
        // _ = Application.Current?.Dispatcher.InvokeAsync(() =>
        // {
        //     this._eventAggregator?.GetEvent<PubSubEvent<LogEntry>>().Publish(logEntry);
        // });
        PublishToUI(this.ToEntry(logEvent));
    }

    /// <summary>
    /// 批量写入方法.
    /// </summary>
    /// <param name="logEvents">日志事件信息.</param>
    protected override void Write(IList<AsyncLogEventInfo> logEvents)
    {
        if (logEvents == null || logEvents.Count == 0)
        {
            return;
        }

        // 将所有日志事件转换为LogEntry
        LogEntry[] messages = [.. logEvents.Select(e => this.ToEntry(e.LogEvent))];

        // 安全地将任务派发到UI线程
        PublishToUI(messages);
    }

    private LogEntry ToEntry(LogEventInfo logEvent)
    {
        try
        {
            string message = ToString(logEvent.Exception, logEvent.Message, logEvent.Parameters);
            if (logEvent.Level == NLog.LogLevel.Error ||
                logEvent.Level == NLog.LogLevel.Fatal)
            {
                Trace.TraceError(message);
            }
            else if (logEvent.Level == NLog.LogLevel.Warn)
            {
                Trace.TraceWarning(message);
            }
            else if (logEvent.Level == NLog.LogLevel.Info)
            {
                Trace.TraceInformation(message);
            }
            else if (logEvent.Level == NLog.LogLevel.Debug ||
                     logEvent.Level == NLog.LogLevel.Trace)
            {
                Trace.WriteLine(message);
            }

            return new LogEntry
            {
                Timestamp = logEvent.TimeStamp,
                Level = logEvent.Level.ToString(),
                Source = logEvent.LoggerName, // 日志来源
                Message = this.RenderLogEvent(this.Layout, logEvent), // 使用Layout渲染消息
                ExceptionInfo = logEvent.Exception?.ToString() ?? string.Empty,
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void PublishToUI(params LogEntry[] messages)
    {
        // // 通过事件聚合器发布一个包含多条日志的事件
        // this.eventAggregator?.GetEvent<PublishLogEntryEvent>().Publish(messages.ToList());

        // 安全地将任务派发到UI线程
        _ = Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            // 通过事件聚合器发布一个包含多条日志的事件
            Ioc.EventAggregator?.GetEvent<PublishLogEntryEvent>().Publish(messages.ToList());
        });
    }

    private static string ToString(Exception? ex, string format, params object[] args)
    {
        StringBuilder builder = new ();
        if (args != null && args.Length > 0)
        {
            try
            {
                _ = builder.AppendFormat(format, args);
                _ = builder.AppendLine();
            }
            catch (Exception)
            {
            }
        }
        else
        {
            _ = builder.AppendLine(format);
        }

        if (ex != null)
        {
            // 首先可以记录最外层的异常信息
            _ = builder.AppendLine(ex.ToString());

            // 然后遍历所有内部异常
            Exception? currentEx = ex; // 使用一个临时变量来遍历
            while ((currentEx = currentEx.InnerException) != null)
            {
                _ = builder.AppendLine("--- Inner Exception ---"); // 加上分隔符更清晰
                _ = builder.AppendLine(currentEx.ToString());
            }
        }

        return builder.ToString();
    }
}