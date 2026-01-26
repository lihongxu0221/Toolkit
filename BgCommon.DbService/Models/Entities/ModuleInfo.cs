namespace BgCommon.DbService.Models.Entities;

/// <summary>
/// 模块信息实体类.
/// </summary>
public partial class ModuleInfo :
    ObservableValidator,
    ICloneable,
    IEquatable<ModuleInfo>,
    ISelfValidator
{
    private bool isSelected;
    private bool isVirtual = true;

    /// <summary>
    /// Gets or sets 模块ID (主键).
    /// </summary>
    [Key]
    [ObservableProperty]
    private long id;

    /// <summary>
    /// 模块的唯一业务代码，由开发者定义，用于程序内部识别.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "模块编码不能为空")]
    [MaxLength(100)]
    [ObservableProperty]
    private string code = string.Empty;

    /// <summary>
    /// Gets 获取用于计算的权限等级. 此属性【不会】被持久化到数据库..
    /// </summary>
    [NotMapped]
    public int Authority => (int)SystemRole;

    /// <summary>
    /// Gets or sets 权限等级.
    /// </summary>
    [Required]
    [ObservableProperty]
    private SystemRole systemRole = SystemRole.Guest;

    /// <summary>
    /// Gets or sets 模块名称.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "模块名称不能为空")]
    [MaxLength(100)]
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// Gets or sets 父模块ID，用于构建树形菜单/模块结构.
    /// </summary>
    [ObservableProperty]
    private long? parentId;

    /// <summary>
    /// Gets or sets 模块对应的（在UI框架中）的类型完全限定名，用于动态加载.<br/>
    /// 例如 "MyProject.Views.UserManagementView".
    /// </summary>
    [Required]
    [MaxLength(255)]
    [ObservableProperty]
    private string typeFullName = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether 是否启用该模块.
    /// </summary>
    [ObservableProperty]
    private bool isEnabled = true;

    /// <summary>
    /// Gets or sets a value indicating whether 是否为预制默认功能模块.
    /// </summary>
    [ObservableProperty]
    private bool isDefault = false;

    /// <summary>
    /// 是否为树节点.
    /// </summary>
    [ObservableProperty]
    private bool isTreeNode = true;

    /// <summary>
    /// Gets or sets a value indicating whether 是否选中.
    /// </summary>
    [NotMapped]
    public bool IsSelected
    {
        get { return this.isSelected; }
        set { _ = this.SetProperty(ref this.isSelected, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否为虚拟模块（非实际存在的模块）.
    /// </summary>
    [NotMapped]
    public bool IsVirtual
    {
        get { return this.isVirtual; }
        set { _ = this.SetProperty(ref this.isVirtual, value); }
    }

    /// <summary>
    /// Gets or sets 模块图标的字符串表示（例如 FontAwesome, MDI 等）.
    /// </summary>
    [MaxLength(100)]
    [ObservableProperty]
    private string? icon = string.Empty;

    /// <summary>
    /// Gets or sets 创建时间.
    /// </summary>
    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;

    /// <summary>
    /// Gets or sets 最后修改时间.
    /// </summary>
    [ObservableProperty]
    private DateTime lastModifiedAt = DateTime.Now;

    /// <summary>
    /// Gets or sets 创建人.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [ObservableProperty]
    private string createdBy = string.Empty;

    /// <summary>
    /// Gets or sets 修改人.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [ObservableProperty]
    private string modifiedBy = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInfo"/> class.
    /// </summary>
    public ModuleInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInfo"/> class.
    /// 根据ModuleItem实例初始化ModuleInfo类的新实例.
    /// 此构造函数用于从ModuleItem到ModuleInfo的显式转换.
    /// </summary>
    /// <param name="moduleItem">作为数据源的ModuleItem实例.</param>
    public ModuleInfo(FeatureSeedNode moduleItem)
    {
        this.Id = moduleItem.Id;
        this.ParentId = moduleItem.ParentId;
        this.Code = moduleItem.Code;
        this.Name = moduleItem.Name;
        this.TypeFullName = moduleItem.TypeFullName;
        this.SystemRole = moduleItem.SystemRole;
        this.Icon = moduleItem.Icon;
        this.IsTreeNode = moduleItem.IsTreeNode;
        this.IsDefault = moduleItem.IsDefault;
        this.IsEnabled = moduleItem.IsEnabled;
        this.IsVirtual = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInfo"/> class.
    /// 使用指定的参数初始化ModuleInfo类的新实例.
    /// 此构造函数提供完整的参数设置，用于创建具有特定属性的模块信息.
    /// </summary>
    /// <param name="id">模块的唯一标识符.</param>
    /// <param name="parentId">父模块的标识符，可为空.</param>
    /// <param name="code">模块代码.</param>
    /// <param name="name">模块名称.</param>
    /// <param name="typeFullName">模块类型的完整名称.</param>
    /// <param name="systemRole">模块的系统角色.</param>
    /// <param name="icon">模块图标，可为空.</param>
    /// <param name="isTreeNode">指示模块是否作为树节点显示，默认为true.</param>
    /// <param name="isDefault">指示模块是否为默认模块，默认为false.</param>
    /// <param name="isVirtual">指示模块是否为虚拟模块，默认为true.</param>
    public ModuleInfo(
        long id,
        long? parentId,
        string code,
        string name,
        string typeFullName,
        SystemRole systemRole,
        string? icon = null,
        bool isTreeNode = true,
        bool isDefault = false,
        bool isVirtual = true)
    {
        this.Id = id;
        this.ParentId = parentId;
        this.Code = code;
        this.Name = name;
        this.TypeFullName = typeFullName;
        this.SystemRole = systemRole;
        this.Icon = icon;
        this.IsTreeNode = isTreeNode;
        this.IsDefault = isDefault;
        this.CreatedAt = DateTime.Now;
        this.IsEnabled = true;
        this.IsVirtual = isVirtual;
    }

    /// <summary>
    /// Gets or sets 导航属性：父模块.
    /// </summary>
    public virtual ModuleInfo? ParentModule { get; set; }

    /// <summary>
    /// Gets or sets 导航属性：子模块集合.
    /// </summary>
    public virtual ICollection<ModuleInfo> ChildModules { get; set; } = new List<ModuleInfo>();

    /// <summary>
    /// Gets or sets  导航属性：与此模块关联的所有权限.
    /// </summary>
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    /// <summary>
    /// Gets 通过模块类全路径获取Type (非数据库映射).
    /// </summary>
    /// <returns>Type对象或 null.</returns>
    [NotMapped]
    public Type? ModuleType
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TypeFullName))
            {
                return null;
            }

            return Type.GetType(TypeFullName);
        }
    }

    /// <inheritdoc/>
    public object Clone()
    {
        return this.MemberwiseClone();
    }

    /// <inheritdoc/>
    public bool Equals(ModuleInfo? other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(default(ModuleInfo), other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Code == other.Code &&
               this.Name == other.Name &&
               this.TypeFullName == other.TypeFullName &&
               this.ParentId == other.ParentId &&
               this.SystemRole == other.SystemRole &&
               this.Icon == other.Icon &&
               this.IsEnabled == other.IsEnabled &&
               this.IsDefault == other.IsDefault &&
               this.IsTreeNode == other.IsTreeNode;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as ModuleInfo);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(this.Code);
        hash.Add(this.Name);
        hash.Add(this.TypeFullName);
        hash.Add(this.ParentId);
        hash.Add(this.SystemRole);
        hash.Add(this.Icon);
        hash.Add(this.IsEnabled);
        hash.Add(this.IsDefault);
        hash.Add(this.IsTreeNode);
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Code: {Code}, Type: {TypeFullName}, Enabled: {IsEnabled})";
    }

    /// <inheritdoc/>
    public void Validate()
    {
        this.ValidateAllProperties();
    }
}