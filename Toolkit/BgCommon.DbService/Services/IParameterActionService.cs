namespace BgCommon.DbService.Services;

/// <summary>
/// 参数操作服务.
/// </summary>
public interface IParameterActionService
{
    /// <summary>
    /// 指定参数在指定模块中对指定用户是否可见.
    /// </summary>
    /// <param name="parameterCode">参数名称.</param>
    /// <param name="userId">用户编码.</param>
    /// <param name="moduleCode">模块编码.</param>
    /// <returns>返回是否可见.</returns>
    ResponseResult IsVisible(string parameterCode, long userId, string moduleCode);

    /// <summary>
    /// 配置指定模块的指定类型的参数.
    /// </summary>
    /// <param name="moduleCode">模块编码.</param>
    /// <param name="request">待配置的参数实例(报考集合类型的).</param>
    /// <returns>返回配置结果.</returns>
    Task<ResponseResult> ConfigureAsync(string moduleCode, ParameterQueryRequest request);

    /// <summary>
    /// 根据当前用户，获取其所有【可见】的参数，并标记其是否可编辑.
    /// </summary>
    /// <param name="moduleCode">模块编码，用于只筛选特定模块下的参数.</param>
    /// <param name="request">ViewModel中的参数列表.</param>
    /// <returns>返回一个经过权限过滤和计算的参数列表.</returns>
    Task<ResponseResult<VisibleParameterQueryResult>> GetVisibleParametersAsync(string moduleCode, ParameterQueryRequest request);

    /// <summary>
    /// 设定目标参数值.
    /// </summary>
    /// <param name="request">目标参数实例.</param>
    /// <param name="parameter">源参数实例.</param>
    /// <returns>返回设定结果.</returns>
    ResponseResult<List<FailedParameterQuery>> TryUpdateFrom(ParameterQueryRequest request, params SystemParameterDisplay[] parameter);

    /// <summary>
    /// 设定目标参数值.
    /// </summary>
    /// <param name="request">目标参数实例.</param>
    /// <param name="parameter">源参数实例.</param>
    /// <returns>返回设定结果.</returns>
    ResponseResult<List<FailedParameterQuery>> TryUpdateTo(ParameterQueryRequest request, params SystemParameterDisplay[] parameter);

    /// <summary>
    /// 配置指定模块的指定类型的参数 (此方法主要用于保存参数值).
    /// </summary>
    /// <param name="moduleCode">模块编码.</param>
    /// <param name="parameters">待保存的参数列表(报考集合类型的).</param>
    /// <param name="request">待保存的ViewModel参数实例(报考集合类型的).</param>
    /// <returns>返回配置结果.</returns>
    Task<ResponseResult> SaveAsync(string moduleCode, List<SystemParameterDisplay> parameters, ParameterQueryRequest request);

    /// <summary>
    /// 获取指定模块的指定类型的参数.
    /// </summary>
    /// <typeparam name="TEntity">参数的类型.</typeparam>
    /// <param name="userId">用户编码.</param>
    /// <param name="moduleCode">模块编码.</param>
    /// <param name="entity">指定类型参数值.</param>
    /// <returns>返回指定类型关联参数结果.</returns>
    Task<ResponseResult<TEntity>> LoadForEntityAsync<TEntity>(long userId, string moduleCode, TEntity? entity = null)
        where TEntity : class, new();
}