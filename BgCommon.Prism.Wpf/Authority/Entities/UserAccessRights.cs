namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 用户权限类.
/// </summary>
public partial class UserAccessRights : ObservableValidator, ICloneable
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
    public long UserId { get; set; }

    /// <summary>
    /// Gets or sets 引用类型.
    /// </summary>
    [Required]
    public RefObjectType RefObjType { get; set; }

    /// <summary>
    /// Gets or sets 外键引用的对象ID (可以是模块ID、功能ID等).
    /// </summary>
    [Required]
    public long RefObjId { get; set; }

    /// <summary>
    /// Gets or sets 外连用户信息实体.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual UserInfo? User { get; set; }

    /// <summary>
    /// Gets or sets 手动加载的模块信息.
    /// 此属性不会被持久化到数据库.
    /// </summary>
    [NotMapped]
    public ModuleInfo? Module { get; set; }

    /// <summary>
    /// Gets or sets 手动加载的权限信息.
    /// 此属性不会被持久化到数据库.
    /// </summary>
    [NotMapped]
    public Permission? Permission { get; set; }

    /// <inheritdoc/>
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
