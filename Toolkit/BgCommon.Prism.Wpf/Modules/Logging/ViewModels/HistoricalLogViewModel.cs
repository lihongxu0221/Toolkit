using BgCommon.Prism.Wpf.Modules.Logging.Models;
using BgCommon.Prism.Wpf.Modules.Logging.Services;
using BgCommon.Prism.Wpf.Modules.Logging.Views;

namespace BgCommon.Prism.Wpf.Modules.Logging.ViewModels;

/// <summary>
/// 历史日志对话框视图模型，负责管理历史日志的查询与显示。
/// </summary>
public partial class HistoricalLogViewModel : DialogViewModelBase
{
    /// <summary>
    /// 数据库服务，用于查询日志数据。
    /// </summary>
    private readonly IDatabaseService dbService;

    /// <summary>
    /// 日志配置服务，用于获取日志来源配置信息。
    /// </summary>
    private readonly ILoggingConfigurationService configService;

    private DateTime startDate = DateTime.Today.AddDays(-7);
    private DateTime endDate = DateTime.Now;
    private EnumModel? selectedSource = null;
    private DisplayLevel? selectedLevel = null;
    private LogEntry? selectedLogEntry = null;
    private ICollectionView? filterDatasource = null;

    /// <summary>
    /// Gets or sets 查询的起始日期。
    /// </summary>
    public DateTime StartDate
    {
        get => startDate;
        set => SetProperty(ref startDate, value);
    }

    /// <summary>
    /// Gets or sets 查询的结束日期。
    /// </summary>
    public DateTime EndDate
    {
        get => endDate;
        set => SetProperty(ref endDate, value);
    }

    /// <summary>
    /// Gets or sets 当前选中的日志来源。
    /// </summary>
    public EnumModel? SelectedSource
    {
        get => selectedSource;
        set => SetProperty(ref selectedSource, value);
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
                if (FilterDatasource != null)
                {
                    if (selectedLevel == null || string.IsNullOrWhiteSpace(selectedLevel.Name) || selectedLevel.Name.ToLowerInvariant() == "all")
                    {
                        FilterDatasource.Filter = null; // 清除过滤
                    }
                    else
                    {
                        string filter = selectedLevel.Name.ToLower();
                        FilterDatasource.Filter = item =>
                        {
                            if (item is LogEntry person)
                            {
                                // 在多个属性上进行过滤
                                return person.Level.ToLower().Contains(filter);
                            }

                            return false;
                        };
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets 当前选中的日志级别
    /// </summary>
    public LogEntry? SelectedLogEntry
    {
        get { return selectedLogEntry; }
        set { _ = SetProperty(ref selectedLogEntry, value); }
    }

    /// <summary>
    /// Gets 可用的日志来源集合。
    /// </summary>
    public ObservableCollection<EnumModel> AvailableSources { get; } = new ObservableCollection<EnumModel>();

    /// <summary>
    /// Gets 历史日志集合。
    /// </summary>
    public ObservableCollection<LogEntry> HistoricalLogs { get; } = new ObservableCollection<LogEntry>();

    /// <summary>
    /// Gets 日志级别的集合
    /// </summary>
    public ObservableCollection<DisplayLevel> AllLevels { get; } = new ObservableCollection<DisplayLevel>();

    /// <summary>
    /// Gets or sets a value indicating whether 是否正在加载数据。
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool isLoading = false;

    /// <summary>
    /// Gets or sets 状态
    /// </summary>
    [ObservableProperty]
    private string status = string.Empty;

    /// <summary>
    /// Gets 获取或设置经过筛选的日志源集合视图
    /// </summary>
    public ICollectionView? FilterDatasource
    {
        get
        {
            return filterDatasource;
        }

        private set
        {
            _ = SetProperty(ref filterDatasource, value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoricalLogViewModel"/> class.
    /// 构造函数，初始化依赖服务并加载日志来源。
    /// </summary>
    /// <param name="container">依赖注入容器。</param>
    public HistoricalLogViewModel(IContainerExtension container)
        : base(container)
    {
        dbService = container.Resolve<IDatabaseService>();
        configService = container.Resolve<ILoggingConfigurationService>();
        _ = InitializeAsync();

        FilterDatasource = CollectionViewSource.GetDefaultView(HistoricalLogs);
    }

    /// <summary>
    /// 加载可用的日志来源。
    /// </summary>
    private async Task<bool> InitializeAsync()
    {
        return await AsyncHelper.OnAsync((tcs) =>
        {
            AvailableSources.Add(new EnumModel() { Name = "All", Display = "All" }); // Option to search across all sources
            IEnumerable<LogSourceSetting> settings = configService.GetLogSourceSettings();
            foreach (LogSourceSetting setting in settings.OrderBy(s => s.Name))
            {
                if (Enum.TryParse(typeof(BgLoggerSource), setting.Name, true, out object? enumValue) && enumValue != null)
                {
                    AvailableSources.Add(((BgLoggerSource)enumValue).GetEnumModel());
                }
            }

            SelectedSource = AvailableSources.FirstOrDefault();

            AllLevels.Clear();
            _ = AllLevels.AddRange(DisplayLevel.GetAllLevels());
            SelectedLevel = AllLevels.FirstOrDefault();

        }).ConfigureAwait(true);

    }

    /// <summary>
    /// 执行搜索操作，查询指定时间范围内的日志条目。
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSearch))]
    private async Task OnSearchAsync()
    {
        IsLoading = true; // 设置加载状态为 true，表示正在加载
        HistoricalLogs.Clear(); // 清空历史日志集合
        try
        {
            // 调用数据库服务，获取指定来源和时间范围内的日志条目
            List<LogEntry> logs = await dbService.GetLogsAsync(
                SelectedSource == null ? "All" : SelectedSource.Name,
                StartDate,
                EndDate);

            // 将查询到的日志条目添加到集合中
            foreach (LogEntry log in logs)
            {
                HistoricalLogs.Add(log);
            }
        }
        catch (Exception ex)
        {
            // 适当记录此错误（例如，使用状态栏或通用日志记录器）
            // 这里只是简单地输出到控制台，作为演示
            Console.WriteLine($"Error searching logs: {ex.Message}");

            // 在实际应用中，应将此错误显示给用户或通过 NLog 记录
            var generalLogger = NLog.LogManager.GetLogger(BgLoggerSource.General.ToString());
            generalLogger.Error(ex, "Failed to search historical logs.");
        }
        finally
        {
            IsLoading = false; // 加载完成，重置加载状态
        }
    }

    /// <summary>
    /// 判断是否可以执行搜索命令。
    /// </summary>
    /// <returns>如果未处于加载状态，则返回 true。</returns>
    private bool CanExecuteSearch() => !IsLoading;

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
                parameters.Add(nameof(LoggerDetailViewModel.AllLogEntries), this.FilterDatasource);
            });
    }

    /// <summary>
    /// 对话框打开时的回调方法。
    /// </summary>
    /// <param name="parameters">对话框参数。</param>
    public override void OnDialogOpened(IDialogParameters parameters)
    {
        Title = "Historical Logs";
    }
}