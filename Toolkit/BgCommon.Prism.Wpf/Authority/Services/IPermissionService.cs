namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 提供对系统中可用的操作权限（Permission）本身的增删改查管理功能.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// 获取所有已定义的权限项.
    /// </summary>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回所有权限的列表.</returns>
    Task<ResponseResult<List<Permission>>> GetAllAsync(UserInfo operatorUser);

    /// <summary>
    /// 添加一个新的权限项.
    /// </summary>
    /// <param name="permission">要添加的权限实体.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回包含新创建的权限实体的结果.</returns>
    Task<ResponseResult<Permission>> AddAsync(Permission permission, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已存在的权限项.
    /// </summary>
    /// <param name="permission">要更新的权限实体.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回包含更新后的权限实体的结果.</returns>
    Task<ResponseResult<Permission>> UpdateAsync(Permission permission, UserInfo operatorUser);

    /// <summary>
    /// 删除一个权限项.
    /// </summary>
    /// <param name="permissionId">要删除的权限ID.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult<Permission>> DeleteAsync(int permissionId, UserInfo operatorUser);
}