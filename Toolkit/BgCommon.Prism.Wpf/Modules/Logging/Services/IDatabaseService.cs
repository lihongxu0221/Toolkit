using BgCommon.Prism.Wpf.Modules.Logging.Models;

namespace BgCommon.Prism.Wpf.Modules.Logging.Services;

/// <summary>
/// 提供日志数据库相关操作的服务接口.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// 初始化数据库（如创建表结构等）.
    /// </summary>
    /// <returns>返回是否初始化成功.</returns>
    bool InitializeDatabase();

    /// <summary>
    /// 异步记录一条日志条目.NLog 会通过其目标调用此方法.
    /// </summary>
    /// <param name="entry">要记录的日志条目.</param>
    /// <returns>返回是否成功记录.</returns>
    Task<bool> LogAsync(LogEntry entry);

    /// <summary>
    /// 异步获取指定来源、时间范围内的日志条目.
    /// </summary>
    /// <param name="sourceName">日志来源名称.</param>
    /// <param name="startDate">起始时间.</param>
    /// <param name="endDate">结束时间.</param>
    /// <param name="limit">返回的最大日志条目数，默认为 1000.</param>
    /// <returns>日志条目列表.</returns>
    Task<List<LogEntry>> GetLogsAsync(string sourceName, DateTime startDate, DateTime endDate, int limit = 1000);

    /// <summary>
    /// 异步清理过期的日志条目.
    /// </summary>
    /// <param name="settings">日志来源的清理设置集合.</param>
    /// <param name="clearAll">是否清除全部(不按过滤条件来).</param>
    /// <returns>返回 清理结果.</returns>
    Task<bool> CleanupOldLogsAsync(IEnumerable<LogSourceSetting>? settings, bool clearAll);

    /// <summary>
    /// 异步获取历史日志文件大小.
    /// </summary>
    /// <returns>返回 历史日志文件大小.</returns>
    Task<long> GetHistoryFileSize();
}