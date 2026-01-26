namespace BgCommon.Prism.Wpf.Modules.Parameters.Models;

/// <summary>
/// 参数约束用于UI显示的实体类.
/// </summary>
public partial class ParameterConstraintDisplay : ObservableObject
{
    /// <summary>
    /// Gets 原始的参数约束信息 (只读)
    /// </summary>
    public ParameterConstraint Constraint { get; }

    /// <summary>
    /// 约束的类型.
    /// </summary>
    [ObservableProperty]
    private ConstraintType type;

    /// <summary>
    /// 约束的值.
    /// 例如: 对于 MinValue, 值为 "0"; 对于 Regex, 值为 "^[a-zA-Z]+$".
    /// </summary>
    [ObservableProperty]
    private string value = string.Empty;

    /// <summary>
    /// 对此约束的描述，可以用于UI提示 (可选).
    /// </summary>
    [ObservableProperty]
    private string? description = string.Empty;

    public ParameterConstraintDisplay(ParameterConstraint constraint)
    {
        this.Constraint = constraint;

        // 初始化UI的勾选状态
        this.Type = constraint.Type;
        this.Value = constraint.Value;
        this.Description = constraint.Description;
    }
}