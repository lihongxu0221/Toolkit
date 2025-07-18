using DryIoc;
using System.Collections.Concurrent;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Threading.Tasks;
using TraceX = System.Diagnostics.Trace;

namespace BgLogger;

/// <summary>
/// 提供统一的日志记录静态方法，支持基于 LogSource 枚举或字符串源名的日志输出。
/// </summary>
public static class BgLogger
{
    private static ConcurrentDictionary<string, Logger> loggers = new ConcurrentDictionary<string, Logger>();

    /// <summary>
    /// 获取特定源的 NLog Logger 实例。
    /// </summary>
    /// <param name="source">日志源（LogSource 枚举）。</param>
    /// <returns>NLog Logger 实例。</returns>
    private static Logger GetLogger(BgLoggerSource source)
    {
        string keyNames = source.ToString();
        return GetLogger(keyNames);
    }

    /// <summary>
    /// 获取特定源的 NLog Logger 实例。
    /// </summary>
    /// <param name="sourceName">日志源 字符串。</param>
    /// <returns>NLog Logger 实例。</returns>
    private static Logger GetLogger(string sourceName)
    {
        return loggers.GetOrAdd(sourceName, LogManager.GetLogger(sourceName));
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(this BgLoggerSource source, string format, params object[] args)
    {
        GetLogger(source)?.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(this BgLoggerSource source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(this BgLoggerSource source, Exception ex)
    {
        GetLogger(source)?.Fatal(ex);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string source, string format, params object[] args)
    {
        GetLogger(source)?.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志。
    /// </summary>
    public static void Trace(string source, Exception ex)
    {
        GetLogger(source)?.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string source, string format, params object[] args)
    {
        GetLogger(source)?.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志。
    /// </summary>
    public static void Debug(string source, Exception ex)
    {
        GetLogger(source)?.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string source, string format, params object[] args)
    {
        GetLogger(source)?.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志。
    /// </summary>
    public static void Info(string source, Exception ex)
    {
        GetLogger(source)?.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string source, string format, params object[] args)
    {
        GetLogger(source)?.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志。
    /// </summary>
    public static void Warn(string source, Exception ex)
    {
        GetLogger(source)?.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string source, string format, params object[] args)
    {
        GetLogger(source)?.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志。
    /// </summary>
    public static void Error(string source, Exception ex)
    {
        GetLogger(source)?.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string source, string format, params object[] args)
    {
        GetLogger(source)?.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="source">日志源。</param>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string source, Exception ex, string format, params object[] args)
    {
        GetLogger(source)?.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志。
    /// </summary>
    public static void Fatal(string source, Exception ex)
    {
        GetLogger(source)?.Fatal(ex);
    }
}

/// <summary>
/// 日志记录接口
/// </summary>
public interface IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Trace(string format, params object[] args);

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Trace(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Trace(Exception ex);

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Debug(string format, params object[] args);

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Debug(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Debug(Exception ex);

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Info(string format, params object[] args);

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Info(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Info(Exception ex);

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Warn(string format, params object[] args);

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Warn(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Warn(Exception ex);

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static abstract void Error(string format, params object[] args);

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static abstract void Error(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Error(Exception ex);

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static abstract void Fatal(string format, params object[] args);

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static abstract void Fatal(Exception ex, string format, params object[] args);

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static abstract void Fatal(Exception ex);
}

/// <summary>
/// 运行日志
/// </summary>
public class LogRun : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.General.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.General.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.General.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.General.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.General.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.General.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.General.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.General.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.General.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.General.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.General.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.General.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.General.Fatal(ex);
    }
}

/// <summary>
/// 弹窗日志
/// </summary>
public class LogDialog : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.Popup.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.Popup.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.Popup.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.Popup.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.Popup.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.Popup.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.Popup.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.Popup.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.Popup.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.Popup.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.Popup.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Popup.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.Popup.Fatal(ex);
    }
}

/// <summary>
/// 视觉日志
/// </summary>
public class LogVision : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.Vision.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.Vision.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.Vision.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.Vision.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.Vision.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.Vision.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.Vision.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.Vision.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.Vision.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.Vision.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.Vision.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Vision.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.Vision.Fatal(ex);
    }
}

/// <summary>
/// 运控日志
/// </summary>
public class LogMotion : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.Motion.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.Motion.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.Motion.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.Motion.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.Motion.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.Motion.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.Motion.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.Motion.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.Motion.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.Motion.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.Motion.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Motion.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.Motion.Fatal(ex);
    }
}

/// <summary>
/// MES日志
/// </summary>
public class LogMES : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.MES.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.MES.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.MES.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.MES.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.MES.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.MES.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.MES.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.MES.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.MES.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.MES.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.MES.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.MES.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.MES.Fatal(ex);
    }
}

/// <summary>
/// 数据库日志
/// </summary>
public class LogDB : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.DataBase.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.DataBase.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.DataBase.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.DataBase.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.DataBase.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.DataBase.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.DataBase.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.DataBase.Fatal(ex);
    }
}

/// <summary>
/// 数据库日志
/// </summary>
public class LogHardware : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.Hardware.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.Hardware.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.Hardware.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.Hardware.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.Hardware.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.Hardware.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.Hardware.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.Hardware.Fatal(ex);
    }
}

/// <summary>
/// 实时生产日志
/// </summary>
public class LogProduct : IBgLogger
{
    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Trace(format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Trace(ex, format, args);
    }

    /// <summary>
    /// 记录 Trace 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Trace(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Trace(ex);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Debug(format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Debug(ex, format, args);
    }

    /// <summary>
    /// 记录 Debug 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Debug(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Debug(ex);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Info(format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Info(ex, format, args);
    }

    /// <summary>
    /// 记录 Info 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Info(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Info(ex);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Warn(format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Warn(ex, format, args);
    }

    /// <summary>
    /// 记录 Warn 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Warn(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Warn(ex);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Error(format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Error(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Error(ex, format, args);
    }

    /// <summary>
    /// 记录 Error 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Error(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Error(ex);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Fatal(format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    /// <param name="ex">异常对象（可选）。</param>
    /// <param name="format">日志消息 或格式化日志消息。</param>
    /// <param name="args">格式化参数。</param>
    public static void Fatal(Exception ex, string format, params object[] args)
    {
        BgLoggerSource.RealtimeProduction.Fatal(ex, format, args);
    }

    /// <summary>
    /// 记录 Fatal 级别日志（基于 LogSource 枚举）。
    /// </summary>
    public static void Fatal(Exception ex)
    {
        BgLoggerSource.RealtimeProduction.Fatal(ex);
    }
}