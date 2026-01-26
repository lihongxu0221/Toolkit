namespace BgCommon.DbService;

/// <summary>
/// 定义直接访问权限检查的三种可能结果.
/// </summary>
internal enum DirectAccessCheckResult
{
    /// <summary>
    /// 用户未配置直接权限，应回退到基于角色的检查.
    /// </summary>
    FallbackToRole,

    /// <summary>
    /// 用户已配置直接权限，且权限列表中包含当前请求，因此授权.
    /// </summary>
    Granted,

    /// <summary>
    /// 用户已配置直接权限，但权限列表中【不】包含当前请求，因此拒绝.
    /// </summary>
    Denied,
}