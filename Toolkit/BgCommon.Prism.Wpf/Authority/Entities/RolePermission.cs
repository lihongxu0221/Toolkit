namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 角色权限列表类.
/// </summary>
public partial class RolePermission : ObservableValidator, ICloneable
{
    /// <summary>
    /// Gets or sets Key.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets 角色ID.
    /// </summary>
    [Required]
    [ObservableProperty]
    private int roleId;

    /// <summary>
    /// Gets or sets 权限Id.
    /// </summary>
    [Required]
    [ObservableProperty]
    private int permissionId;

    public Role? Role { get; set; } = null;

    public Permission? Permission { get; set; } = null;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}