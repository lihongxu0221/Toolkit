namespace BgCommon.DbService.Services;

/// <summary>
/// 参数授权服务.
/// </summary>
public interface IParameterPermissionService
{
    /// <summary>
    /// 获取某个角色的所有参数权限设置.
    /// </summary>
    /// <param name="roleId">角色Id.</param>
    /// <param name="operatorUser">操作用户.</param>
    /// <returns>返回操作结果.</returns>
    Task<ResponseResult<List<ParameterPermission>>> GetPermissionsForRoleAsync(int roleId, UserInfo operatorUser);

    /// <summary>
    /// 为一个角色添加一条新的参数权限.
    /// </summary>
    /// <param name="permission">要添加的权限实体，应包含 RoleId 和 ParameterId.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回操作结果.</returns>
    Task<ResponseResult<ParameterPermission>> AddPermissionAsync(ParameterPermission permission, UserInfo operatorUser);

    /// <summary>
    /// 更新某个角色的参数权限设置.
    /// </summary>
    /// <param name="roleId">角色Id.</param>
    /// <param name="permissions">角色参数权限.</param>
    /// <param name="operatorUser">操作用户.</param>
    /// <returns>返回操作结果.</returns>
    Task<ResponseResult> UpdatePermissionsForRoleAsync(int roleId, List<ParameterPermission> permissions, UserInfo operatorUser);

    /// <summary>
    /// 删除一条参数权限记录.
    /// </summary>
    /// <param name="permissionId">要删除的 ParameterPermission 记录的ID.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回操作结果.</returns>
    Task<ResponseResult> DeletePermissionAsync(long permissionId, UserInfo operatorUser);
}