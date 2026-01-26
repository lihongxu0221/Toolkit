namespace BgCommon.Prism.Wpf.Authority.Models;

/// <summary>
/// 用于封装用户角色信息及其最高权限等级的数据传输对象.
/// </summary>
public class UserRolesInfo
{
    /// <summary>
    /// Gets or sets 用户【当前已被分配】的角色列表
    /// </summary>
    public List<Role> AssignedRoles { get; set; } = new List<Role>();

    /// <summary>
    /// Gets or sets 用户【有权管理/分配】的所有角色列表
    /// (权限等级低于或等于用户自身的最高权限)
    /// </summary>
    public List<Role> ManageableRoles { get; set; } = new List<Role>();

    /// <summary>
    /// Gets or sets 在用户拥有的所有角色中，最高的 Authority 值.
    /// </summary>
    public int MaxAuthority { get; set; }

    /// <summary>
    /// Gets 用户具有的最大权限的角色.
    /// </summary>
    public Role? MaxRole => AssignedRoles.FirstOrDefault(r => r.Authority == MaxAuthority);
}