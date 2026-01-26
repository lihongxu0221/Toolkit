namespace BgCommon.DbService;

/// <summary>
/// 定义了系统参数支持的约束类型.
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum ConstraintType
{
    /// <summary>
    /// 最小值约束 (数值或日期).
    /// </summary>
    [Display(Name = "最小值约束(数值或日期)")]
    Minimum,

    /// <summary>
    /// 最大值约束 (数值或日期).
    /// </summary>
    [Display(Name = "最大值约束(数值或日期)")]
    Maximum,

    /// <summary>
    /// 正则表达式约束 (字符串).
    /// </summary>
    [Display(Name = "正则表达式")]
    Regex,

    /// <summary>
    /// 定义了下拉列表的数据源.
    /// 值可以是逗号分隔的列表 ("A,B,C") 或一个API端点/查询标识符.
    /// </summary>
    [Display(Name = "下拉列表数据源")]
    OptionsSource,

    /// <summary>
    /// 字符串的最大长度.
    /// </summary>
    [Display(Name = "字符串的最大长度")]
    MaxLength,

    /// <summary>
    /// 小数点位数.
    /// </summary>
    [Display(Name = "小数点位数")]
    DecimalPlaces,

    /// <summary>
    /// 步长.
    /// </summary>
    [Display(Name = "步长")]
    Increment,
}