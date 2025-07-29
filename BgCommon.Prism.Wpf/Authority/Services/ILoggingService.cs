namespace BgCommon.Prism.Wpf.Authority.Services;

public interface ILoggingService
{
    /// <summary>
    /// 通用的操作日志记录方法.
    /// </summary>
    /// <param name="operatorId">用户ID (可空).</param>
    /// <param name="operatorUsername">用户名.</param>
    /// <param name="actionType">操作类型.</param>
    /// <param name="details">操作详情.</param>
    Task LogOperationAsync(long? operatorId, string operatorUsername, string actionType, string details);
}