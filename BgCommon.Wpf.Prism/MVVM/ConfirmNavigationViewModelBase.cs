namespace BgCommon.Wpf.Prism.MVVM;

/// <summary>
/// ConfirmNavigationViewModelBase.cs. <br/>
/// <code>触发时机  导航请求发起时</code>
/// <code>主要用途  确认是否允许导航（如取消未保存的更改）</code>
/// <code>关键方法  OnNavigatedTo、OnNavigatedFrom、ConfirmNavigationRequest</code>
/// <code>是否可取消导航   是（通过 continuationCallback(false)</code>
/// </summary>
public abstract partial class ConfirmNavigationViewModelBase : NavigationViewModelBase, IConfirmNavigationRequest
{
    protected ConfirmNavigationViewModelBase(IContainerExtension container)
        : base(container)
    {
    }

    /// <summary>
    /// 进行实际导航之前进行一些确认操作
    /// </summary>
    /// <param name="context">导航上下文</param>
    /// <param name="continuationCallback">回传是否支持导航</param>
    public void ConfirmNavigationRequest(NavigationContext context, Action<bool> continuationCallback)
    {
        bool ret = OnConfirmNavigationRequest(context);
        continuationCallback(ret);
    }

    /// <summary>
    /// 进行实际导航之前进行一些确认操作
    /// </summary>
    /// <param name="context">导航上下文</param>
    /// <returns>返回值 true 表示确认导航，false 表示取消导航</returns>
    protected virtual bool OnConfirmNavigationRequest(NavigationContext context)
    {
        return true;
    }
}