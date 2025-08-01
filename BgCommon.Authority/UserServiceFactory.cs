using BgCommon.Authority.Services;
using BgCommon.Authority.Services.Implementation;
using BgCommon.Authority.ViewModels;
using BgCommon.Authority.Views;

namespace BgCommon.Authority;

public class UserServiceFactory
{
    /// <summary>
    /// 注册依赖类型和服务到容器.<br/>
    /// Prism 应用程序的 RegisterTypes 方法通常在应用程序启动时调用.<br/>
    /// </summary>
    /// <param name="containerRegistry">容器注册器.</param>
    /// <param name="registryAction"> 额外的注册内容. </param>
    public static void Register(IContainerRegistry containerRegistry, Action<IContainerRegistry>? registryAction = null)
    {
        Register<UserService>(containerRegistry, registryAction);
    }

    /// <summary>
    /// 注册依赖类型和服务到容器。<br/>
    /// Prism 应用程序的 RegisterTypes 方法通常在应用程序启动时调用.<br/>
    /// </summary>
    /// <typeparam name="TUserService">用户服务类型.</typeparam>
    /// <param name="containerRegistry">容器注册器.</param>
    /// <param name="registryAction"> 额外的注册内容. </param>
    public static void Register<TUserService>(IContainerRegistry containerRegistry, Action<IContainerRegistry>? registryAction = null)
        where TUserService : class, IUserService
    {
        _ = containerRegistry.RegisterSingleton<IUserService, TUserService>();
        containerRegistry.RegisterDialogWindow<AuthorityDialogWindow>("BgCommon.Authority");
        containerRegistry.RegisterForNavigation<UserLoginView, UserLoginViewModel>();
        if (registryAction != null)
        {
            registryAction.Invoke(containerRegistry);
        }
    }
}