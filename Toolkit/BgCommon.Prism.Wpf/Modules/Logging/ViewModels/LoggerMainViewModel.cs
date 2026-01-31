using BgCommon.Prism.Wpf.Modules.Logging.Models;
using BgCommon.Prism.Wpf.Modules.Logging.Services;
using BgCommon.Prism.Wpf.Modules.Logging.Views;

namespace BgCommon.Prism.Wpf.Modules.Logging.ViewModels;

/// <summary>
/// LoggerMainViewModel.
/// </summary>
public partial class LoggerMainViewModel : DialogViewModelBase, INavigationAware
{
    // 通用应用日志记录器
    private readonly ILoggingConfigurationService configService;

    private bool hasInitialized;
    private bool isLoading;
    private string status = string.Empty;
    private LogSourceDisplay? _selectedLogSourceDisplay = null;
    private DisplayLevel? selectedLevel = null;
    private CancellationTokenSource? cancelLogTestToken = null;

    public bool IsLoading
    {
        get => isLoading;
        set => _ = SetProperty(ref isLoading, value);
    }

    /// <summary>
    /// Gets or sets 状态
    /// </summary>
    public string Status
    {
        get => status;
        set => _ = SetProperty(ref status, value);
    }

    /// <summary>
    /// Gets or sets 当前选中的日志源显示
    /// </summary>
    public LogSourceDisplay? SelectedLogSourceDisplay
    {
        get => _selectedLogSourceDisplay;
        set => SetProperty(ref _selectedLogSourceDisplay, value);
    }

    /// <summary>
    /// Gets or sets 当前选中的日志级别
    /// </summary>
    public DisplayLevel? SelectedLevel
    {
        get
        {
            return selectedLevel;
        }

        set
        {
            if (SetProperty(ref selectedLevel, value))
            {
                SelectedLogSourceDisplay?.ApplyFilter(selectedLevel?.Name ?? "All");
            }
        }
    }

    /// <summary>
    /// Gets 日志源显示的集合
    /// </summary>
    public ObservableCollection<LogSourceDisplay> LogSources { get; } = new ObservableCollection<LogSourceDisplay>();

    /// <summary>
    /// Gets 日志级别的集合
    /// </summary>
    public ObservableCollection<DisplayLevel> AllLevels { get; } = new ObservableCollection<DisplayLevel>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerMainViewModel"/> class.
    /// MainWindowViewModel 构造函数
    /// </summary>
    public LoggerMainViewModel(IContainerExtension container)
        : base(container)
    {
        configService = Container.Resolve<ILoggingConfigurationService>();

        // 订阅日志条目发布事件
        _ = EventAggregator?.GetEvent<PublishLogEntryEvent>().Subscribe(HandleLogPublished, ThreadOption.PublisherThread);
        _ = EventAggregator?.GetEvent<LogConfigurationChangedEvent>().Subscribe(OnLogConfigurationChanged);
    }

    /// <summary>
    /// 从配置加载日志源显示
    /// </summary>
    private void InitialAsync()
    {
        IEnumerable<LogSourceSetting> settings = configService.GetLogSourceSettings();
        IList<LogSourceDisplay> list = new List<LogSourceDisplay>();
        foreach (LogSourceSetting setting in settings)
        {
            if (setting.IsDisplayEnabled)
            {
                LogSourceDisplay? display = LogSources.FirstOrDefault(t => t.Name == setting.Name);
                if (display != null)
                {
                    display.Update(setting);
                    list.Add(display);
                }
                else
                {
                    list.Add(new LogSourceDisplay(setting));
                }
            }
        }

        list = list.OrderBy(t => t.Source).ToList();
        LogSources.Clear();
        LogSources.AddRange(list);
        list.Clear();
        if (LogSources.Any())
        {
            SelectedLogSourceDisplay = LogSources.First();
        }

        AllLevels.Clear();
        AllLevels.AddRange(DisplayLevel.GetAllLevels());
        if (AllLevels.Any())
        {
            SelectedLevel = AllLevels.FirstOrDefault();
        }

        Status = Ioc.GetString("状态初始化完成。请查看日志源");
    }

    /// <summary>
    /// 处理日志条目发布事件
    /// </summary>
    private void HandleLogPublished(List<LogEntry> logEntries)
    {
        if (!hasInitialized)
        {
            InitialAsync();
            hasInitialized = true;
        }

        if (logEntries == null || logEntries.Count == 0)
        {
            return;
        }

        Dictionary<string, LogEntry[]> logs = logEntries.GroupBy(t => t.Source).ToDictionary(t => t.Key, v => v.ToArray());
        // 根据日志源分组处理日志条目
        foreach (string source in logs.Keys)
        {
            LogSourceDisplay? sourceDisplay = LogSources.FirstOrDefault(s => s.Name.Equals(source, StringComparison.OrdinalIgnoreCase));
            if (sourceDisplay != null)
            {
                // 是否显示的检查在 AddLogEntry 内部
                sourceDisplay.AddLogEntry(logs[source]);
            }
            else
            {
                // 日志源未配置显示？可以动态添加或记录到“General”备用项
                Trace.TraceError($"Received log for unconfigured source '{source}'. Message: {logs[source].FirstOrDefault()?.Message}");
            }
        }
    }

