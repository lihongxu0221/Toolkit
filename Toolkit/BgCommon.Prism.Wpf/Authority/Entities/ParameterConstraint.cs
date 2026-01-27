namespace BgCommon.Prism.Wpf.Authority.Entities;

/// <summary>
/// 系统参数的约束规则实体.
/// </summary>
public partial class ParameterConstraint : ObservableValidator, ICloneable, ISelfValidator
{
    /// <summary>
    /// Gets or sets 约束Id (主键).
    /// </summary>
    [Key]
    [ObservableProperty]
    private long id;

    /// <summary>
    /// Gets or sets 此约束所属的参数ID (外键).
    /// </summary>
    [Required]
    [ObservableProperty]
    private long parameterId;

    /// <summary>
    /// Gets or sets 约束的类型.
    /// </summary>
    [Required]
    [ObservableProperty]
    private ConstraintType type;

    /// <summary>
    /// Gets or sets 约束的值.
    /// 例如: 对于 MinValue, 值为 "0"; 对于 Regex, 值为 "^[a-zA-Z]+$".
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "约束值不能为空")]
    [ObservableProperty]
    private string value = string.Empty;

    /// <summary>
    /// Gets or sets 对此约束的描述，可以用于UI提示 (可选).
    /// </summary>
    [MaxLength(255)]
    [ObservableProperty]
    private string? description;

    /// <summary>
    /// 参数是否启用.
    /// </summary>
    [ObservableProperty]
    private bool isEnable = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterConstraint"/> class.
    /// </summary>
    public ParameterConstraint()
    {
    }

    /// <summary>
    /// Gets or sets 导航属性：所属的系统参数.
    /// </summary>
    [ForeignKey(nameof(ParameterId))]
    public virtual SystemParameter? SystemParameter { get; set; }

    /// <inheritdoc/>
    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <inheritdoc/>
    public void Validate()
    {
        this.ValidateAllProperties();
    }
}