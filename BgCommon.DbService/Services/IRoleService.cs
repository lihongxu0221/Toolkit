using BgCommon.DbService.Models.Dtos;
using BgCommon.DbService.Models.Entities;

namespace BgCommon.DbService.Services;

/// <summary>
/// 角色服务接口.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 获取所有角色列表.
    /// </summary>
    /// <returns>返回包含所有角色的列表.</returns>
    Task<ResponseResult<List<Role>>> GetAllRolesAsync();

    /// <summary>
    /// 通过用户ID查找该用户的所有角色，并返回其最高权限等级.
    /// </summary>
    /// <param name="userId">要查询的用户ID.</param>
    /// <returns>一个包含角色列表和最高权限值的复合结果.</returns>
    Task<ResponseResult<UserRolesInfo>> GetRolesByUserIdAsync(long userId);

    /// <summary>
    /// 添加一个新角色.
    /// </summary>
    /// <param name="roleToAdd">要添加的角色信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> AddRoleAsync(Role roleToAdd, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已有的角色信息.
    /// </summary>
    /// <param name="roleToUpdate">要更新的角色信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> UpdateRoleAsync(Role roleToUpdate, UserInfo operatorUser);

    /// <summary>
    /// 删除一个角色.
    /// </summary>
    /// <param name="roleIdToDelete">要删除的角色ID.</param>
    /// <param name="operatorUser">当前操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult> DeleteRoleAsync(int roleIdToDelete, UserInfo operatorUser);
}