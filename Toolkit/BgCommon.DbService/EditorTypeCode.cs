namespace BgCommon.DbService;

/// <summary>
/// 编辑器类型代码枚举.
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum EditorTypeCode
{
    /// <summary>
    /// 纯文本编辑器.
    /// </summary>
    [Display(Name = "纯文本")] PlainText = 1,

    /// <summary>
    /// 文本只读编辑器.
    /// </summary>
    [Display(Name = "文本只读")] ReadOnlyPlainText = 2,

    /// <summary>
    /// 枚举类型.
    /// </summary>
    [Display(Name = "枚举")] Enum = 3,

    /// <summary>
    /// 开关编辑器.
    /// </summary>
    [Display(Name = "布尔值")] Switch = 4,

    /// <summary>
    /// 日期时间编辑器.
    /// </summary>
    [Display(Name = "时间日期")] DateTime = 5,

    /// <summary>
    /// 有符号字节数字编辑器.
    /// </summary>
    [Display(Name = "字节")] SByteNumber = 8,

    /// <summary>
    /// 无符号字节数字编辑器.
    /// </summary>
    [Display(Name = "字节(无符号)")] ByteNumber = 9,

    /// <summary>
    /// 16位有符号整数编辑器.
    /// </summary>
    [Display(Name = "短整数")] Int16Number = 16,

    /// <summary>
    /// 16位无符号整数编辑器.
    /// </summary>
    [Display(Name = "短整数(无符号)")] UInt16Number = 17,

    /// <summary>
    /// 32位有符号整数编辑器.
    /// </summary>
    [Display(Name = "整数")] Int32Number = 32,

    /// <summary>
    /// 32位无符号整数编辑器.
    /// </summary>
    [Display(Name = "整数(无符号)")] UInt32Number = 33,

    /// <summary>
    /// 64位有符号整数编辑器.
    /// </summary>
    [Display(Name = "长整数")] Int64Number = 64,

    /// <summary>
    /// 64位无符号整数编辑器.
    /// </summary>
    [Display(Name = "长整数(无符号)")] UInt64Number = 65,

    /// <summary>
    /// 单精度浮点数编辑器.
    /// </summary>
    [Display(Name = "单精度浮点数")] FloatNumber = 66,

    /// <summary>
    /// 双精度浮点数编辑器.
    /// </summary>
    [Display(Name = "双精度浮点数")] DoubleNumber = 67,

    /// <summary>
    /// 水平对齐方式编辑器.
    /// </summary>
    [Display(Name = "水平对齐")] HorizontalAlignment = 68,

    /// <summary>
    /// 垂直对齐方式编辑器.
    /// </summary>
    [Display(Name = "垂直对齐")] VerticalAlignment = 69,

    /// <summary>
    /// 图像源编辑器.
    /// </summary>
    [Display(Name = "图像源")] ImageSource = 70,

    /// <summary>
    /// 列表编辑器.
    /// </summary>
    [Display(Name = "列表")] Array = 71,
}