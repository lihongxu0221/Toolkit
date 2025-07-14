using BgCommon.Wpf.Prism.MVVM;
using CommunityToolkit.Mvvm.Input;

namespace BgCommon.Prism.MVVM;

/// <summary>
/// Dialog ViewModel 基类.
/// </summary>
public abstract partial class DialogViewModelBase : ViewModelBase, IDialogAware
{
    private string title = string.Empty;
    private ButtonResult _buttonResult = ButtonResult.None;

    public string Title
    {
        get => title;
        set => _ = SetProperty(ref title, value);
    }

    protected ButtonResult Result
    {
        get => _buttonResult;
        set => _ = SetProperty(ref _buttonResult, value);
    }

    public DialogCloseListener RequestClose { get; }

    public DialogViewModelBase(IContainerExtension container)
        : base(container)
    {
    }

    /// <summary>
    /// 是否可关闭弹窗
    /// </summary>
    /// <returns>true:可以 false:否 </returns>
    public virtual bool CanCloseDialog()
    {
        return true;
    }

    /// <summary>
    /// 弹窗关闭时
    /// </summary>
    public virtual void OnDialogClosed()
    {
    }

    /// <summary>
    /// 弹窗显示出来
    /// </summary>
    /// <param name="parameters">参数</param>
    public abstract void OnDialogOpened(IDialogParameters parameters);

    /// <summary>
    /// Ok
    /// </summary>
    [RelayCommand(CanExecute = nameof(OnOkCanExecute))]
    private void Ok(object? paramter)
    {
        IDialogParameters keys = new DialogParameters();
        if (OnOK(paramter, ref keys))
        {
            Result = ButtonResult.OK;
            RequestClose.Invoke(keys, Result);
        }
    }

    /// <summary>
    /// Cancel
    /// </summary>
    [RelayCommand(CanExecute = nameof(OnCancelCanExecute))]
    private void Cancel(object? paramter)
    {
        IDialogParameters keys = new DialogParameters();
        if (OnCancel(paramter, ref keys))
        {
            Result = ButtonResult.Cancel;
            RequestClose.Invoke(keys, Result);
        }
    }

    /// <summary>
    /// Ignore
    /// </summary>
    [RelayCommand(CanExecute = nameof(OnIgnoreCanExecute))]
    private void Ignore(object? paramter)
    {
        IDialogParameters keys = new DialogParameters();
        if (OnIgnore(paramter, ref keys))
        {
            Result = ButtonResult.Ignore;
            RequestClose.Invoke(keys, Result);
        }
    }

    /// <summary>
    /// Retry
    /// </summary>
    [RelayCommand(CanExecute = nameof(OnRetryCanExecute))]
    private void Retry(object? paramter)
    {
        IDialogParameters keys = new DialogParameters();
        if (OnRetry(paramter, ref keys))
        {
            Result = ButtonResult.Retry;
            RequestClose.Invoke(keys, Result);
        }
    }

    /// <summary>
    /// No
    /// </summary>
    [RelayCommand(CanExecute = nameof(OnNoCanExecute))]
    private void No(object? paramter)
    {
        IDialogParameters keys = new DialogParameters();
        if (OnNo(paramter, ref keys))
        {
            Result = ButtonResult.No;
            RequestClose.Invoke(keys, Result);
        }
    }

    /// <summary>
    /// 请求关闭弹窗
    /// </summary>
    /// <param name="result">弹窗结果</param>
    /// <param name="configure">配置弹窗回调参数</param>
    protected void OnRequestClose(ButtonResult result, Action<IDialogParameters>? configure = null)
    {
        IDialogParameters keys = new DialogParameters();
        if (configure != null)
        {
            configure.Invoke(keys);
        }

        RequestClose.Invoke(keys, result);
    }

    protected virtual bool OnOK(object? paramter, ref IDialogParameters keys) => true;

    protected virtual bool OnCancel(object? paramter, ref IDialogParameters keys) => true;

    protected virtual bool OnIgnore(object? paramter, ref IDialogParameters keys) => true;

    protected virtual bool OnRetry(object? paramter, ref IDialogParameters keys) => true;

    protected virtual bool OnNo(object? paramter, ref IDialogParameters keys) => true;

    protected virtual bool OnOkCanExecute(object? paramter) => true;

    protected virtual bool OnCancelCanExecute(object? paramter) => true;

    protected virtual bool OnIgnoreCanExecute(object? paramter) => true;

    protected virtual bool OnRetryCanExecute(object? paramter) => true;

    protected virtual bool OnNoCanExecute(object? paramter) => true;
}
