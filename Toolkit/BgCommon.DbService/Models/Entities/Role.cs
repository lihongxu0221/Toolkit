namespace BgCommon.DbService.Models.Entities;

/// <summary>
/// 角色类.
/// </summary>
public partial class Role :
    ObservableValidator,
    ICloneable
{
    /// <summary>
    /// Gets or sets 角色ID.
    /// </summary>
    [Key]
    public int Id { get; set; }

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
    /// Gets or sets 角色名称.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// Gets or sets 描述信息.
    /// </summary>
    [MaxLength(200)]
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Gets or sets 是否启用.
    /// </summary>
    [ObservableProperty]
    private bool enabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    public Role()
    {
        this.SystemRole = SystemRole.Guest;
    }

    /// <summary>
    /// Gets or sets 导航属性：通过 UserRole 连接表指向多个角色.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Gets or sets 导航属性：一个角色可以拥有多个权限.
    /// </summary>
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();

    /// <inheritdoc/>
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}