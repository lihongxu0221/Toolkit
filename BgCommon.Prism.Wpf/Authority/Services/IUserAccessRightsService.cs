namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 用户直接访问权限管理服务接口.
/// 定义了对 UserAccessRights 实体的直接操作，用于授予/撤销用户对特定对象的精细化访问权限,
/// 作为基于角色的权限系统的补充.
/// </summary>
internal interface IUserAccessRightsService
{
    /// <summary>
    /// 检查用户的直接访问权限 (UserAccessRights).
    /// </summary>
    /// <param name="userId">要检查的用户.</param>
    /// <param name="objectType">要访问的对象类型.</param>
    /// <param name="objectId">要访问的对象ID.</param>
    /// <returns>返回一个枚举，指示授权结果 (授予、拒绝 或 回退到角色检查).</returns>
    Task<DirectAccessCheckResult> ValidateForDirectAccessAsync(long userId, RefObjectType objectType, long objectId);

    /// <summary>
    /// 获取用户的直接授权信息.
    /// - 如果用户未配置任何直接权限(处于默认角色模式), 则返回结果中的 Result 为 null.
    /// - 如果用户已配置直接权限(处于显式模式), 则返回一个包含其所有授权模块ID的HashSet (可能为空集).
    /// </summary>
    /// <param name="userId">要查询的用户ID.</param>
    /// <returns>一个包含授权模块ID的HashSet，或者在默认模式下为null.</returns>
    Task<ResponseResult<HashSet<long>?>> GetDirectlyGrantedModuleIdsAsync(long userId);

    /// <summary>
    /// 批量授予用户对多个对象的直接访问权限.
    /// </summary>
    /// <param name="userId">要授权的用户ID.</param>
    /// <param name="requests">要授予的权限请求列表.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>包含详细处理结果的响应.</returns>
    Task<ResponseResult<BatchGrantAccessResult>> GrantAccessBatchAsync(long userId, List<AccessRightRequest> requests, UserInfo operatorUser);

    /// <summary>
    /// 批量撤销用户的直接访问权限.
    /// </summary>
    /// <param name="accessRightIds">要撤销的 UserAccessRights 记录的ID列表.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> RevokeAccessBatchAsync(List<int> accessRightIds, UserInfo operatorUser);

    /// <summary>
    /// 授予用户对特定对象的直接访问权限.
    /// </summary>
    /// <param name="userId">要授权的用户ID.</param>
    /// <param name="refObjectType">被引用对象的类型 (如模块, 功能).</param>
    /// <param name="refObjectId">被引用对象的ID.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>操作结果，成功时包含新创建的权限实体.</returns>
    Task<ResponseResult<UserAccessRights>> GrantAccessAsync(long userId, RefObjectType refObjectType, long refObjectId, UserInfo operatorUser);

    /// <summary>
    /// 撤销用户对特定对象的直接访问权限.
    /// </summary>
    /// <param name="accessRightId">要撤销的 UserAccessRights 记录的ID.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> RevokeAccessAsync(int accessRightId, UserInfo operatorUser);

    /// <summary>
    /// 获取指定用户的所有直接访问权限记录.
    /// </summary>
    /// <param name="userId">要查询的用户ID.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>返回该用户的所有权限记录列表.</returns>
    Task<ResponseResult<List<UserAccessRights>>> GetAccessRightsForUserAsync(long userId, UserInfo operatorUser);

    /// <summary>
    /// 检查用户是否拥有对特定对象的直接访问权限.
    /// </summary>
    /// <param name="userId">用户ID.</param>
    /// <param name="refObjectType">被引用对象的类型.</param>
    /// <param name="refObjectId">被引用对象的ID.</param>
    /// <returns>如果用户有权限，返回成功的 ResponseResult；否则返回失败的 ResponseResult.</returns>
    Task<ResponseResult> CheckAccessAsync(long userId, RefObjectType refObjectType, long refObjectId);
}