    /// <summary>
    /// 处理日志配置更改，通过重新加载显示设置来响应。
    /// </summary>
    /// <remarks>
    /// 当日志配置发生更改时会触发此方法。它会更新显示设置以反映新的配置。
    /// 请确保在调用此方法前，日志配置已正确更新。
    /// </remarks>
    private void OnLogConfigurationChanged()
    {
        // 记录日志配置变更信息
        Status = GetString("日志配置已更改，正在重新加载显示设置");
        BgLoggerSource.General.Trace(Status);

        // 重新加载日志源显示
        InitialAsync();
    }

    /// <summary>
    /// 鼠标按下 开始执行测试日志操作
    /// </summary>
    [RelayCommand]
    private async void OnLogTest(LogSourceDisplay? source)
    {
        if (source == null)
        {
            BgLoggerSource.General.Warn("Test log source name is empty.");
            return;
        }

        // 通过源名称获取NLog日志记录器
        var logger = LogManager.GetLogger(source.Name);
        var random = new Random();
        cancelLogTestToken = new CancellationTokenSource();
        await Task.Run(async () =>
        {
            while (!cancelLogTestToken.IsCancellationRequested)
            {
                // 随机选择日志级别（Trace到Error）
                NLog.LogLevel level = NLog.LogLevel.FromOrdinal(random.Next(0, 5));

                logger.Log(level, $"This is a test log for {source.Name} at {DateTime.Now:HH:mm:ss.fff}");
                if (level == NLog.LogLevel.Error)
                {
                    try
                    {
                        throw new InvalidOperationException("Simulated test exception");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "This is a test error with exception.");
                    }
                }

                await Task.Delay(10).ConfigureAwait(true);
            }

            cancelLogTestToken?.Dispose();
            cancelLogTestToken = null;
        });
    }

    /// <summary>
    /// 鼠标弹起，取消日志输出
    /// </summary>
    [RelayCommand]
    private void OnLogTestCancel()
    {
        cancelLogTestToken?.Cancel();
    }

    /// <summary>
    /// 执行显示历史日志操作
    /// </summary>
    [RelayCommand]
    private void OnShowHistoricalLogs()
    {
        this.ShowDialog<HistoricalLogView>(nameof(LoggerDialogWindow), result =>
        {
            if (result.Result == ButtonResult.OK)
            {
                // Handle dialog close if needed
                // 如有需要，处理对话框关闭
            }
        });
    }

    /// <summary>
    /// 显示日志配置对话框。
    /// </summary>
    /// <remarks>此方法会打开一个用于配置日志设置的对话窗口。如果对话框以 OK 结果确认，则会异步重新加载日志源。</remarks>
    [RelayCommand]
    private void OnShowConfigDialog()
    {
        this.ShowDialog<LoggerConfigView>(nameof(LoggerDialogAutoSizeWindow), result =>
        {
            if (result.Result == ButtonResult.OK)
            {
            }
        });
    }

    /// <summary>
    /// 清除当前会话中的所有实时日志。
    /// </summary>
    /// <remarks>此方法会移除所有已累积的实时日志条目，重置日志状态。通常用于在开始新操作或会话前清空日志。</remarks>
    [RelayCommand]
    private void OnClearRealTimeLogs()
    {
        if (SelectedLogSourceDisplay != null)
        {
            SelectedLogSourceDisplay.RealTimeLogs.Clear();
            Status = $"[{SelectedLogSourceDisplay.Source.GetLocalizationString()}] 实时日志已清空";
            BgLoggerSource.General.Trace(Status);
        }
    }

    /// <summary>
    /// 显示日志条目的详细信息对话框。
    /// </summary>
    /// <param name="logEntry">要显示详细信息的日志条目。</param>
    [RelayCommand]
    private void OnShowDetailedLog(LogEntry? logEntry)
    {
        if (logEntry == null)
        {
            Status = "未选择日志条目，无法显示详细信息。";
            return;
        }

        this.ShowDialog<LoggerDetailView>(
            nameof(LoggerDialogAutoSizeWindow),
            callback: result =>
            {
                // 可根据需要处理对话框关闭后的逻辑
                if (result.Result == ButtonResult.OK)
                {
                    Status = "已查看日志详细信息。";
                }
            },
            parameters =>
            {
                // 打开详细日志对话框，传递选中的日志条目
                parameters.Add(nameof(LoggerDetailViewModel.CurrentEntry), logEntry);
                parameters.Add(nameof(LoggerDetailViewModel.AllLogEntries), SelectedLogSourceDisplay?.RealTimeLogs.ToList() ?? new List<LogEntry>());
            });
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        Title = GetString("实时日志");
        BgLoggerSource.General.Trace("MainView dialog opened.");
    }

    /// <summary>
    /// 决定是否重用现有实例
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual bool IsNavigationTarget(NavigationContext context)
    {
        return true;
    }

    /// <summary>
    /// 导航离开当前视图前执行
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual void OnNavigatedFrom(NavigationContext context)
    {
        context.Parameters.Add(GetType().Name, this);
    }

    /// <summary>
    /// 导航到当前视图后执行
    /// </summary>
    /// <param name="context">导航上下文</param>
    public virtual void OnNavigatedTo(NavigationContext context)
    {
        context.Parameters.Add(GetType().Name, this);
        BgLoggerSource.General.Trace("MainView navigated to.");
    }
}