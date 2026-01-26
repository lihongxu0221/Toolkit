namespace BgCommon.DbService.Attributes;

/// <summary>
/// 模块信息特性.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ModuleInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInfoAttribute"/> class.
    /// </summary>
    /// <param name="name">模块名称.</param>
    /// <param name="role">与模块关联的系统角色.</param>
    public ModuleInfoAttribute(string name, SystemRole role)
    {
        this.Name = name;
        this.Role = role;
        this.Code = string.Empty;
        this.Id = 0;
        this.ParentId = null;
        this.ParentCode = string.Empty;
    }

    /// <summary>
    /// Gets 模块代码(如果未指定则默认以特性所在类的FullName为ModuleCode).
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Gets 模块名称.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets 与模块关联的系统角色.
    /// </summary>
    public SystemRole Role { get; }

    /// <summary>
    /// Gets 模块ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets 父模块代码.
    /// </summary>
    public string ParentCode { get; init; }

    /// <summary>
    /// Gets 父模块ID.
    /// </summary>
    public int? ParentId { get; init; }

    /// <summary>
    /// Gets 模块图标.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Gets a value indicating whether 模块是否为树节点.
    /// </summary>
    public bool IsTreeNode { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether 模块是否为默认模块.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Gets a value indicating whether 模块是否已启用.
    /// </summary>
    public bool IsEnabled { get; init; } = true;
}