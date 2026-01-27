namespace BgCommon.DbService.Services;

/// <summary>
/// 模块管理服务接口.
/// 定义系统模块的核心操作规范，包含模块的查询、新增、编辑、删除、状态管理及关联权限处理等功能，
/// 为权限系统提供模块层面的统一数据访问和业务逻辑处理入口.
/// </summary>
public interface IModuleService
{
    /// <summary>
    /// 通过唯一的业务代码查找模块.
    /// </summary>
    /// <param name="code">唯一的业务代码.</param>
    /// <returns>
    /// 一个包含了详细成功与失败报告的 ResponseResult.<br/>
    /// Result 属性中包含了 ModuleInfo 对象.
    /// </returns>
    Task<ResponseResult<ModuleInfo>> GetModuleByCodeAsync(string code);

    /// <summary>
    /// 批量添加模块信息.<br/>
    /// 除非所有模块都添加失败，否则操作总体上被视为成功.
    /// </summary>
    /// <param name="modulesToAdd">要添加的模块信息列表.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>
    /// 一个包含了详细成功与失败报告的 ResponseResult.<br/>
    /// Result 属性中包含了 BatchModuleImportResult 对象.
    /// </returns>
    Task<ResponseResult<BatchModuleImportResult>> AddModulesBatchAsync(List<ModuleInfo> modulesToAdd, UserInfo? operatorUser);

    /// <summary>
    /// 获取全部模块列表.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <returns>模块列表.</returns>
    Task<ResponseResult<List<ModuleInfo>>> GetAllSystemModulesAsync(UserInfo operatorUser);

    /// <summary>
    /// 添加一个新模块.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleToAdd">要添加的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult<ModuleInfo>> AddModuleAsync(ModuleInfo moduleToAdd, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已有的模块信息.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleToUpdate">要更新的模块信息.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult<ModuleInfo>> UpdateModuleAsync(ModuleInfo moduleToUpdate, UserInfo operatorUser);

    /// <summary>
    /// 删除一个模块.<br/>
    /// 管理员使用的接口.
    /// </summary>
    /// <param name="moduleIdToDelete">要删除的模块ID.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>操作结果.</returns>
    Task<ResponseResult<ModuleInfo>> DeleteModuleAsync(long moduleIdToDelete, UserInfo operatorUser);

    /// <summary>
    /// 普通用户基于权限获取模块的接口.
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <returns>子模块列表.</returns>
    Task<ResponseResult<List<ModuleInfo>>> GetAuthorizedModulesAsync(UserInfo? operatorUser, long? parentId = null);

    /// <summary>
    /// 用户获取模块的接口（为防止误调，需要校验用户是否具有打开ModuleView的权限）.
    /// </summary>
    /// <param name="operatorUser">当前用户.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <returns>子模块列表.</returns>
    Task<ResponseResult<List<ModuleInfo>>> GetModulesAsync(UserInfo operatorUser, long? parentId = null);
}