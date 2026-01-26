namespace BgCommon.DbService.Models.Entities;

/// <summary>
/// 用于在本地数据库中记住用户登录信息的数据实体.
/// </summary>
public class LoginInfo
{
    /// <summary>
    /// Gets or sets 主键.
    /// </summary>
    [Key]
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets 记住的用户ID，用于直接关联用户.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Gets or sets 记住的用户名.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 记住的密码 (警告：应加密存储).
    /// 为加密后的密码预留足够空间.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets 是否启用了自动登录.
    /// </summary>
    public bool IsAutoLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets 是否勾选了“记住密码”.
    /// 如果为 false, Password 字段应为 null.
    /// </summary>
    public bool IsRemember { get; set; }

    /// <summary>
    /// Gets or sets 登录IP地址 (保留属性).
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }
}