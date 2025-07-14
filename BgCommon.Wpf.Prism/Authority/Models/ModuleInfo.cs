namespace BgCommon.Wpf.Prism.Authority.Models;

/// <summary>
/// 模块信息实体类.
/// </summary>
public class ModuleInfo : ObservableObject
{
    private int moduleId;
    private string moduleName = string.Empty;
    private int? parentModuleId;
    private string moduleTypeFullName = string.Empty;
    private bool isEnabled;
    private bool isSelected = false;
    private string? icon;
    private DateTime createdTime;
    private DateTime lastModifiedTime;
    private string? createdBy;
    private string? modifiedBy;
    private UserAuthority authority = UserAuthority.Operator;

    /// <summary>
    /// Gets or sets 模块ID.
    /// </summary>
    public int ModuleId
    {
        get => moduleId;
        set => _ = SetProperty(ref moduleId, value);
    }

    /// <summary>
    /// Gets or sets 模块名称.
    /// </summary>
    public string ModuleName
    {
        get => moduleName;
        set => _ = SetProperty(ref moduleName, value);
    }

    /// <summary>
    /// Gets or sets 父模块ID.
    /// </summary>
    public int? ParentModuleId
    {
        get => parentModuleId;
        set => _ = SetProperty(ref parentModuleId, value);
    }

    /// <summary>
    /// Gets or sets 模块类型全名.
    /// </summary>
    public string ModuleTypeFullName
    {
        get => moduleTypeFullName;
        set => _ = SetProperty(ref moduleTypeFullName, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否启用.
    /// </summary>
    public bool IsEnabled
    {
        get => isEnabled;
        set => _ = SetProperty(ref isEnabled, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否选中.
    /// </summary>
    public bool IsSelected
    {
        get => isSelected;
        set => _ = SetProperty(ref isSelected, value);
    }

    /// <summary>
    /// Gets or sets 图标.
    /// </summary>
    public string? Icon
    {
        get => icon;
        set => _ = SetProperty(ref icon, value);
    }

    /// <summary>
    /// Gets or sets 创建时间.
    /// </summary>
    public DateTime CreatedTime
    {
        get => createdTime;
        set => _ = SetProperty(ref createdTime, value);
    }

    /// <summary>
    /// Gets or sets 最后修改时间.
    /// </summary>
    public DateTime LastModifiedTime
    {
        get => lastModifiedTime;
        set => _ = SetProperty(ref lastModifiedTime, value);
    }

    /// <summary>
    /// Gets or sets 创建人.
    /// </summary>
    public string? CreatedBy
    {
        get => createdBy;
        set => _ = SetProperty(ref createdBy, value);
    }

    /// <summary>
    /// Gets or sets 修改人.
    /// </summary>
    public string? ModifiedBy
    {
        get => modifiedBy;
        set => _ = SetProperty(ref modifiedBy, value);
    }

    /// <summary>
    /// Gets or sets 可用权限.
    /// </summary>
    public UserAuthority Authority
    {
        get => authority;
        set => _ = SetProperty(ref authority, value);
    }

    /// <summary>
    /// 通过模块类全路径获取Type.
    /// </summary>
    /// <returns>Type对象或 null.</returns>
    public Type? GetModuleType()
    {
        if (string.IsNullOrWhiteSpace(ModuleTypeFullName))
        {
            return null;
        }

        return Type.GetType(ModuleTypeFullName);
    }

    public ModuleInfo()
    {
    }

    public ModuleInfo(int moduleId, string moduleName, string? moduleTypeFullName, bool isEnabled, int? parentModuleId = null, string? icon = "")
        : this(UserAuthority.Operator, moduleId, moduleName, moduleTypeFullName, isEnabled, parentModuleId, icon)
    {
    }

    public ModuleInfo(UserAuthority authority, int moduleId, string moduleName, string? moduleTypeFullName, bool isEnabled, int? parentModuleId = null, string? icon = "")
    {
        Authority = authority;
        ModuleId = moduleId;
        ModuleName = moduleName;
        ParentModuleId = parentModuleId;
        ModuleTypeFullName = moduleTypeFullName ?? string.Empty;
        IsEnabled = isEnabled;
        Icon = icon;
        CreatedTime = DateTime.Now;
        LastModifiedTime = DateTime.Now;
        CreatedBy = string.Empty;
        ModifiedBy = string.Empty;
    }
}
