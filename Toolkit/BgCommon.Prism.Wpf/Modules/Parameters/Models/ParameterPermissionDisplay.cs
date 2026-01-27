using System.Reflection.Metadata;

namespace BgCommon.Prism.Wpf.Modules.Parameters.Models;

/// <summary>
/// 用于在参数权限界面DataGrid中展示的、可绑定的数据模型.
/// </summary>
public partial class ParameterPermissionDisplay : ObservableObject
{
    /// <summary>
    /// Gets 原始的参数信息 (只读)
    /// </summary>
    public SystemParameter Parameter { get; }

    /// <summary>
    /// Gets 原始的权限信息 (可能为 null)
    /// </summary>
    public ParameterPermission Permission { get; }

    public string ParameterName { get; }

    public string ParameterCode { get; }

    public string RoleName { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    private bool isVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    private bool isEditable;

    /// <summary>
    /// Gets a value indicating whether a value indicating whether a new instance of the <see cref="ParameterPermissionDisplay"/> class is dirty.
    /// 检查此条目的权限设置是否已被用户修改.
    /// </summary>
    public bool IsDirty => Permission == null ? (IsVisible || IsEditable) : (Permission.IsVisible != IsVisible || Permission.IsEditable != IsEditable);

    public ParameterPermissionDisplay(ParameterPermission permission)
    {
        Permission = permission;
        Parameter = Permission.SystemParameter!;
        ParameterCode = Parameter.Code;
        ParameterName = Parameter.Name;
        IsVisible = Permission.IsVisible;
        IsEditable = Permission.IsEditable;
        RoleName = Permission.Role?.Name ?? string.Empty;
    }

    public ParameterPermissionDisplay(SystemParameter parameter, ParameterPermission? permission)
    {
        Parameter = parameter;
        ParameterCode = parameter.Code;
        ParameterName = parameter.Name;
        Permission = permission ?? new ParameterPermission { ParameterId = parameter.Id };

        // 初始化UI的勾选状态
        IsVisible = Permission.IsVisible;
        IsEditable = Permission.IsEditable;
        RoleName = Permission.Role?.Name ?? string.Empty;
    }

    /// <summary>
    /// 将当前的UI状态转换回一个可保存的 ParameterPermission 实体.
    /// </summary>
    public ParameterPermission ToEntity()
    {
        Permission.IsVisible = IsVisible;
        Permission.IsEditable = IsEditable;
        return Permission;
    }
}