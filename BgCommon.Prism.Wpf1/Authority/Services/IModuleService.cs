using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;

namespace BgCommon.Prism.Wpf.Authority.Services;

public interface IModuleService
{
    /// <summary>
    /// 获取全部模块列表.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <returns>模块列表.</returns>
    Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAllSystemModulesAsync(UserInfo operatorUser);

    /// <summary>
    /// 添加一个新模块.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleToAdd">要添加的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> AddModuleAsync(Entities.ModuleInfo moduleToAdd, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已有的模块信息.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleToUpdate">要更新的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> UpdateModuleAsync(Entities.ModuleInfo moduleToUpdate, UserInfo operatorUser);

    /// <summary>
    /// 删除一个模块.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleIdToDelete">要删除的模块ID.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<AuthorityResult> DeleteModuleAsync(long moduleIdToDelete, UserInfo operatorUser);

    /// <summary>
    /// 普通用户基于权限获取模块的接口
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <returns>子模块列表.</returns>
    Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAuthorizedModulesAsync(UserInfo operatorUser, long? parentId = null);
}