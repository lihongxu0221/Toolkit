namespace BgCommon.DbService.Services;

/// <summary>
/// 日志记录服务接口.
/// 定义系统操作日志的统一记录规范，提供异步日志记录能力，支持追踪用户操作行为.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// 通用的操作日志记录方法，异步将用户操作行为持久化（如写入数据库、日志文件等）.
    /// 适用于系统中各类业务操作的日志追踪，包含操作人、操作类型、操作详情等核心信息.
    /// </summary>
    /// <param name="operatorId">操作人ID (可空)，未登录用户或系统操作可传入 null.</param>
    /// <param name="operatorUsername">操作人用户名，用于直观标识操作主体，不可为空字符串.</param>
    /// <param name="actionType">操作类型，描述操作的具体类别（如“新增用户”“编辑角色”“删除权限”等），建议使用统一枚举值转换的字符串.</param>
    /// <param name="details">操作详情，记录操作的具体内容（如“新增用户ID：1001，用户名：test”“编辑角色名称：管理员→系统管理员”等），支持JSON格式字符串存储复杂信息.</param>
    /// <returns>表示日志记录操作完成的 Task 任务，无返回值.</returns>
    Task LogOperationAsync(long? operatorId, string operatorUsername, string actionType, string details);
}