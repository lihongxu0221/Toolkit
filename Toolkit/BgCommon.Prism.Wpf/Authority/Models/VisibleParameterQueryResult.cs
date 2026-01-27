using BgCommon.Prism.Wpf.Modules.Parameters.Models;

namespace BgCommon.Prism.Wpf.Authority.Models;

public record FailedParameterQuery(SystemParameterDisplay Parameter, string Reason);

public class VisibleParameterQueryResult
{
    /// <summary>
    /// Gets 获取处理失败的参数及其失败原因的列表.
    /// </summary>
    public List<FailedParameterQuery> Failed { get; } = new();

    /// <summary>
    /// Gets 获取成功新增的参数列表.
    /// </summary>
    public List<SystemParameterDisplay> Parameters { get; } = new();

    /// <summary>
    /// Gets 失败的总数.
    /// </summary>
    public int TotalCount => Parameters.Count;

    /// <summary>
    /// Gets 查询成功 (总数 - 失败数量) 的总数.
    /// </summary>
    public int SuccessCount => Parameters.Count - Failed.Count;

    /// <summary>
    /// Gets 失败的总数.
    /// </summary>
    public int FailureCount => Failed.Count;
}