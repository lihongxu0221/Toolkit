namespace BgCommon.Prism.Wpf.Modules.Logging.Models;

/// <summary>
/// 表示应用程序的设置，包括日志源的配置信息。
/// </summary>
public class AppSettings : ObservableObject
{
    private List<LogSourceSetting> settings = new List<LogSourceSetting>();

    /// <summary>
    /// Gets or sets 日志源的配置信息列表。
    /// </summary>
    public List<LogSourceSetting> LogSourceSettings
    {
        get => settings;
        set => _ = SetProperty(ref settings, value);
    }

    /// <summary>
    /// 清除配置信息
    /// </summary>
    public void Clear()
    {
        if (LogSourceSettings != null)
        {
            LogSourceSettings.Clear();
        }
    }
}