using BgCommon.Prism.Wpf.Modules.Logging.Models;
using BgCommon.Prism.Wpf.Modules.Logging.Services;

namespace BgCommon.Prism.Wpf.Modules.Logging.ViewModels;

/// <summary>
/// 日志配置变更事件.
/// 用于通知日志配置视图模型日志配置已变更.
/// </summary>
public class LogConfigurationChangedEvent : PubSubEvent
{
}

/// <summary>
/// 日志配置视图模型.
/// 用于管理日志源设置的编辑、保存和重置操作.
/// </summary>
public partial class LoggerConfigViewModel : DialogViewModelBase
{
    /// <summary>
    /// 日志配置服务，用于获取和保存日志源设置.
    /// </summary>
    private readonly ILoggingConfigurationService configService;

    /// <summary>
    /// Gets or sets a value indicating whether 获取或设置是否正在保存配置.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool isSaving;

    /// <summary>
    ///  Gets 可配置的日志源设置集合.
    /// </summary>
    public ObservableCollection<LogSourceSettingEditable> ConfigurableSettings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerConfigViewModel"/> class.
    /// 构造函数，初始化日志配置视图模型.
    /// </summary>
    /// <param name="container">依赖注入容器.</param>
    public LoggerConfigViewModel(IContainerExtension container)
        : base(container)
    {
        this.configService = container.Resolve<ILoggingConfigurationService>();
        this.ConfigurableSettings = new ObservableCollection<LogSourceSettingEditable>();
        this.ConfigurableSettings.CollectionChanged += this.ConfigurableSettings_CollectionChanged;
        this.Intialize();
    }

    private void ConfigurableSettings_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                foreach (LogSourceSettingEditable item in e.NewItems)
                {
                    item.PropertyChanged += this.LogSourceSettingEditable_PropertyChanged;
                }
            }

            return;
        }

        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (LogSourceSettingEditable item in e.OldItems)
                {
                    item.PropertyChanged -= this.LogSourceSettingEditable_PropertyChanged;
                }
            }

            return;
        }
    }

    private void LogSourceSettingEditable_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LogSourceSettingEditable.IsDirty))
        {
            this.OkCommand.NotifyCanExecuteChanged();
            this.ResetCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// 加载当前日志源设置.
    /// </summary>
    private void Intialize()
    {
        this.ConfigurableSettings.Clear();
        IEnumerable<LogSourceSetting> settings = this.configService.GetLogSourceSettings();

        IList<LogSourceSettingEditable> list = [];
        foreach (LogSourceSetting setting in settings)
        {
            list.Add(new LogSourceSettingEditable(setting));
        }

        _ = this.ConfigurableSettings.AddRange(list.OrderBy(s => s.Source).ToArray());
    }

    /// <summary>
    /// 判断命令是否可执行.
    /// </summary>
    /// <returns>如果有更改且未在保存中，则返回true.</returns>
    private bool CanExecute()
    {
        return !this.IsSaving && this.ConfigurableSettings.Any(s => s.IsDirty);
    }

    /// <inheritdoc/>
    protected override bool OnOkCanExecute(object? parameter)
    {
        return !this.IsSaving && this.ConfigurableSettings.Any(s => s.IsDirty);
    }

    /// <summary>
    /// 保存日志源设置的命令.
    /// </summary>
    /// <param name="parameter">命令参数.</param>
    /// <param name="keys">对话框参数.</param>
    /// <returns>如果保存成功则返回true.</returns>
    protected override bool OnOK(object? parameter, ref IDialogParameters keys)
    {
        this.IsSaving = true;
        try
        {
            var settingsToSave = this.ConfigurableSettings.Select(s => s.ToLogSourceSetting()).ToList();
            if (this.configService.SaveLogSourceSettings(settingsToSave))
            {
                // 更新EditableLogSourceSetting中的原始设置，重置IsDirty
                foreach (LogSourceSettingEditable editableSetting in this.ConfigurableSettings)
                {
                    LogSourceSetting? saved = settingsToSave.FirstOrDefault(s => s.Name == editableSetting.Name);
                    if (saved != null)
                    {
                        editableSetting.IsDisplayEnabled = saved.IsDisplayEnabled; // 反映已保存状态
                        editableSetting.StorageDays = saved.StorageDays;

                        // 重新创建原始设置对象
                        LogSourceSetting editableSetting_originalSetting = new ()
                        {
                            Name = saved.Name,
                            IsDisplayEnabled = saved.IsDisplayEnabled,
                            StorageDays = saved.StorageDays,
                            MaxRealTimeEntries = saved.MaxRealTimeEntries,
                        };

                        editableSetting.NotifyPropertyChanged(nameof(LogSourceSettingEditable.IsDirty)); // 触发IsDirty属性重新计算
                    }
                }

                this.EventAggregator?.GetEvent<LogConfigurationChangedEvent>().Publish();
                return true;
            }
        }
        catch (Exception ex)
        {
            // 向用户显示错误（如通过消息框服务）
            // 此处仅记录或抛出异常
            BgLoggerSource.General.Error(ex, "保存日志配置时出错.");
        }
        finally
        {
            this.IsSaving = false;
        }

        return false;
    }

    /// <summary>
    /// 重置所有日志源设置为原始状态的命令.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void OnReset()
    {
        foreach (LogSourceSettingEditable setting in this.ConfigurableSettings)
        {
            setting.Reset();
        }

        // 重新评估保存和重置按钮的可执行状态
        this.OkCommand.NotifyCanExecuteChanged();
        this.ResetCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// 判断对话框是否可以关闭.
    /// </summary>
    /// <returns>如果未在保存中，则返回true.</returns>
    public override bool CanCloseDialog() => !this.IsSaving;

    /// <summary>
    /// 对话框打开时的回调.
    /// </summary>
    /// <param name="parameters">对话框参数.</param>
    public override void OnDialogOpened(IDialogParameters parameters)
    {
        this.Title = this.GetString("日志数据配置界面");
    }
}