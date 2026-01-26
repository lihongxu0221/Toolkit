namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 认证与校验服务.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登陆授权.
    /// </summary>
    /// <param name="userName">用户名.</param>
    /// <param name="password">用户密码.</param>
    /// <returns>是否成功登陆.</returns>
    Task<ResponseResult<UserInfo>> LoginAsync(string userName, string password);

    /// <summary>
    /// 注销登陆.
    /// </summary>
    /// <param name="userId">用户编号.</param>
    /// <returns>是否成功注销登陆.</returns>
    Task<ResponseResult> LogoutAsync(long userId);

    /// <summary>
    /// 以指定的系统角色身份的默认系统账号进行自动登录.
    /// </summary>
    /// <param name="roleToLogin">要登录的系统角色身份 (例如 SystemRole.Operator).</param>
    /// <returns>如果成功，返回该角色的第一个可用用户的登录信息；否则返回失败结果.</returns>
    Task<ResponseResult<UserInfo>> LoginAsSystemRoleAsync(SystemRole roleToLogin);

    /// <summary>
    /// 校验是否具有操作权限.
    /// </summary>
    /// <param name="operatorUserId">进行操作的用户编号.</param>
    /// <param name="requiredPermissionCode">操作代码.</param>
    /// <returns>返回 是否具有操作权限.</returns>
    Task<ResponseResult> VerifyPermissionAsync(long operatorUserId, string requiredPermissionCode);

    /// <summary>
    /// 校验当前用户是否有权限访问指定模块的特定操作.<br/>
    /// 当 actionCode 为空时, 使用 Authority 等级进行粗粒度访问校验.<br/>
    /// 当 actionCode 不为空时, 使用 Permission.Code 进行细粒度操作校验.<br/>
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <param name="moduleCode">模块唯一编码.</param>
    /// <param name="actionCode">操作的权限码 (对应 Permission.Code). 如果为空, 则检查模块访问权.</param>
    /// <returns>返回是否允许访问.</returns>
    Task<ResponseResult> VerifyAsync(UserInfo? operatorUser, string moduleCode, string? actionCode = "");
}