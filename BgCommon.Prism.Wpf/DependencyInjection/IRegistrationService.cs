namespace BgCommon.Prism.Wpf.DependencyInjection;

/// <summary>
/// 注入服务接口.
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// 动态注入 当前加载的全部程序集，所有实现IResolver的接口类
    /// </summary>
    Task RegisterViewsAsync();

    /// <summary>
    /// 动态注入 指定程序集里，所有实现IResolver的接口类
    /// </summary>
    /// <param name="assembly">程序集</param>
    void RegisterViews(Assembly assembly);

    /// <summary>
    /// 动态注入
    /// </summary>
    /// <typeparam name="TView">待注入的类型</typeparam>
    /// <param name="viewName">视图名称</param>
    void RegisterView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(string? viewName = "")
        where TView : IRegistration;

    /// <summary>
    /// 注入指定类型，指定名称的视图
    /// </summary>
    /// <param name="type">IRegistration的实现</param>
    /// <param name="viewName">待注入的视图名称</param>
    public void RegisterView(Type type, string? viewName);
}