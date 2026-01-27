namespace BgCommon.Prism.Wpf.Authority.Models;

/// <summary>
/// 批量授予直接访问权限操作的结果.
/// </summary>
public class BatchGrantAccessResult
{
    /// <summary>
    /// Gets 成功授予的权限列表.
    /// </summary>
    public List<UserAccessRights> SuccessfullyGranted { get; } = new List<UserAccessRights>();

    /// <summary>
    /// Gets 因重复而被跳过的请求列表.
    /// </summary>
    public List<AccessRightRequest> SkippedAsDuplicate { get; } = new List<AccessRightRequest>();

    /// <summary>
    /// Gets 处理失败的请求及其原因列表.
    /// </summary>
    public List<FailedGrantRequest> Failed { get; } = new List<FailedGrantRequest>();

    /// <summary>
    /// Gets 成功处理的数量.
    /// </summary>
    public int SuccessCount => SuccessfullyGranted.Count;

    /// <summary>
    /// Gets 跳过的数量.
    /// </summary>
    public int SkippedCount => SkippedAsDuplicate.Count;

    /// <summary>
    /// Gets 失败的数量.
    /// </summary>
    public int FailureCount => Failed.Count;
}

/// <summary>
/// 表示一个要授予的访问权限的请求.
/// </summary>
public class AccessRightRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessRightRequest"/> class.
    /// </summary>
    /// <param name="refObjectType">被引用对象的类型.</param>
    /// <param name="refObjectId">被引用对象的ID.</param>
    public AccessRightRequest(RefObjectType refObjectType, long refObjectId)
    {
        this.RefObjectType = refObjectType;
        this.RefObjectId = refObjectId;
    }

    /// <summary>
    /// Gets 被引用对象的类型.
    /// </summary>
    public RefObjectType RefObjectType { get; }

    /// <summary>
    /// Gets 被引用对象的ID.
    /// </summary>
    public long RefObjectId { get; }
}

/// <summary>
/// 表示一个失败的授权请求及其原因.
/// </summary>
public record FailedGrantRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedGrantRequest"/> class.
    /// </summary>
    /// <param name="request">原始请求.</param>
    /// <param name="reason">失败原因.</param>
    public FailedGrantRequest(AccessRightRequest request, string reason)
    {
        this.Request = request;
        this.Reason = reason;
    }

    /// <summary>
    /// Gets 原始请求.
    /// </summary>
    public AccessRightRequest Request { get; }

    /// <summary>
    /// Gets 失败原因.
    /// </summary>
    public string Reason { get; }
}
