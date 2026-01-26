using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.MVVM;

namespace BgCommon.Prism.Wpf.Authority.Modules.User.ViewModels;

/// <summary>
/// UserLoginViewModel.cs .
/// </summary>
public partial class UserLoginViewModel : DialogViewModelBase
{
    private readonly IAuthorityService authority;
    private readonly ObservableCollection<string> users = new ObservableCollection<string>();
    private bool isLoginSuccess = false;
    private string userName = string.Empty;
    private string password = string.Empty;
    private bool isAutoLogin = false;
    private bool isRemember = false;
    private bool isAllowAutoLogin = false;
    private bool isAllowRemember = false;
    private AuthorityResult loginResult = new AuthorityResult();
    private LoginParam? loginParam = null;

    /// <summary>
    /// Gets or sets 用户名
    /// </summary>
    public string UserName
    {
        get => userName;
        set => SetProperty(ref userName, value);
    }

    /// <summary>
    /// Gets or sets 密码
    /// </summary>
    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否自动登陆
    /// </summary>
    public bool IsAutoLogin
    {
        get => isAutoLogin;
        set => SetProperty(ref isAutoLogin, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否记住密码
    /// </summary>
    public bool IsRemember
    {
        get
        {
            return isRemember;
        }

        set
        {
            _ = SetProperty(ref isRemember, value);

            if (value == false)
            {
                isAutoLogin = false;
                OnPropertyChanged(nameof(IsAutoLogin));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否允许自动登陆
    /// </summary>
    public bool IsAllowAutoLogin
    {
        get => isAllowAutoLogin;
        set => SetProperty(ref isAllowAutoLogin, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否允许记住密码
    /// </summary>
    public bool IsAllowRemember
    {
        get => isAllowRemember;
        set => SetProperty(ref isAllowRemember, value);
    }

    /// <summary>
    /// Gets 可用用户列表
    /// </summary>
    public ObservableCollection<string> Users => users;

    public UserLoginViewModel(IContainerExtension container)
        : base(container)
    {
        authority = Container.Resolve<IAuthorityService>();
    }

    public override void OnDialogClosed()
    {
        if (!isLoginSuccess)
        {
            loginResult.Code = 0;
            loginResult.Message = GetString("用户取消登陆");
            this.OnRequestClose(
                ButtonResult.Cancel,
                keys =>
                {
                    // 取消时，回传原有的登陆参数.
                    keys.Add(Constraints.LoginParam, loginParam!);
                    keys.Add(Constraints.LoginResult, loginResult);
                });
        }
    }

    public override async void OnDialogOpened(IDialogParameters parameters)
    {
        Title = GetString("用户登录");
        AuthorityResult<List<UserInfo>> response = await authority.GetAllUsersAsync();
        if (response.Success && response.Result != null)
        {
            foreach (UserInfo user in response.Result)
            {
                if (user.IsActive)
                {
                    Users.Add(user.UserName);
                }
            }

            loginParam = parameters.GetValue<LoginParam>(Constraints.LoginParam);
            if (loginParam != null)
            {
                UserName = loginParam.UserName;
                Password = loginParam.Password;
                IsRemember = loginParam.IsRemember;
                IsAutoLogin = loginParam.IsAutoLogin;
                IsAllowAutoLogin = loginParam.IsAllowAutoLogin;
                IsAllowRemember = loginParam.IsAllowRemember;
            }

            if (string.IsNullOrEmpty(UserName) || !Users.Contains(UserName))
            {
                UserName = Users.FirstOrDefault() ?? string.Empty;
                Password = string.Empty;
            }
        }
    }

    [RelayCommand]
    private async Task OnLogin()
    {
        isLoginSuccess = false;
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            _ = await this.ErrorAsync(GetString("用户名或密码不能为空"));
            return;
        }

        loginResult = await authority.LoginAsync(UserName, Password);
        if (loginResult.Code == -1)
        {
            _ = await this.ErrorAsync(loginResult.Message);
            return;
        }

        isLoginSuccess = true;
        this.OnRequestClose(
            ButtonResult.OK,
            keys =>
            {
                if (loginParam != null)
                {
                    loginParam.IsAutoLogin = IsAutoLogin;
                    loginParam.IsRemember = IsRemember;
                    if (IsRemember)
                    {
                        loginParam.UserName = UserName;
                        loginParam.Password = Password;
                    }
                    else
                    {
                        loginParam.UserName = string.Empty;
                        loginParam.Password = string.Empty;
                    }
                }

                keys.Add(Constraints.LoginParam, loginParam!);
                keys.Add(Constraints.LoginResult, loginResult);
            });
    }
}
