using BgCommon.Localization.ComponentModel;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// 定义了可以进行数据权限校验的操作类型.
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum DataOperation
{
    /// <summary>
    /// 查询操作，通常用于读取数据.
    /// </summary>
    [Display(Name ="查询")]
    Select,

    /// <summary>
    /// 创建操作，通常用于新增数据记录.
    /// </summary>
    [Display(Name = "创建")]
    Create,

    /// <summary>
    /// 更新操作，通常用于修改现有数据记录.
    /// </summary>
    [Display(Name = "更新")]
    Update,

    /// <summary>
    /// 删除操作，通常用于删除数据记录.
    /// </summary>
    [Display(Name = "删除")]
    Delete,
}
