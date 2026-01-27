namespace BgCommon.DbService.Models.Entities;

/// <summary>
/// 用户信息.
/// </summary>
public partial class UserInfo :
    ObservableValidator,
    ICloneable,
    ISelfValidator
{
    /// <summary>
    /// Gets or sets 用户ID (主键).
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets 用户名.
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空", AllowEmptyStrings = false)]
    [MaxLength(50)]
    [ObservableProperty]
    private string userName = string.Empty;

    /// <summary>
    /// Gets or sets 密码.
    /// </summary>
    [Required(ErrorMessage = "密码不能为空", AllowEmptyStrings = false)]
    [MaxLength(50)]
    [ObservableProperty]
    private string password = string.Empty;

    /// <summary>
    /// Gets or sets 创建时间.
    /// </summary>
    [Required]
    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;

    /// <summary>
    /// Gets or sets 创建人.
    /// </summary>
    [Required]
    [ObservableProperty]
    private string createBy = string.Empty;

    /// <summary>
    /// Gets or sets 是否在线.
    /// </summary>
    [Required]
    [ObservableProperty]
    private bool isOnline = false;

    /// <summary>
    /// Gets or sets a value indicating whether 是否为系统默认的操作员账号.
    /// 系统中应该只有一个用户的此标志位为 true.
    /// </summary>
    [ObservableProperty]
    private bool isDefault = false;

    /// <summary>
    /// Gets or sets 是否启用.
    /// </summary>
    [Required]
    [ObservableProperty]
    private bool isActive = true;

    /// <summary>
    /// Gets or sets 导航属性：一个用户可以有多个角色.
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Gets 专门用于UI绑定的显示属性.
    /// </summary>
    [NotMapped]
    public string MaxRoleName
    {
        get
        {
            // 在这里处理 null 的情况
            return MaxRole?.Name ?? Lang.GetString("未分配角色");
        }
    }

    /// <summary>
    /// Gets 用户具有的最高权限的角色. (只读计算属性)
    /// 明确告诉EF Core不要尝试将此属性映射到数据库列.
    /// </summary>
    [NotMapped]
    public Role? MaxRole
    {
        get
        {
            // 如果 UserRoles 为空或未加载，直接返回 null
            if (UserRoles == null || UserRoles.Count == 0)
            {
                return null;
            }

            // 【最终方案】
            // 1. OrderByDescending(ur => ur.Role?.Authority ?? -1)
            //    - 按照每个 UserRole 关联的 Role 的 Authority 属性进行【降序】排序.
            //    - 使用 ?. (空值传播操作符) 和 ?? (空合并操作符) 来安全地处理 ur.Role 为 null 的情况。
            //      如果 Role 为 null，我们给一个非常低的值(-1)，确保它排在最后。
            // 2. .FirstOrDefault()
            //    - 在排序后，获取【第一个】元素。因为是降序，所以第一个就是权限最高的那个。
            //    - 如果集合为空，FirstOrDefault 会安全地返回 null，绝不会抛出异常。
            // 3. .Role
            //    - 从找到的那个 UserRole 对象中，获取其关联的 Role 对象。
            return UserRoles.OrderByDescending(ur => ur.Role?.Authority ?? int.MinValue)
                            .FirstOrDefault()?.Role;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserInfo"/> class.
    /// </summary>
    public UserInfo()
    {
    }

    /// <summary>
    /// 克隆当前对象.
    /// </summary>
    /// <returns>返回克隆的结果.</returns>
    public object Clone()
    {
        return this.MemberwiseClone();
    }

    /// <summary>
    /// 校验所有的属性.
    /// </summary>
    public void Validate()
    {
        this.ValidateAllProperties();
    }
}