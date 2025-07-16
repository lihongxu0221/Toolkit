using BgCommon;
using BgCommon.Collections;
using BgCommon.Configuration;
using BgCommon.Prism.Wpf.Authority.Models;
using BgCommon.Prism.Wpf.Authority.Views;

namespace BgCommon.Prism.Wpf.Authority.Services.Implementation;

/// <summary>
/// 用户服务.
/// </summary>
internal class DefaultUserService : ObservableObject, IUserService
{
    private const string DataFile = "Configs//Users.dat";
    private const string LoginParamFile = "Configs//LoginParam.dat";

    private readonly IContainerExtension container;
    private readonly IDialogService dialogService;
    private UserInfo? currentUser = null;
    private ObservableRangeCollection<UserInfo> users = new ObservableRangeCollection<UserInfo>();
    private ConfigurationMgr<UserConfig>? mgr = null;
    private ConfigurationMgr<LoginParam>? loginParam = null;

    [Serializable]
    private class UserConfig
    {
        public UserConfig() { }

        public UserConfig(ObservableCollection<UserInfo> users)
        {
            Users = users ?? new ObservableCollection<UserInfo>();
        }

        public ObservableCollection<UserInfo> Users { get; set; } = new ObservableCollection<UserInfo>();

        /// <summary>
        /// 初始化默认用户数据.
        /// </summary>
        public void Initialize()
        {
            if (Users.Count == 0)
            {
                Users.Add(new UserInfo("操作员", "123", UserAuthority.Operator));
                Users.Add(new UserInfo("工程师", "8888", UserAuthority.Engineer));
                Users.Add(new UserInfo("管理员", "8888*", UserAuthority.Admin));
            }
        }
    }

    public ObservableRangeCollection<UserInfo> Users
    {
        get => users;
        set => _ = SetProperty(ref users, value);
    }

    public UserInfo? CurrentUser
    {
        get => currentUser;
        set => _ = SetProperty(ref currentUser, value);
    }

    public bool IsLogin => CurrentUser != null;

    /// <summary>
    /// Gets or sets a value indicating whether 是否处于调试模式.
    /// </summary>
    public bool IsDebugMode { get; set; }

    public DefaultUserService(IContainerExtension container)
    {
        this.container = container;
        this.dialogService = container.Resolve<IDialogService>();
        this.Users = new ObservableRangeCollection<UserInfo>();
    }

    public virtual async Task<bool> InitializeAsync()
    {
        // 1. 注入登录视图
        container.RegisterDialog<UserLoginView>();

        // 2. 初始化用户信息
        string dataFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, DataFile);
        mgr = new ConfigurationMgr<UserConfig>(dataFilePath, ConfigurationMgr<UserConfig>.SerializeMethod.Json);

        try
        {
            // 读取用户数据文件并反序列化为 ObservableCollection<UserInfo>
            mgr.LoadFromFile();
        }
        catch (Exception ex)
        {
            LogRun.Error(ex, "加载用户数据失败，使用默认用户数据");
        }
        finally
        {
            if (mgr.Entity == null)
            {
                mgr.Entity = new UserConfig();
                mgr.Entity.Initialize();
                mgr.SaveToFile();
            }

            if (mgr.Entity.Users == null || mgr.Entity.Users.Count == 0)
            {
                mgr.Entity.Initialize();
                mgr.SaveToFile();
            }

            Users.AddRange(mgr.Entity.Users!.ToArray());
        }

        // 3. 初始化登录参数
        loginParam = new ConfigurationMgr<LoginParam>(LoginParamFile, ConfigurationMgr<LoginParam>.SerializeMethod.Json);

        try
        {
            await loginParam.LoadFromFileAsync();
        }
        catch (Exception ex)
        {
            LogRun.Error(ex, "加载登陆参数失败，使用默认的登陆参数");
        }
        finally
        {
            if (loginParam.Entity == null)
            {
                loginParam.Entity = new LoginParam();
                await loginParam.SaveToFileAsync();
            }
        }

