namespace BgCommon.Prism.Wpf.Modules.Parameters.Models;

/// <summary>
/// 给参数赋予权限.
/// </summary>
public partial class ParameterRoleDisplay : ObservableObject
{
    public ERole Role { get; }

    public int Id { get; }

    public string Name { get; }

    public SystemRole SystemRole { get; }

    public int Authority => (int)SystemRole;

    public bool HasLoaded { get; set; }

    [ObservableProperty]
    private ObservableRangeCollection<ParameterPermissionDisplay> permissions = new();

    public ParameterRoleDisplay(ERole role)
    {
        Role = role;
        Id = Role.Id;
        Name = Role.Name;
        SystemRole = Role.SystemRole;
        Permissions = new ObservableRangeCollection<ParameterPermissionDisplay>();
    }
}