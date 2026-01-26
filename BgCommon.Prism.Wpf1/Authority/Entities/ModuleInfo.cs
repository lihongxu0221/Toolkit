namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 模块信息实体类.
/// </summary>
public partial class ModuleInfo : ObservableValidator
{
    /// <summary>
    /// Gets or sets 模块ID (主键).
    /// </summary>
    [Key]
    [ObservableProperty]
    private long id;

    /// <summary>
    /// Gets or sets 角色访问权限 code.
    /// </summary>
    [Required]
    [ObservableProperty]
    private int authority;

    /// <summary>
    /// Gets or sets 模块名称.
    /// </summary>
    [Required]
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
    /// 例如 "MyProject.Views.UserManagementView"
    /// </summary>
    [Required]
    [MaxLength(255)]
    [ObservableProperty]
    private string typeFullName = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether 是否启用该模块.
    /// </summary>
    [ObservableProperty]
    private bool isEnabled;

    /// <summary>
    /// Gets or sets a value indicating whether 是否选中.
    /// </summary>
    [ObservableProperty]
    private bool isSelected = false;

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

    public ModuleInfo()
    {
    }

    public ModuleInfo(int moduleId, string moduleName, string? moduleTypeFullName, bool isEnabled, int? parentModuleId = null, string? icon = "")
    {
        Id = moduleId;
        Name = moduleName;
        ParentId = parentModuleId;
        TypeFullName = moduleTypeFullName ?? string.Empty;
        IsEnabled = isEnabled;
        Icon = icon;
        CreatedAt = DateTime.Now;
        LastModifiedAt = DateTime.Now;
        CreatedBy = string.Empty;
        ModifiedBy = string.Empty;
    }

    /// <summary>
    /// Gets or sets 导航属性：父模块.
    /// </summary>
    public ModuleInfo? ParentModule { get; set; }

    /// <summary>
    /// Gets or sets 导航属性：子模块集合.
    /// </summary>
    public ICollection<ModuleInfo> ChildModules { get; set; } = new List<ModuleInfo>();

    /// <summary>
    /// Gets or sets  导航属性：与此模块关联的所有权限
    /// </summary>
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

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
}