namespace BgCommon.Prism.Wpf.Modules.Logging.Models;

/// <summary>
/// 可编辑的日志源设置视图模型.
/// 用于在界面上编辑日志源的显示与存储天数等配置.
/// </summary>
public class LogSourceSettingEditable : ObservableObject
{
    /// <summary>
    /// 原始日志源设置，用于判断是否有更改.
    /// </summary>
    private readonly LogSourceSetting originalSetting;
    private string name = string.Empty;
    private bool isDisplayEnabled = true;
    private int storageDays = 90;
    private int maxRealTimeEntries = 200; // 默认条
    private BgLoggerSource source = BgLoggerSource.UnKnowm;

    /// <summary>
    /// Gets or sets 日志源名称.
    /// </summary>
    public string Name
    {
        get => this.name;
        set => _ = this.SetProperty(ref this.name, value);
    }

    /// <summary>
    /// Gets or sets 日志源枚举.
    /// </summary>
    public BgLoggerSource Source
    {
        get => this.source;
        set => _ = this.SetProperty(ref this.source, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否显示日志源.
    /// </summary>
    public bool IsDisplayEnabled
    {
        get
        {
            return this.isDisplayEnabled;
        }

        set
        {
            if (this.SetProperty(ref this.isDisplayEnabled, value))
            {
                this.OnPropertyChanged(nameof(this.IsDirty));
            }
        }
    }

    /// <summary>
    /// Gets or sets 日志存储天数，最小为0天，最大为5年（1825天）.
    /// </summary>
    public int StorageDays
    {
        get => this.storageDays;
        set
        {
            // 最小0天（表示无限期或由应用逻辑处理）
            if (value < 0)
            {
                value = 0;
            }

            // 最大5年（1825天）
            if (value > 365 * 5)
            {
                value = 365 * 5;
            }

            if (this.SetProperty(ref this.storageDays, value))
            {
                this.OnPropertyChanged(nameof(this.IsDirty));
            }
        }
    }

    /// <summary>
    /// Gets or Sets 最大显示条数.
    /// </summary>
    public int MaxRealTimeEntries
    {
        get
        {
            return this.maxRealTimeEntries;
        }

        set
        {
            if (this.SetProperty(ref this.maxRealTimeEntries, value))
            {
                this.OnPropertyChanged(nameof(this.IsDirty));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether 判断当前设置是否已被修改.
    /// </summary>
    public bool IsDirty
    {
        get
        {
            return this.originalSetting.IsDisplayEnabled != this.IsDisplayEnabled ||
                   this.originalSetting.StorageDays != this.StorageDays ||
                   this.originalSetting.MaxRealTimeEntries != this.MaxRealTimeEntries;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogSourceSettingEditable"/> class.
    /// 构造函数，基于现有日志源设置初始化.
    /// </summary>
    /// <param name="setting">日志源设置.</param>
    public LogSourceSettingEditable(LogSourceSetting setting)
    {
        this.originalSetting = setting;
        this.Name = setting.Name;
        this.IsDisplayEnabled = setting.IsDisplayEnabled;
        this.StorageDays = setting.StorageDays;
        this.MaxRealTimeEntries = setting.MaxRealTimeEntries;
        if (Enum.TryParse(typeof(BgLoggerSource), this.name, true, out object? enumValue) && enumValue != null)
        {
            this.Source = (BgLoggerSource)enumValue;
        }
    }

    /// <summary>
    /// 转换为日志源设置对象.
    /// </summary>
    /// <returns>日志源设置.</returns>
    public LogSourceSetting ToLogSourceSetting()
    {
        return new LogSourceSetting
        {
            Name = this.Name,
            IsDisplayEnabled = this.IsDisplayEnabled,
            StorageDays = this.StorageDays,
            MaxRealTimeEntries = this.MaxRealTimeEntries,
        };
    }

    /// <summary>
    /// 重置为原始设置.
    /// </summary>
    public void Reset()
    {
        this.IsDisplayEnabled = this.originalSetting.IsDisplayEnabled;
        this.StorageDays = this.originalSetting.StorageDays;
        this.MaxRealTimeEntries = this.originalSetting.MaxRealTimeEntries;
        this.MaxRealTimeEntries = this.originalSetting.MaxRealTimeEntries;
        this.OnPropertyChanged(nameof(this.IsDirty)); // 通知IsDirty属性变化
    }

    /// <summary>
    /// 通知所有属性已更改.
    /// </summary>
    /// <param name="propertyName">要通知更改的属性名称.</param>
    public void NotifyPropertyChanged(string propertyName)
    {
        this.OnPropertyChanged(propertyName);
    }
}
