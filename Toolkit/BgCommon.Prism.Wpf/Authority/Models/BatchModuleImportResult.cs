namespace BgCommon.Prism.Wpf.Authority.Models;

/// <summary>
/// 封装了批量导入模块操作中，单个失败模块的详细信息.
/// </summary>
/// <param name="AttemptedModule">尝试添加的原始模块实体.</param>
/// <param name="FailureReason">添加失败的原因.</param>
public record FailedModuleImport(ModuleInfo AttemptedModule, string FailureReason);

/// <summary>
/// 封装了批量导入模块操作的完整结果.
/// </summary>
public class BatchModuleImportResult
{
    /// <summary>
    /// Gets 成功添加并已存入数据库的模块列表 (这些模块现在拥有了数据库生成的ID).
    /// </summary>
    public List<ModuleInfo> SuccessfullyAddedModules { get; } = new();

    /// <summary>
    /// Gets 在导入过程中被更新的模块列表 (这些模块可能已经存在并进行了更新).
    /// </summary>
    public List<ModuleInfo> SuccessfullyUpdatedModules { get; } = new();

    /// <summary>
    /// Gets 在导入过程中被跳过的模块列表 (可能是因为它们已经存在或不需要导入).
    /// </summary>
    public List<ModuleInfo> SkippedModules { get; } = new();

    /// <summary>
    /// Gets 因失败而未能添加或修改的模块列表及其失败原因.
    /// </summary>
    public List<FailedModuleImport> FailedModules { get; } = new();

    /// <summary>
    /// Gets 本次操作尝试添加的模块总数.
    /// </summary>
    public int TotalAttempted => SuccessfullyAddedModules.Count + SuccessfullyUpdatedModules.Count + SkippedModules.Count + FailedModules.Count;

    /// <summary>
    /// Gets 成功添加的模块数量.
    /// </summary>
    public int SuccessCount => SuccessfullyAddedModules.Count + SuccessfullyUpdatedModules.Count;

    /// <summary>
    /// Gets 添加失败的模块数量.
    /// </summary>
    public int FailureCount => FailedModules.Count;

    /// <summary>
    /// Gets 跳过的模块数量 (这些模块没有被添加或更新).
    /// </summary>
    public int SkippedCount => SkippedModules.Count;
}