namespace BgCommon.DbService.Models.Dtos;

/// <summary>
/// 用于配置的参数行.
/// </summary>
public partial class SystemParameterDisplay : ObservableObject
{
    /// <summary>
    /// 参数Id (主键).
    /// </summary>
    [ObservableProperty]
    private long id;

    /// <summary>
    /// 参数编码 (应唯一).
    /// </summary>
    [ObservableProperty]
    private string code = string.Empty;

    /// <summary>
    /// 参数显示名称 (应唯一).
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// 参数值 (以字符串形式存储，在应用层进行类型转换).
    /// </summary>
    [ObservableProperty]
    private string value = string.Empty;

    /// <summary>
    /// 排序序号.
    /// </summary>
    [ObservableProperty]
    private int sortIndex = 0;

    // /// <summary>
    // /// 参数值的类型.
    // /// </summary>
    // [ObservableProperty]
    // private EditorTypeCode valueType = EditorTypeCode.PlainText;

    /// <summary>
    /// 最低可见权限.
    /// </summary>
    [ObservableProperty]
    private SystemRole minVisibleRole = SystemRole.Operator;

    /// <summary>
    /// 最低可编辑权限.
    /// </summary>
    [ObservableProperty]
    private SystemRole minEditableRole = SystemRole.Operator;

    /// <summary>
    /// a value indicating whether 对于当前查询的用户，此参数是否可编辑.
    /// </summary>
    [ObservableProperty]
    private bool isEditable = true;

    /// <summary>
    /// 小数位数.
    /// </summary>
    [ObservableProperty]
    private int? decimalPlaces = null;

    /// <summary>
    /// 最小值.
    /// </summary>
    [ObservableProperty]
    private double? minimum = null;

    /// <summary>
    /// 最大值.
    /// </summary>
    [ObservableProperty]
    private double? maximum = null;

    /// <summary>
    /// 步长.
    /// </summary>
    [ObservableProperty]
    private double? increment = null;

    /// <summary>
    /// 所属模块Id (外键; 可选).
    /// </summary>
    [ObservableProperty]
    private long? moduleId;

    /// <summary>
    /// 参数是否启用.
    /// </summary>
    [ObservableProperty]
    private bool isEnable = true;

    /// <summary>
    /// 创建人.
    /// </summary>
    [ObservableProperty]
    private string createdBy = string.Empty;

    /// <summary>
    /// 创建时间.
    /// </summary>
    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;

    /// <summary>
    /// 修改人.
    /// </summary>
    [ObservableProperty]
    private string modifiedBy = string.Empty;

    /// <summary>
    /// 最后修改时间.
    /// </summary>
    [ObservableProperty]
    private DateTime lastModifiedAt = DateTime.Now;

    /// <summary>
    /// 所属顶层父节点(VM中参数实例名称).
    /// </summary>
    [ObservableProperty]
    private string rootName = string.Empty;

    /// <summary>
    /// 所属顶层父节点显示名称(VM中参数实例名称).
    /// </summary>
    [ObservableProperty]
    private string rootDisplayName = string.Empty;

    /// <summary>
    /// 所属父节点.
    /// </summary>
    [ObservableProperty]
    private string? parentNode = string.Empty;

    /// <summary>
    /// 所属父节点显示名称.
    /// </summary>
    [ObservableProperty]
    private string? parentDisplayName = string.Empty;

    /// <summary>
    /// 参数分类.
    /// </summary>
    [ObservableProperty]
    private string category = string.Empty;

    /// <summary>
    /// 描述备注.
    /// </summary>
    [ObservableProperty]
    private string? description = string.Empty;

    /// <summary>
    /// Gets or sets 所属模块.
    /// </summary>
    public ModuleInfo? Module { get; set; }

    /// <summary>
    /// Gets or sets 与此参数关联的所有约束规则.
    /// </summary>
    public ICollection<ParameterConstraint> Constraints { get; set; } = new List<ParameterConstraint>();

    /// <summary>
    /// Gets or sets 参数实体.
    /// </summary>
    public SystemParameter? Parameter { get; set; }

    /// <summary>
    /// Gets or sets 属性描述符.
    /// </summary>
    public PropertyDescriptor? Descriptor { get; set; }

    /// <summary>
    /// Gets a value indicating whether gets 是否为集合类型的属性.
    /// </summary>
    public bool IsArray => this.Descriptor?.PropertyType.IsArray() ?? false;

