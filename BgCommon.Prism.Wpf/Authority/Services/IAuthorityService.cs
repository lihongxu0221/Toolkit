using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;

namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 权限校验服务接口.
/// </summary>
public interface IAuthorityService : IDebugMode
{
    /// <summary>
    /// 通用的操作日志记录方法.
    /// </summary>
    /// <param name="userId">用户ID (可空).</param>
    /// <param name="username">用户名.</param>
    /// <param name="actionType">操作类型.</param>
    /// <param name="details">操作详情.</param>
    Task LogOperationAsync(long? userId, string username, string actionType, string details);

    /// <summary>
    /// 用户登陆授权.
    /// </summary>
    /// <param name="userName">用户名.</param>
    /// <param name="password">用户密码.</param>
    /// <returns>是否成功登陆</returns>
    Task<AuthorityResult> LoginAsync(string userName, string password);

    /// <summary>
    /// 注销登陆
    /// </summary>
    /// <param name="userId">用户编号.</param>
    /// <returns>是否成功注销登陆</returns>
    Task<AuthorityResult> LogoutAsync(long userId);

    /// <summary>
    /// 注册用户
    /// </summary>
    /// <param name="userName">用户名.</param>
    /// <param name="password">用户密码.</param>
    /// <param name="role">角色.</param>
    /// <returns>是否成功注册用户.</returns>
    Task<AuthorityResult> RegisterAsync(string userName, string password, Role role);

    /// <summary>
    /// 更新用户信息.
    /// </summary>
    /// <param name="user">用户信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>是否成功更新用户信息.</returns>
    Task<AuthorityResult> UpdateUserInfoAsync(UserInfo user, UserInfo operatorUser);

    /// <summary>
    /// 删除用户.
    /// </summary>
    /// <param name="userId">用户编码.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>是否成功删除用户.</returns>
    Task<AuthorityResult> DeleteUserAsync(int userId, UserInfo operatorUser);

    /// <summary>
    /// 获取所有用户.
    /// </summary>
    /// <returns>返回用户列表.</returns>
    Task<AuthorityResult<List<UserInfo>>> GetAllUsersAsync();

    #region ModuleInfo Management APIs

    /// <summary>
    /// 获取全部模块列表.
    /// </summary>
    /// <param name="user">当前用户.</param>
    /// <returns>模块列表.</returns>
    Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAllModulesAsync(UserInfo user);

    /// <summary>
    /// 通过父模块ID查找子模块列表.
    /// </summary>
    /// <param name="user">当前用户.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <returns>子模块列表.</returns>
    Task<AuthorityResult<List<Entities.ModuleInfo>>> GetModulesAsync(UserInfo user, long? parentId = null);

    /// <summary>
    /// 校验当前用户是否有权限访问指定模块.
    /// </summary>
    /// <param name="user">当前用户.</param>
    /// <param name="moduleId">模块实例Id.</param>
    /// <param name="action">操作.</param>
    /// <returns>返回 是否权限访问.</returns>
    Task<AuthorityResult> VerifyAsync(UserInfo user, long moduleId, string? action = "");

    /// <summary>
    /// 获取系统中的所有模块列表（用于管理界面，非用户权限过滤）.
    /// </summary>
    /// <returns>返回包含所有模块的列表.</returns>
    Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAllSystemModulesAsync();

    /// <summary>
    /// 添加一个新模块.
    /// </summary>
    /// <param name="moduleToAdd">要添加的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> AddModuleAsync(Entities.ModuleInfo moduleToAdd, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已有的模块信息.
    /// </summary>
    /// <param name="moduleToUpdate">要更新的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> UpdateModuleAsync(Entities.ModuleInfo moduleToUpdate, UserInfo operatorUser);

    /// <summary>
    /// 删除一个模块.
    /// </summary>
    /// <param name="moduleIdToDelete">要删除的模块ID.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> DeleteModuleAsync(long moduleIdToDelete, UserInfo operatorUser);
    #endregion

    #region Role Management APIs

    /// <summary>
    /// 获取所有角色列表.
    /// </summary>
    /// <returns>返回包含所有角色的列表.</returns>
    Task<AuthorityResult<List<Role>>> GetAllRolesAsync();

    /// <summary>
    /// 添加一个新角色.
    /// </summary>
    /// <param name="role">要添加的角色信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> AddRoleAsync(Role role, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已有的角色信息.
    /// </summary>
    /// <param name="role">要更新的角色信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> UpdateRoleAsync(Role role, UserInfo operatorUser);

    /// <summary>
    /// 删除一个角色.
    /// </summary>
    /// <param name="roleId">要删除的角色ID.</param>
    /// <param name="operatorUser">当前操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> DeleteRoleAsync(int roleId, UserInfo operatorUser);

    #endregion
}