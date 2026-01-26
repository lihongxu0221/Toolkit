namespace BgCommon.Prism.Wpf.Modules;

/// <summary>
/// 参数约束定义的轻量级数据传输对象 (DTO).
/// </summary>
/// <param name="Type">约束类型.</param>
/// <param name="Value">约束值.</param>
/// <param name="Description">备注.</param>
public record ParameterConstraintSeed(ConstraintType Type, object? Value, string Description = "");