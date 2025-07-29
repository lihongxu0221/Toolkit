using BgCommon.Collections;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.MVVM;

namespace BgCommon.Prism.Wpf.Authority.Modules.User.ViewModels;

/// <summary>
/// 用户新增和修改.
/// </summary>
public partial class UserInfoViewModel : DialogViewModelBase
{
    private UserInfo? curLoginUser = null;

    [ObservableProperty]
    private string mode = string.Empty;

    [ObservableProperty]
    private UserInfo user = new();

    /// <summary>
    /// 可用的角色列表.
    /// </summary>
    [ObservableProperty]
    private ObservableRangeCollection<Entities.Role> roles = new();

    [ObservableProperty]
    private bool isCreateNew = false;

    /// <summary>
    /// 确认密码.
    /// </summary>
    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public UserInfoViewModel(IContainerExtension container)
        : base(container)
    {
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        if (parameters.TryGetValue<UserInfo>(EditUser, out UserInfo? parameter2))
        {
            this.User = parameter2;
        }

        if (parameters.TryGetValue<string>(EditMode, out string? parameter1))
        {
            this.Mode = parameter1;
        }

        if (this.Mode == EditModeNew)
        {
            this.Title = GetString("新建用户");
            this.IsCreateNew = true;
        }

        if (this.Mode == EditModeUpdate)
        {
            this.Title = GetString("更新用户信息");
            this.IsCreateNew = false;
        }

        if (parameters.TryGetValue<UserInfo>(LoginedUser, out UserInfo? user))
        {
            this.curLoginUser = user;
        }
    }

    protected override bool OnOK(object? parameter, ref IDialogParameters keys)
    {
        bool result = true;
        string errorMessage = string.Empty;

        if (string.IsNullOrEmpty(this.User.UserName))
        {
            result = false;
            errorMessage = GetString("用户不能为空");
        }
        else if (string.IsNullOrEmpty(this.User.Password))
        {
            result = false;
            errorMessage = GetString("用户密码不能为空");
        }

        if (!IsCreateNew)
        {
            if (this.User.Password != this.ConfirmPassword)
            {
                result = false;
                errorMessage = GetString("俩次输入的密码不一致");
            }
        }

        // if (curLoginUser != null && curLoginUser.Authority < this.User.Authority)
        // {
        //     result = false;
        //     errorMessage = GetString("当前用户权限无法创建或修改高于自身权限的账号的权限等级");
        // }

        if (result == false)
        {
            _ = this.Error(errorMessage);
            return false;
        }

        keys.Add(Constraints.Result, new UserInfo(this.User));
        return base.OnOK(parameter, ref keys);
    }
}
