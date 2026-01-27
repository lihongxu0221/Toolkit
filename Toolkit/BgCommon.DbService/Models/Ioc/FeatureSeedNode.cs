namespace BgCommon.DbService.Models.Ioc;

/// <summary>
/// 定义一个功能树的节点，它是一个可递归的数据结构.
/// 这是开发者向系统提供“出厂设置”的核心DTO.
/// </summary>
public class FeatureSeedNode
{
    /// <summary>
    /// Gets 与此模块关联的权限集合.
    /// </summary>
    public ICollection<PermissionSeed> Permissions { get; init; } = new List<PermissionSeed>();

    /// <summary>
    /// Gets 默认的角色权限映射.
    /// 字典的键(Key)是权限代码(PermissionCode)，值(Value)是拥有该权限的角色枚举集合.
    /// </summary>
    public IDictionary<string, IEnumerable<SystemRole>> DefaultRolePermissions { get; init; } = new Dictionary<string, IEnumerable<SystemRole>>();

    /// <summary>
    /// Gets 与此模块关联的参数集合.
    /// </summary>
    public ICollection<ParameterSeed> Parameters { get; init; } = new List<ParameterSeed>();

    /// <summary>
    /// Gets 子节点集合，用于构建功能树.
    /// </summary>
    public ICollection<FeatureSeedNode> Children { get; init; } = new List<FeatureSeedNode>();

    /// <summary>
    /// Gets or sets 节点Id.
    /// </summary>
    internal long Id { get; set; }

    /// <summary>
    /// Gets 模块父Id.
    /// </summary>
    internal long? ParentId { get; init; }

    /// <summary>
    /// Gets 模块编码（唯一）.
    /// </summary>
    internal string Code { get; init; }

    /// <summary>
    /// Gets 模块名称.
    /// </summary>
    internal string Name { get; init; }

    /// <summary>
    /// Gets 模块全路径.
    /// </summary>
    internal string TypeFullName { get; init; }

    /// <summary>
    /// Gets 权限角色.
    /// </summary>
    internal SystemRole SystemRole { get; init; }

    /// <summary>
    /// Gets 字体图标Unicode值.
    /// </summary>
    internal string? Icon { get; init; }

    /// <summary>
    /// Gets a value indicating whether gets 是否为树节点.
    /// </summary>
    internal bool IsTreeNode { get; init; }

    /// <summary>
    /// Gets a value indicating whether gets 是否为系统默认模块.
    /// </summary>
    internal bool IsDefault { get; init; }

    /// <summary>
    /// Gets a value indicating whether gets 是否启|禁用模块.
    /// </summary>
    internal bool IsEnabled { get; init; }

    /// <summary>
    /// Gets or sets 节点层级【辅助字段】.<br/>
    /// 不应手动赋值，用于解决父节点Id未知时，无法正确增加子节点问题.
    /// </summary>
    internal int Level { get; set; }

