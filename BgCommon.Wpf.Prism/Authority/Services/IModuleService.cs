using ModuleInfo = BgCommon.Wpf.Prism.Authority.Models.ModuleInfo;

namespace BgCommon.Wpf.Prism.Authority.Services;

/// <summary>
/// 模块服务接口.
/// </summary>
public interface IModuleService
{
    /// <summary>
    /// 初始化模块服务.
    /// </summary>
    void Initialize();

    /// <summary>
    /// 获取全部模块列表.
    /// </summary>
    /// <returns>模块列表.</returns>
    IEnumerable<ModuleInfo> GetAllModules();

    /// <summary>
    /// 通过父模块ID查找子模块列表.
    /// </summary>
    /// <param name="parentId">父模块ID.</param>
    /// <returns>子模块列表.</returns>
    IEnumerable<ModuleInfo> GetModules(int? parentId = null);

    /// <summary>
    /// 校验当前用户是否有权限访问指定模块.
    /// </summary>
    /// <param name="module">模块实例.</param>
    /// <returns>返回 是否权限访问.</returns>
    bool Verify(ModuleInfo module);

    /// <summary>
    /// 校验当前用户是否有权限访问指定模块.
    /// </summary>
    /// <param name="moduleId">模块实例Id.</param>
    /// <returns>返回 是否权限访问.</returns>
    bool Verify(int moduleId);
}