namespace BgCommon.DbService.Attributes;

/// <summary>
/// 定义权限特性.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class PermissionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttribute"/> class.
    /// </summary>
    /// <param name="name">权限名称.</param>
    /// <param name="code">权限代码.</param>
    public PermissionAttribute(string name, PermissionCode code)
    {
        this.Code = code;
        this.Name = name;
        this.ModuleCode = string.Empty;
    }

    /// <summary>
    /// Gets 权限代码.
    /// </summary>
    public PermissionCode Code { get; }

    /// <summary>
    /// Gets 权限名称.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets 所属模块编码(如果未指定则默认以特性所在类的FullName为ModuleCode).
    /// </summary>
    public string ModuleCode { get; init; }
}