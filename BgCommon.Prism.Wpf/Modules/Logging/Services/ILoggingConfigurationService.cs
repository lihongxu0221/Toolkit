using BgCommon.Prism.Wpf.Modules.Logging.Models;

namespace BgCommon.Prism.Wpf.Modules.Logging.Services;

/// <summary>
/// 提供日志配置相关服务的接口。
/// </summary>
public interface ILoggingConfigurationService
{
    /// <summary>
    /// 获取所有日志源的配置信息集合。
    /// </summary>
    /// <returns>日志源配置信息的枚举集合。</returns>
    IEnumerable<LogSourceSetting> GetLogSourceSettings();

    /// <summary>
    /// 根据日志源名称获取对应的配置信息。
    /// </summary>
    /// <param name="sourceName">日志源名称。</param>
    /// <returns>指定日志源的配置信息。</returns>
    LogSourceSetting GetSettingForSource(string sourceName);

    /// <summary>
    /// 异步保存日志源配置信息集合。
    /// </summary>
    /// <param name="settings">要保存的日志源配置信息集合。</param>
    /// <returns>保存操作是否成功的异步结果。</returns>
    bool SaveLogSourceSettings(IEnumerable<LogSourceSetting> settings);
}