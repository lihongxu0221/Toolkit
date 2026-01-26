using BgCommon.Prism.Wpf.Modules.Logging.Models;

namespace BgCommon.Prism.Wpf.Modules.Logging.ViewModels;

/// <summary>
/// 日志详情视图模型，负责日志条目的导航与展示.
/// </summary>
internal partial class LoggerDetailViewModel : DialogViewModelBase
{
    /// <summary>
    /// Gets or sets 当前选中的日志条目.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private LogEntry? currentEntry = null;

    /// <summary>
    /// 异常信息是否显示
    /// </summary>
    [ObservableProperty]
    private Visibility exceptionVisibility = Visibility.Collapsed;

    /// <summary>
    /// 所有日志条目的列表.
    /// </summary>
    [ObservableProperty]
    private ListCollectionView? allLogEntries;

    /// <summary>
    /// 构造函数，初始化依赖注入容器.
    /// </summary>
    /// <param name="container">依赖注入容器.</param>
    public LoggerDetailViewModel(IContainerExtension container)
        : base(container)
    {
    }

    /// <summary>
    /// 对话框打开时的回调，初始化日志条目数据.
    /// </summary>
    /// <param name="parameters">对话框参数.</param>
    public override void OnDialogOpened(IDialogParameters parameters)
    {
        Title = GetString("日志信息");

        if (parameters.TryGetValue(nameof(AllLogEntries), out ListCollectionView? entries))
        {
            AllLogEntries = entries;
        }

        if (parameters.TryGetValue(nameof(CurrentEntry), out LogEntry? entry))
        {
            _ = AllLogEntries?.MoveCurrentTo(entry);
            SetCurrentEntry(entry);
        }

        if (CurrentEntry == null)
        {
            _ = AllLogEntries?.MoveCurrentToFirst();
            SetCurrentEntry((LogEntry?)entries?.CurrentItem);
        }
    }

    /// <summary>
    /// 切换到上一个日志条目.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecutePrevious))]
    private void OnPrevious()
    {
        if (AllLogEntries != null && CurrentEntry != null)
        {
            // 如果当前条目不是第一个，则切换到上一个条目
            if (AllLogEntries?.MoveCurrentToPrevious() ?? false)
            {
                SetCurrentEntry((LogEntry?)AllLogEntries?.CurrentItem);
            }
        }
    }

    /// <summary>
    /// 判断是否可以切换到上一个日志条目.
    /// </summary>
    /// <returns>如果可以切换返回 true，否则返回 false.</returns>
    private bool CanExecutePrevious()
    {
        if (AllLogEntries != null && CurrentEntry != null)
        {
            return AllLogEntries.CurrentPosition > 0;
        }

        return false;
    }

    /// <summary>
    /// 切换到下一个日志条目.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteNext))]
    private void OnNext()
    {
        if (AllLogEntries != null && CurrentEntry != null)
        {
            if (AllLogEntries?.MoveCurrentToNext() ?? false)
            {
                SetCurrentEntry((LogEntry?)AllLogEntries?.CurrentItem);
            }
        }
    }

    /// <summary>
    /// 判断是否可以切换到下一个日志条目.
    /// </summary>
    /// <returns>如果可以切换返回 true，否则返回 false.</returns>
    private bool CanExecuteNext()
    {
        if (AllLogEntries != null && CurrentEntry != null)
        {
            return AllLogEntries.CurrentPosition < (AllLogEntries.Count - 1);
        }

        return false;
    }

    private void SetCurrentEntry(LogEntry? entry)
    {
        CurrentEntry = entry;

        if (entry == null || entry.ExceptionInfo.IsEmpty())
        {
            ExceptionVisibility = Visibility.Collapsed;
        }
        else
        {
            ExceptionVisibility = Visibility.Visible;
        }
    }
}