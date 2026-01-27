namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 提供系统参数的增删改查及权限过滤功能.
/// </summary>
public interface ISystemParameterService
{
    /// <summary>
    /// 【管理用】获取所有系统参数（不过滤权限）.
    /// </summary>
    /// <param name="operatorUser">当前登录的用户.</param>
    /// <param name="moduleCode">可选的模块编码，用于只筛选特定模块下的参数.</param>
    /// <returns>返回所有系统参数的列表.</returns>
    Task<ResponseResult<List<SystemParameter>>> GetAllAsync(UserInfo operatorUser, string? moduleCode = null);

    /// <summary>
    /// 【管理用】添加一个新参数及其约束.
    /// </summary>
    /// <param name="parameter">参数实例.</param>
    /// <param name="operatorUser">当前登录的用户.</param>
    /// <returns>返回被添加的参数实体.</returns>
    Task<ResponseResult<SystemParameter>> AddAsync(SystemParameter parameter, UserInfo operatorUser);

    /// <summary>
    /// 【管理用】更新一个参数及其约束.
    /// </summary>
    /// <param name="parameter">参数实例.</param>
    /// <param name="operatorUser">当前登录的用户.</param>
    /// <returns>返回被更新的参数实体.</returns>
    Task<ResponseResult<SystemParameter>> UpdateAsync(SystemParameter parameter, UserInfo operatorUser);

    /// <summary>
    /// 【管理用】删除一个参数.
    /// </summary>
    /// <param name="id">参数id.</param>
    /// <param name="operatorUser">当前登录的用户.</param>
    /// <returns>返回被删除的参数实体.</returns>
    Task<ResponseResult<SystemParameter>> DeleteAsync(long id, UserInfo operatorUser);

    /// <summary>
    /// 批量添加或更新参数及其关联的约束.
    /// 如果参数已存在，则根据指定字段判断是否更新.
    /// </summary>
    /// <param name="parameters">要处理的参数列表.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>返回一个包含详细处理结果的对象.</returns>
    Task<ResponseResult<BatchParameterImportResult>> AddOrUpdateBatchAsync(List<SystemParameter> parameters, UserInfo operatorUser);

    /// <summary>
    /// 获取指定参数所有约束.
    /// </summary>
    /// <param name="parameterId">要查询其约束的参数的ID.</param>
    /// <param name="operatorUser">执行此操作的用户.</param>
    /// <returns>返回该参数的所有约束的列表.</returns>
    Task<ResponseResult<List<ParameterConstraint>>> GetConstraintsAsync(long parameterId, UserInfo operatorUser);

    /// <summary>
    /// 为指定参数添加一个新的约束.
    /// </summary>
    /// <param name="parameterId">约束所属的参数ID.</param>
    /// <param name="constraint">要添加的约束实体 (其内部的ParameterId将被忽略).</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回包含新创建的约束实体的结果.</returns>
    Task<ResponseResult<ParameterConstraint>> AddConstraintAsync(long parameterId, ParameterConstraint constraint, UserInfo operatorUser);

    /// <summary>
    /// 更新一个已存在的约束.
    /// </summary>
    /// <param name="constraint">要添加的约束实体 (其内部的ParameterId将被忽略).</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回包含更新后约束实体的结果.</returns>
    Task<ResponseResult<ParameterConstraint>> UpdateConstraintAsync(ParameterConstraint constraint, UserInfo operatorUser);

    /// <summary>
    /// 删除一个参数约束.
    /// </summary>
    /// <param name="constraintId">要删除的约束Id.</param>
    /// <param name="operatorUser">执行此操作的管理员.</param>
    /// <returns>返回包含被删除约束实体的结果.</returns>
    Task<ResponseResult<ParameterConstraint>> DeleteConstraintAsync(long constraintId, UserInfo operatorUser);

    /// <summary>
    /// 根据当前用户，获取其所有【可见】的参数，并标记其是否可编辑.
    /// </summary>
    /// <param name="currentUser">当前登录的用户.</param>
    /// <param name="moduleCode">可选的模块编码，用于只筛选特定模块下的参数.</param>
    /// <returns>返回一个经过权限过滤和计算的参数列表.</returns>
    Task<ResponseResult<List<ParameterQueryResult>>> GetVisibleParametersForUserAsync(UserInfo? currentUser, string? moduleCode = null);
}