        return true;
    }

    protected virtual void SaveUsersToFile()
    {
        if (mgr == null)
        {
            string dataFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, DataFile);
            mgr = new ConfigurationMgr<UserConfig>(dataFilePath, ConfigurationMgr<UserConfig>.SerializeMethod.Json);
            mgr.Entity = new UserConfig();
        }

        if (mgr.Entity == null)
        {
            mgr.Entity = new UserConfig();
        }

        // 更新用户数据
        mgr.Entity.Users = Users;
        mgr.SaveToFile();
    }

    public virtual async Task<LoginResult> ShowLoginViewAsync(bool isAllowAutoLoad = false)
    {
        LoginResult? loginResult = null;
        try
        {
            LoginParam? loginParam = this.loginParam?.Entity;
            if (loginParam == null)
            {
                loginParam = new LoginParam();
                this.loginParam = new ConfigurationMgr<LoginParam>(LoginParamFile, ConfigurationMgr<LoginParam>.SerializeMethod.Json);
                this.loginParam.Entity = loginParam;
                await this.loginParam.SaveToFileAsync();
            }

            loginParam.IsAllowAutoLogin = isAllowAutoLoad;
            if (!isAllowAutoLoad)
            {
                loginParam.IsAutoLogin = false;
            }

            // 未勾选自动登录时，显示登陆界面
            UserInfo? user = Users.FirstOrDefault(u => u.Authority == UserAuthority.Admin);
            if (IsDebugMode && user != null)
            {
                loginResult = await LoginAsync(user.UserName, user.Password);
            }
            else
            {
                IDialogResult? ret = await dialogService.ShowDialogAsync<UserLoginView>(
                    Constraints.LoginHostWindow,
                    keys =>
                    {
                        keys.Add(Constraints.LoginParam, loginParam!);
                    });
                if (ret != null)
                {
                    loginResult = ret.Parameters.GetValue<LoginResult>(Constraints.LoginResult);
                    if (ret.Exception != null)
                    {
                        LogRun.Error(ret.Exception);
                    }

                    try
                    {
                        LoginParam? loginParam1 = ret.Parameters.GetValue<LoginParam>(Constraints.LoginParam);
                        if (loginParam1 != null)
                        {
                            this.loginParam!.Entity = loginParam1;
                            await this.loginParam.SaveToFileAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogRun.Error(ex, "保存登陆参数出错！");
                    }
                }
            }

            return loginResult!;
        }
        catch (Exception ex)
        {
            LogRun.Error(ex);
            loginResult = new LoginResult();
            loginResult.Code = -1;
            loginResult.Message = ex.Message;
            return loginResult;
        }
    }

    public virtual async Task<LoginResult> ShowSwitchUserViewAsync()
    {
        LoginResult? loginResult = null;
        try
        {
            LoginParam loginParam = new LoginParam();
            loginParam.UserName = this.loginParam?.Entity?.UserName ?? string.Empty;
            loginParam.Password = this.loginParam?.Entity?.Password ?? string.Empty;
            loginParam.IsAllowAutoLogin = false;
            loginParam.IsAllowRemember = false;
            loginParam.IsAutoLogin = false;
            loginParam.IsRemember = false;

            IDialogResult? ret = await dialogService.ShowDialogAsync<UserLoginView>(
                 Constraints.LoginHostWindow,
                 keys => keys.Add(Constraints.LoginParam, loginParam));
            if (ret != null)
            {
                loginResult = ret.Parameters.GetValue<LoginResult>(Constraints.LoginResult);
                if (ret.Exception != null)
                {
                    LogRun.Error(ret.Exception);
                }
            }

            return loginResult!;
        }
        catch (Exception ex)
        {
            LogRun.Error(ex);
            loginResult = new LoginResult();
            loginResult.Code = -1;
            loginResult.Message = ex.Message;
            return loginResult;
        }
    }

    public virtual async Task<LoginResult> LoginAsync(string userName, string password)
    {
        LoginResult loginResult = new LoginResult();
        if (Users == null || Users.Count == 0)
        {
            loginResult.Code = -1;
            loginResult.Message = Ioc.GetString("UserService 未正常初始化, 或 UserService 未正常注入到容器");
            return loginResult;
        }

        UserInfo? user = Users.FirstOrDefault(u => u.DisplayName == userName && u.Password == password);
        if (user != null)
        {
            user.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            user.IsOnline = true;
            CurrentUser = user;

            loginResult.Code = 1;
            loginResult.Message = Ioc.GetString("成功登陆");
            return loginResult;
        }

        await Task.Delay(10);

        loginResult.Code = -1;
        loginResult.Message = Ioc.GetString($"用户名和密码不匹配，登陆失败！");
        return loginResult;
    }

    public virtual async Task<bool> LogoutAsync()
    {
        if (CurrentUser != null)
        {
            CurrentUser.IsOnline = false;
            CurrentUser = null;
        }

        await Task.Delay(10);
        return true;
    }

    public virtual async Task<bool> RegisterAsync(string userName, string password, UserAuthority authority)
    {
        UserInfo? user = Users.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
        {
            return false;
        }

        user = new UserInfo(userName, password, authority);
        Users.Add(user);
        await Task.Delay(10);
        return true;
    }

    public virtual async Task<bool> UpdateUserInfoAsync(UserInfo user)
    {
        UserInfo? userInfo = Users.FirstOrDefault(u => u.UserId == user.UserId);
        if (userInfo != null)
        {
            userInfo.UserName = user.UserName;
            userInfo.Password = user.Password;
            userInfo.Authority = user.Authority;

            // 保存更新后的用户数据
            mgr?.SaveToFile();
            return true;
        }

        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> DeleteUserAsync(string userId)
    {
        UserInfo? userInfo = Users.FirstOrDefault(u => u.UserId == userId);
        if (userInfo != null)
        {
            _ = Users.Remove(userInfo);

            // 保存更新后的用户数据
            mgr?.SaveToFile();
            return true;
        }

        await Task.Delay(10);
        return false;
    }

    /// <inheritdoc/>
    public virtual async Task<List<UserInfo>> GetAllUsersAsync()
    {
        return await AsyncHelper.OnAsync(() =>
        {
            return Users.ToList();
        }) ?? new List<UserInfo>();
    }
}
