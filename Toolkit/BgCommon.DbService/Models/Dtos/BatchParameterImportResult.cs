using BgCommon.DbService.Models.Entities;

namespace BgCommon.DbService.Models.Dtos;

public record FailedParameterImport(SystemParameter Parameter, string Reason);

/// <summary>
/// 封装了批量导入或更新系统参数操作的结果.
/// </summary>
public class BatchParameterImportResult
{
    /// <summary>
    /// Gets 获取成功新增的参数列表.
    /// </summary>
    public List<SystemParameter> SuccessfullyAdded { get; } = new();

    /// <summary>
    /// Gets 获取成功更新的参数列表.
    /// </summary>
    public List<SystemParameter> SuccessfullyUpdated { get; } = new();

    /// <summary>
    /// Gets 获取因数据无变化而跳过的参数列表.
    /// </summary>
    public List<SystemParameter> Skipped { get; } = new();

    /// <summary>
    /// Gets 获取处理失败的参数及其失败原因的列表.
    /// </summary>
    public List<FailedParameterImport> Failed { get; } = new();

    /// <summary>
    /// Gets a value indicating whether 操作是否包含任何成功或跳过的项.
    /// </summary>
    public bool HasSuccessOrSkipped => SuccessfullyAdded.Any() || SuccessfullyUpdated.Any() || Skipped.Any();

    /// <summary>
    /// Gets 成功处理 (新增 + 更新) 的总数.
    /// </summary>
    public int SuccessCount => SuccessfullyAdded.Count + SuccessfullyUpdated.Count;

    /// <summary>
    /// Gets 失败的总数.
    /// </summary>
    public int FailureCount => Failed.Count;
}