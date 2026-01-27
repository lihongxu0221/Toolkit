using BgCommon.DbService.Models.Dtos;
using BgCommon.DbService.Models.Entities;

namespace BgCommon.DbService.Services;

/// <summary>
/// 用户管理操作契约接口.
/// 定义用户管理相关的核心操作规范，包含用户注册、信息更新、账号删除、角色分配及用户数据查询等功能.
/// </summary>
/// <remarks>
/// 本接口提供异步化的用户及关联角色管理方法，适用于系统内用户全生命周期的管理场景.
/// 实现类需为每个操作强制执行对应的权限校验和数据合法性验证（如用户信息格式校验、操作人权限校验等）.
/// 所有修改用户数据的操作（如更新、删除、角色分配）均要求指定操作人信息，以支持操作审计追踪和权限管控场景.
/// 方法返回值统一使用 ResponseResult[T] 类型，包含操作成功状态、错误信息及业务数据，为调用方提供详细的执行结果反馈.
/// </remarks>
public interface IUserService
{
    /// <summary>
    /// 注册一个新用户，并同时分配角色和直接权限.
    /// </summary>
    /// <param name="userToRegister">要注册的用户信息（用户名、密码等）.</param>
    /// <param name="roleIds">要分配给该用户的角色ID列表.</param>
    /// <param name="accessRights">要授予的直接访问权限列表.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作成功后返回新创建的用户信息.</returns>
    Task<ResponseResult<UserInfo>> RegisterAsync(UserInfo userToRegister, List<int> roleIds, List<AccessRightRequest> accessRights, UserInfo operatorUser);

    /// <summary>
    /// 更新用户信息，并同步其角色和直接权限.
    /// </summary>
    /// <param name="userToUpdate">要更新的用户信息.</param>
    /// <param name="roleIds">用户【最终】应该拥有的角色ID列表.</param>
    /// <param name="accessRights">用户【最终】应该拥有的直接访问权限列表.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作成功后返回更新后的用户信息.</returns>
    Task<ResponseResult<UserInfo>> UpdateUserInfoAsync(UserInfo userToUpdate, List<int> roleIds, List<AccessRightRequest> accessRights, UserInfo operatorUser);

    /// <summary>
    /// 注册用户.
    /// </summary>
    /// <param name="userName">用户名.</param>
    /// <param name="password">用户密码.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>是否成功注册用户.</returns>
    Task<ResponseResult<UserInfo>> RegisterAsync(string userName, string password, UserInfo operatorUser);

    /// <summary>
    /// 更新用户信息.
    /// </summary>
    /// <param name="userToUpdate">用户信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>是否成功更新用户信息.</returns>
    Task<ResponseResult<UserInfo>> UpdateUserInfoAsync(UserInfo userToUpdate, UserInfo operatorUser);

    /// <summary>
    /// 删除用户.
    /// </summary>
    /// <param name="userIdToDelete">用户编码.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>是否成功删除用户.</returns>
    Task<ResponseResult> DeleteUserAsync(long userIdToDelete, UserInfo operatorUser);

    /// <summary>
    /// 获取所有用户.
    /// </summary>
    /// <returns>返回用户列表.</returns>
    Task<ResponseResult<List<UserInfo>>> GetAllUsersAsync();

    /// <summary>
    /// 根据角色ID获取拥有该角色的所有用户列表.
    /// </summary>
    /// <param name="roleId">角色ID.</param>
    /// <returns>返回一个包含所有匹配用户的列表.</returns>
    Task<ResponseResult<List<UserInfo>>> GetUsersByRoleAsync(int roleId);

    /// <summary>
    /// 为用户分配或更新角色.
    /// </summary>
    /// <param name="userId">要操作的用户ID.</param>
    /// <param name="roleIds">要分配给该用户的角色ID列表.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> AssignRolesToUserAsync(long userId, List<int> roleIds, UserInfo operatorUser);

    /// <summary>
    /// 修改用户密码.
    /// </summary>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>Task.</returns>
    Task ShowChangePasswordViewAsync(UserInfo operatorUser);

    /// <summary>
    /// 用户信息管理.
    /// </summary>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>Task.</returns>
    Task<ResponseResult> ShowUserViewAsync(UserInfo operatorUser);
}