    /// <summary>
    /// Gets or sets 父节点层级 【辅助字段】.
    /// </summary>
    internal FeatureSeedNode? ParentNode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSeedNode"/> class.
    /// </summary>
    /// <param name="id">模块ID.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <param name="code">模块代码.</param>
    /// <param name="name">模块名称.</param>
    /// <param name="typeFullName">模块类型的完全限定名.</param>
    /// <param name="role">访问该模块所需的基础系统角色.</param>
    /// <param name="icon">模块图标.</param>
    /// <param name="isTreeNode">一个值，指示模块是否为树节点.</param>
    /// <param name="isDefault">一个值，指示模块是否为默认模块.</param>
    /// <param name="isEnabled">一个值，指示模块是否已启用.</param>
    public FeatureSeedNode(
        long id,
        long? parentId,
        string code,
        string name,
        string typeFullName,
        SystemRole role,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isEnabled = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        this.Id = id;
        this.ParentId = parentId;
        this.Code = code;
        this.Name = name;
        this.TypeFullName = typeFullName;
        this.SystemRole = role;
        this.Icon = icon;
        this.IsTreeNode = isTreeNode;
        this.IsDefault = isDefault;
        this.IsEnabled = isEnabled;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSeedNode"/> class.
    /// </summary>
    /// <param name="name">模块名称.</param>
    /// <param name="moduleType">模块类型.</param>
    /// <param name="role">访问该模块所需的基础系统角色.</param>
    /// <param name="icon">模块图标.</param>
    /// <param name="isTreeNode">一个值，指示模块是否为树节点.</param>
    /// <param name="isDefault">一个值，指示模块是否为默认模块.</param>
    /// <param name="isEnabled">一个值，指示模块是否已启用.</param>
    public FeatureSeedNode(
        string name,
        Type moduleType,
        SystemRole role,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isEnabled = true)
        : this(moduleType.FullName!, name, moduleType, role, icon, isTreeNode, isDefault, isEnabled)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSeedNode"/> class.
    /// </summary>
    /// <param name="code">模块代码.</param>
    /// <param name="name">模块名称.</param>
    /// <param name="moduleType">模块类型.</param>
    /// <param name="role">访问该模块所需的基础系统角色.</param>
    /// <param name="icon">模块图标.</param>
    /// <param name="isTreeNode">一个值，指示模块是否为树节点.</param>
    /// <param name="isDefault">一个值，指示模块是否为默认模块.</param>
    /// <param name="isEnabled">一个值，指示模块是否已启用.</param>
    public FeatureSeedNode(
        string code,
        string name,
        Type moduleType,
        SystemRole role,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isEnabled = true)
        : this(0, code, name, moduleType, role, icon, isTreeNode, isDefault, isEnabled)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSeedNode"/> class.
    /// </summary>
    /// <param name="id">模块ID.</param>
    /// <param name="code">模块代码.</param>
    /// <param name="name">模块名称.</param>
    /// <param name="moduleType">模块类型.</param>
    /// <param name="role">访问该模块所需的基础系统角色.</param>
    /// <param name="icon">模块图标.</param>
    /// <param name="isTreeNode">一个值，指示模块是否为树节点.</param>
    /// <param name="isDefault">一个值，指示模块是否为默认模块.</param>
    /// <param name="isEnabled">一个值，指示模块是否已启用.</param>
    public FeatureSeedNode(
        long id,
        string code,
        string name,
        Type moduleType,
        SystemRole role,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isEnabled = true)
        : this(id, null, code, name, moduleType, role, icon, isTreeNode, isDefault, isEnabled)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureSeedNode"/> class.
    /// </summary>
    /// <param name="id">模块ID.</param>
    /// <param name="parentId">父模块ID.</param>
    /// <param name="code">模块代码.</param>
    /// <param name="name">模块名称.</param>
    /// <param name="moduleType">模块类型.</param>
    /// <param name="role">访问该模块所需的基础系统角色.</param>
    /// <param name="icon">模块图标.</param>
    /// <param name="isTreeNode">一个值，指示模块是否为树节点.</param>
    /// <param name="isDefault">一个值，指示模块是否为默认模块.</param>
    /// <param name="isEnabled">一个值，指示模块是否已启用.</param>
    public FeatureSeedNode(
        long id,
        long? parentId,
        string code,
        string name,
        Type moduleType,
        SystemRole role,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isEnabled = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        this.Id = id;
        this.ParentId = parentId;
        this.Code = code ?? moduleType.FullName!;
        this.Name = name;
        this.TypeFullName = moduleType.ToFullName();
        this.SystemRole = role;
        this.Icon = icon;
        this.IsTreeNode = isTreeNode;
        this.IsDefault = isDefault;
        this.IsEnabled = isEnabled;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Level}]{this.Id},{this.Name},{this.Code},{this.TypeFullName}";
    }
}