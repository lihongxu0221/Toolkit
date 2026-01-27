namespace BgCommon.DbService.Services;

/// <summary>
/// 登陆UI相关服务接口.
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// Gets or sets 当前已认证的用户.
    /// 如果为 null, 表示没有用户登录.
    /// </summary>
    UserInfo? CurrentUser { get; set; }

    /// <summary>
    /// Gets a value indicating whether 检查当前是否有用户已登录.
    /// </summary>
    bool IsLoggedIn { get; }

    /// <summary>
    /// 手动登陆.
    /// </summary>
    /// <returns>Task.</returns>
    Task<ResponseResult<UserInfo>> ShowLoginDialogAsync();

    /// <summary>
    /// 尝试进行自动登录.
    /// </summary>
    /// <returns>Task.</returns>
    Task<ResponseResult<UserInfo>> TryAutoLoginAsync();

    /// <summary>
    /// 执行应用内切换用户的完整流程.<br/>
    /// 切换用户完成后，会通过事件聚合器广播 <see cref="UserSwitchedEvent"/> 事件.
    /// </summary>
    /// <returns>Task.</returns>
    Task<ResponseResult<UserInfo>> SwitchUserAsync();

    /// <summary>
    /// 以指定的系统角色身份进行自动登录.
    /// </summary>
    /// <param name="roleToLogin">要登录的系统角色身份 (例如 SystemRole.Operator).</param>
    /// <returns>如果成功，返回该角色的第一个可用用户的登录信息；否则返回失败结果.</returns>
    Task<ResponseResult> LoginAsSystemRoleAsync(SystemRole roleToLogin);

    /// <summary>
    /// 查找本地保存的登录信息.
    /// </summary>
    /// <param name="ipAddress">保留参数，用于未来按IP查找，当前版本忽略此参数.</param>
    /// <returns>返回找到的登录信息，如果不存在则返回 null.</returns>
    Task<ResponseResult<LoginInfo>> GetLoginInfoAsync(string? ipAddress = null);

    /// <summary>
    /// 在一次成功的手动登录后，统一处理登录信息的更新.
    /// </summary>
    /// <param name="previousInfo">上一次保存在本地的登录信息 (可能为 null).</param>
    /// <param name="newInfo">本次登录成功后，从对话框返回的包含用户选择的新信息.</param>
    /// <returns>Task.</returns>
    Task UpdateAfterManualLoginAsync(LoginInfo? previousInfo, LoginInfo newInfo);

    /// <summary>
    /// 保存或更新用户的登录信息.
    /// </summary>
    /// <param name="loginInfo">包含要保存信息的实体.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> SaveLoginInfoAsync(LoginInfo loginInfo);

    /// <summary>
    /// 根据用户名删除已保存的登录信息.
    /// </summary>
    /// <param name="userId">要清除登录信息的用户ID.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> DeleteLoginInfoAsync(long userId);

    /// <summary>
    /// 修改用户密码.
    /// </summary>
    /// <returns>Task.</returns>
    Task ShowChangePasswordViewAsync();
}