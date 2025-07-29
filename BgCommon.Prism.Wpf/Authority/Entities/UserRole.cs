namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 用户角色类.
/// </summary>
public partial class UserRole : ObservableValidator
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
    private int userId;

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

    public UserInfo? User { get; set; } = null;

    public Role? Role { get; set; } = null;
}