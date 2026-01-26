namespace BgCommon.DbService.Attributes;

/// <summary>
/// 定义角色权限特性.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RolePermissionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permissionCode">权限代码.</param>
    /// <param name="roles">拥有该权限的系统角色数组.</param>
    public RolePermissionAttribute(PermissionCode permissionCode, params SystemRole[] roles)
    {
        this.PermissionCode = permissionCode;
        this.Roles = roles ?? Array.Empty<SystemRole>();
        this.ModuleCode = string.Empty;
    }

    /// <summary>
    /// Gets 权限代码.
    /// </summary>
    public PermissionCode PermissionCode { get; }

    /// <summary>
    /// Gets 拥有该权限的角色列表.
    /// </summary>
    public IReadOnlyList<SystemRole> Roles { get; }

    /// <summary>
    /// Gets 所属模块编码(如果未指定则默认以特性所在类的FullName为ModuleCode).
    /// </summary>
    public string ModuleCode { get; init; }
}