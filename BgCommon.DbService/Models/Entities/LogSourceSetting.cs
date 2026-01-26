namespace BgCommon.Prism.Wpf.Modules.Logging.Models;

/// <summary>
/// 表示日志源的配置信息，包括名称、是否在界面显示以及本地存储天数.
/// 用于配置和管理不同日志源的显示与存储策略.
/// </summary>
public class LogSourceSetting : ObservableObject
{
    private string name = string.Empty;
    private bool isDisplayEnabled = true;
    private int storageDays = 180; // 默认六个月
    private int maxRealTimeEntries = 200; // 默认条

    /// <summary>
    /// Gets or Sets 日志源名称，对应 LogSource 枚举的字符串值.
    /// </summary>
    public string Name
    {
        get => name;
        set => _ = SetProperty(ref name, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否在界面上显示该日志源.
    /// </summary>
    public bool IsDisplayEnabled
    {
        get => isDisplayEnabled;
        set => _ = SetProperty(ref isDisplayEnabled, value);
    }

    /// <summary>
    /// Gets or Sets 日志的本地存储天数.
    /// </summary>
    public int StorageDays
    {
        get => storageDays;
        set => _ = SetProperty(ref storageDays, value);
    }

    /// <summary>
    /// Gets or Sets 最大显示条数.
    /// </summary>
    public int MaxRealTimeEntries
    {
        get => maxRealTimeEntries;
        set => _ = SetProperty(ref maxRealTimeEntries, value);
    }

    public override string ToString()
    {
        return $"{Name},{IsDisplayEnabled},{StorageDays},{MaxRealTimeEntries}";
    }

    public LogSourceSetting() { }

    public LogSourceSetting(LogSourceSetting s)
    {
        Name = s.Name;
        IsDisplayEnabled = s.IsDisplayEnabled;
        StorageDays = s.StorageDays;
        MaxRealTimeEntries = s.MaxRealTimeEntries;
    }
}