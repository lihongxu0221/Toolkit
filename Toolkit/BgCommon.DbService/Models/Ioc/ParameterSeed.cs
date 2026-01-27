namespace BgCommon.DbService.Models.Ioc;

/// <summary>
/// 参数定义的轻量级数据传输对象 (DTO).
/// </summary>
public class ParameterSeed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSeed"/> class.
    /// </summary>
    /// <param name="rootName">父节点编码.</param>
    /// <param name="rootDisplayName">根节点编码.</param>
    /// <param name="parentCode">根节点显示名称.</param>
    /// <param name="code">编码.</param>
    /// <param name="displayName">显示名称.</param>
    /// <param name="value">值.</param>
    /// <param name="valueType">值类型.</param>
    /// <param name="category">分组.</param>
    /// <param name="isEnabled">是否启用.</param>
    /// <param name="constraints">参数约束.</param>
    public ParameterSeed(
        string rootName,
        string rootDisplayName,
        string parentCode,
        string code,
        string displayName,
        string value,
        EditorTypeCode valueType,
        string category,
        bool isEnabled,
        ICollection<ParameterConstraintSeed>? constraints = null)
    {
        this.RootName = rootName;
        this.RootDisplayName = rootDisplayName;
        this.ParentCode = parentCode;
        this.Code = code;
        this.DisplayName = displayName;
        this.Value = value;
        this.ValueType = valueType;
        this.Category = category;
        this.IsEnabled = isEnabled;
        this.Constraints = constraints;
    }

    /// <summary>
    /// Gets 根节点编码.
    /// </summary>
    public string RootName { get; }

    /// <summary>
    /// Gets 根节点显示名称.
    /// </summary>
    public string RootDisplayName { get; }

    /// <summary>
    /// Gets 父节点编码.
    /// </summary>
    public string ParentCode { get; }

    /// <summary>
    /// Gets 编码.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets 显示名称.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets 值.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets 值类型.
    /// </summary>
    public EditorTypeCode ValueType { get; }

    /// <summary>
    /// Gets 分组.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets a value indicating whether 是否启用.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets or sets 参数约束.
    /// </summary>
    public ICollection<ParameterConstraintSeed>? Constraints { get; set; }
}