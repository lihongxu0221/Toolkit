using BgCommon.Localization.ComponentModel;

namespace BgCommon.Prism.Wpf.Authority.Core;

/// <summary>
/// 定义了系统配置参数支持的值类型.
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum ParameterValueType
{
    /// <summary>
    /// 字符串类型.
    /// </summary>
    [Display(Name = "字符串")]
    String = 0,

    /// <summary>
    /// 长整数类型.
    /// </summary>
    [Display(Name = "长整数")]
    Long = 64,

    /// <summary>
    /// 整数类型.
    /// </summary>
    [Display(Name = "整数")]
    Int = 32,

    /// <summary>
    /// 短整数类型.
    /// </summary>
    [Display(Name = "短整数")]
    Short = 16,

    /// <summary>
    /// 字节类型.
    /// </summary>
    [Display(Name = "字节")]
    Byte = 8,

    /// <summary>
    /// 双精度浮点数类型.
    /// </summary>
    [Display(Name = "双精度浮点数")]
    Double = 65,

    /// <summary>
    /// 单精度浮点数类型.
    /// </summary>
    [Display(Name = "单精度浮点数")]
    Float = 33,

    /// <summary>
    /// 枚举类型.
    /// </summary>
    [Display(Name = "枚举")]
    Enum = 1,

    /// <summary>
    /// 布尔值类型.
    /// </summary>
    [Display(Name = "布尔值")]
    Bool = 2,

    /// <summary>
    /// 时间日期类型.
    /// </summary>
    [Display(Name = "时间日期")]
    DateTime = 3,

    /// <summary>
    /// 坐标点位类型.
    /// </summary>
    [Display(Name = "坐标点位")]
    Position = 99,

    /// <summary>
    /// 数组类型.
    /// </summary>
    [Display(Name = "数组")]
    Array = 4,

    /// <summary>
    /// 集合类型.
    /// </summary>
    [Display(Name = "集合")]
    List = 5,
}