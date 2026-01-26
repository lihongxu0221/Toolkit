namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 用户角色类.
/// </summary>
public partial class UserRole : ObservableValidator, ICloneable
{
    /// <summary>
    /// Gets or sets Key.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets 用户ID.
    /// </summary>
    [Required]
    [ObservableProperty]
    private long userId;

    /// <summary>
    /// Gets or sets 角色ID.
    /// </summary>
    [Required]
    [ObservableProperty]
    private int roleId;

    /// <summary>
    /// Gets or sets 分配时间.
    /// </summary>
    [ObservableProperty]
    private DateTime assignedAt = DateTime.Now;

    /// <summary>
    /// Gets or sets 分配用户ID (可以是 null).
    /// </summary>
    [ObservableProperty]
    private int? assignedByUserId;

    /// <summary>
    /// Gets or sets 外连用户信息实体.
    /// </summary>
    public UserInfo? User { get; set; }

    /// <summary>
    /// Gets or sets 外连角色信息实体.
    /// </summary>
    public Role? Role { get; set; }

    /// <inheritdoc/>
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}