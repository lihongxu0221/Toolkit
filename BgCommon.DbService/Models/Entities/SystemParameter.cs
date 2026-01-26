namespace BgCommon.DbService.Models.Entities;

/// <summary>
/// 系统配置参数实体.
/// </summary>
public partial class SystemParameter :
    ObservableValidator,
    ICloneable,
    ISelfValidator
{
    /// <summary>
    /// Gets or sets 参数Id (主键).
    /// </summary>
    [Key]
    [ObservableProperty]
    private long id;

    /// <summary>
    /// Gets or sets 参数编码 (应唯一).
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "参数编码不能为空")]
    [MaxLength(100)]
    [ObservableProperty]
    private string code = string.Empty;

    /// <summary>
    /// Gets or sets 参数显示名称 (应唯一).
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "参数名称不能为空")]
    [MaxLength(100)]
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// Gets or sets 参数值 (以字符串形式存储，在应用层进行类型转换).
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "参数值不能为空")]
    [ObservableProperty]
    private string value = string.Empty;

    /// <summary>
    /// 最低可见权限.
    /// </summary>
    [Required]
    [ObservableProperty]
    private SystemRole minVisibleRole = SystemRole.Operator;

    /// <summary>
    /// 最低可编辑权限.
    /// </summary>
    [Required]
    [ObservableProperty]
    private SystemRole minEditableRole = SystemRole.Operator;

    /// <summary>
    /// 参数分类.
    /// </summary>
    [ObservableProperty]
    private string? category = string.Empty;

    /// <summary>
    /// 描述备注.
    /// </summary>
    [ObservableProperty]
    private string? description = string.Empty;

    /// <summary>
    /// 父节点.
    /// </summary>
    [ObservableProperty]
    private string? parentNode = string.Empty;

    /// <summary>
    /// 父节点显示名称.
    /// </summary>
    [ObservableProperty]
    private string? parentDisplayName = string.Empty;

    // /// <summary>
    // /// Gets or sets 参数值的类型.
    // /// </summary>
    // [Required]
    // [ObservableProperty]
    // private EditorTypeCode valueType = EditorTypeCode.PlainText;

    /// <summary>
    /// Gets or sets 所属模块Id (外键, 可选).
    /// </summary>
    [ObservableProperty]
    private long? moduleId;

    /// <summary>
    /// 参数是否启用.
    /// </summary>
    [ObservableProperty]
    private bool isEnable = true;

    /// <summary>
    /// Gets or sets 创建人.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [ObservableProperty]
    private string createdBy = string.Empty;

    /// <summary>
    /// Gets or sets 创建时间.
    /// </summary>
    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;

    /// <summary>
    /// Gets or sets 修改人.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [ObservableProperty]
    private string modifiedBy = string.Empty;

    /// <summary>
    /// Gets or sets 最后修改时间.
    /// </summary>
    [ObservableProperty]
    private DateTime lastModifiedAt = DateTime.Now;

    /// <summary>
    /// 所属顶层父节点名称(VM中参数实例名称).
    /// </summary>
    [ObservableProperty]
    private string rootName = string.Empty;

    /// <summary>
    /// 所属顶层父节点显示名称(VM中参数实例名称).
    /// </summary>
    [ObservableProperty]
    private string rootDisplayName = string.Empty;

    /// <summary>
    /// 排序序号.
    /// </summary>
    [ObservableProperty]
    private int sortIndex = 0;

    // --- 导航属性 ---

    /// <summary>
    /// Gets or sets 导航属性：所属模块.
    /// </summary>
    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleInfo? ModuleInfo { get; set; }

    /// <summary>
    /// Gets or sets 导航属性：与此参数关联的所有约束规则.
    /// </summary>
    public virtual ICollection<ParameterConstraint> Constraints { get; set; } = new List<ParameterConstraint>();

    /// <summary>
    /// Gets 防重复组合Key.
    /// </summary>
    [NotMapped]
    public string CompositeKey => $"{this.Code}|{this.ModuleId ?? 0}";

    /// <summary>
    /// 克隆.
    /// </summary>
    /// <returns>返回克隆结果.</returns>
    public object Clone()
    {
        return this.MemberwiseClone();
    }

    /// <summary>
    /// 校验.
    /// </summary>
    public void Validate()
    {
        this.ValidateAllProperties();
    }

    /// <summary>
    /// 更新数据.
    /// </summary>
    /// <param name="parameter">源参数.</param>
    public void Update(SystemParameter parameter)
    {
        this.Name = parameter.Name;
        this.Value = parameter.Value;

        // this.ValueType = parameter.ValueType;
        this.MinEditableRole = parameter.MinEditableRole;
        this.MinVisibleRole = parameter.MinVisibleRole;
        this.ModuleId = parameter.ModuleId;
        this.Category = parameter.Category;
        this.Description = parameter.Description;
        this.IsEnable = parameter.IsEnable;
        this.SortIndex = parameter.SortIndex;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj) || obj == null)
        {
            return false;
        }

        if (obj is SystemParameter incomingParam)
        {
            return this.MinVisibleRole == incomingParam.MinVisibleRole &&
                   this.MinEditableRole == incomingParam.MinEditableRole &&
                   this.Name == incomingParam.Name &&
                   this.Value == incomingParam.Value &&

                   // this.ValueType == incomingParam.ValueType &&
                   this.IsEnable == incomingParam.IsEnable &&
                   this.Category == incomingParam.Category &&
                   this.Description == incomingParam.Description &&
                   this.SortIndex == incomingParam.SortIndex &&
                   AreConstraintsEqual(this.Constraints, incomingParam.Constraints);
        }

        return base.Equals(obj);
    }

    /// <summary>
    /// 比较两组参数约束是否相等 (忽略顺序).
    /// </summary>
    private bool AreConstraintsEqual(ICollection<ParameterConstraint> c1, ICollection<ParameterConstraint> c2)
    {
        if (c1 == null && c2 == null)
        {
            return true;
        }

        if (c1 == null || c2 == null)
        {
            return false;
        }

        if (c1.Count != c2.Count)
        {
            return false;
        }

        // 使用 HashSet 提高比较效率，它不关心元素顺序
        var set1 = c1.Select(c => (c.Type, c.Value)).ToHashSet();
        var set2 = c2.Select(c => (c.Type, c.Value)).ToHashSet();

        return set1.SetEquals(set2);
    }
}