    /// <summary>
    /// Gets 如果属性为集合则返回集合的长度.
    /// </summary>
    public int? ArrayLength
    {
        get
        {
            if (this.IsArray)
            {
                return this.Value.Split(',').Length;
            }

            return null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemParameterDisplay"/> class.
    /// </summary>
    public SystemParameterDisplay()
    {
    }

    partial void OnMaximumChanged(double? value)
    {
        this.OnConstrainChanged(ConstraintType.Maximum, value);
    }

    partial void OnMinimumChanged(double? value)
    {
        this.OnConstrainChanged(ConstraintType.Minimum, value);
    }

    partial void OnDecimalPlacesChanged(int? value)
    {
        this.OnConstrainChanged(ConstraintType.DecimalPlaces, value);
    }

    partial void OnIncrementChanged(double? value)
    {
        this.OnConstrainChanged(ConstraintType.Increment, value);
    }

    /// <summary>
    /// 约束变更后，自动刷新或移除约束条件.
    /// </summary>
    /// <typeparam name="T">约束值类型.</typeparam>
    /// <param name="constraintType">约束类型.</param>
    /// <param name="value">约束变更值.</param>
    private void OnConstrainChanged<T>(ConstraintType constraintType, T? value)
    {
        var existItem = this.Constraints.FirstOrDefault(c => c.Type == constraintType);
        if (existItem != null)
        {
            if (value != null)
            {
                existItem.Value = Convert.ToString(value) ?? string.Empty;
            }
            else
            {
                this.Constraints.Remove(existItem);
            }
        }
        else
        {
            if (value != null)
            {
                this.Constraints.Add(new ParameterConstraint()
                {
                    Id = 0,
                    ParameterId = this.Id,
                    Type = constraintType,
                    Value = value.ToString() ?? string.Empty,
                    IsEnable = true,
                });
            }
        }
    }

    /// <summary>
    /// 展示参数实例转换为参数存储实例.
    /// </summary>
    /// <param name="display">展示参数实例.</param>
    /// <returns>返回参数存储实例.</returns>
    public static explicit operator SystemParameter(SystemParameterDisplay display)
    {
        var parameter = new SystemParameter();
        parameter.Id = display.Id;
        parameter.Code = display.Code;
        parameter.Name = display.Name;
        parameter.Value = display.Value;

        // parameter.ValueType = display.ValueType;
        parameter.MinVisibleRole = display.MinVisibleRole;
        parameter.MinEditableRole = display.MinEditableRole;
        parameter.ModuleId = display.ModuleId;
        parameter.IsEnable = display.IsEnable;
        parameter.CreatedBy = display.CreatedBy;
        parameter.CreatedAt = display.CreatedAt;
        parameter.ModifiedBy = display.ModifiedBy;
        parameter.LastModifiedAt = display.LastModifiedAt;
        parameter.RootName = display.RootName;
        parameter.RootDisplayName = display.RootDisplayName;
        parameter.ParentNode = display.ParentNode;
        parameter.ParentDisplayName = display.ParentDisplayName;
        parameter.Category = display.Category;
        parameter.Description = display.Description;
        parameter.SortIndex = display.SortIndex;

        parameter.Constraints = new List<ParameterConstraint>();
        foreach (var constraint in display.Constraints)
        {
            var constraintNew = (ParameterConstraint)constraint.Clone();
            constraintNew.SystemParameter = constraint.SystemParameter ?? parameter;
            parameter.Constraints.Add(constraintNew);
        }

        return parameter;
    }

    /// <summary>
    /// 参数存储实例转换为展示参数实例.
    /// </summary>
    /// <param name="parameter">参数存储实例.</param>
    public static explicit operator SystemParameterDisplay(SystemParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        SystemParameterDisplay display = new SystemParameterDisplay();
        display.Id = parameter.Id;
        display.Code = parameter.Code;
        display.Name = parameter.Name;
        display.Value = parameter.Value;

        // display.ValueType = parameter.ValueType;
        display.MinVisibleRole = parameter.MinVisibleRole;
        display.MinEditableRole = parameter.MinEditableRole;

        // disPlay.IsEditable = parameter.IsEnable; //是否可编辑，取决于权限比对。
        display.ModuleId = parameter.ModuleId;
        display.IsEnable = parameter.IsEnable;
        display.CreatedBy = parameter.CreatedBy;
        display.CreatedAt = parameter.CreatedAt;
        display.ModifiedBy = parameter.ModifiedBy;
        display.LastModifiedAt = parameter.LastModifiedAt;
        display.RootName = parameter.RootName;
        display.RootDisplayName = parameter.RootDisplayName;
        display.ParentNode = parameter.ParentNode;
        display.ParentDisplayName = parameter.ParentDisplayName;
        display.Category = parameter.Category;
        display.Description = parameter.Description;
        display.SortIndex = parameter.SortIndex;

        display.Module = (ModuleInfo?)parameter.ModuleInfo?.Clone();
        display.Parameter = parameter;
        display.Constraints = new List<ParameterConstraint>();
        foreach (ParameterConstraint constraint in parameter.Constraints)
        {
            var contraintDispay = (ParameterConstraint)constraint.Clone();
            contraintDispay.SystemParameter = parameter;
            display.Constraints.Add(contraintDispay);
            switch (constraint.Type)
            {
                case ConstraintType.Minimum:
                    display.Minimum = double.Parse(constraint.Value);
                    break;
                case ConstraintType.Maximum:
                    display.Maximum = double.Parse(constraint.Value);
                    break;
                case ConstraintType.DecimalPlaces:
                    display.DecimalPlaces = int.Parse(constraint.Value);
                    break;
                case ConstraintType.Increment:
                    display.Increment = double.Parse(constraint.Value);
                    break;
                case ConstraintType.Regex:
                case ConstraintType.OptionsSource:
                case ConstraintType.MaxLength:
                default:
                    break;
            }
        }

        return display;
    }
}