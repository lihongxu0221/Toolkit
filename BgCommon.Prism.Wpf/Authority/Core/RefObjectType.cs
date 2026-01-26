using BgCommon.Localization.ComponentModel;

namespace BgCommon.Prism.Wpf.Authority.Core;

/// <summary>
/// 引用对象类型枚举（用于标识系统中被引用对象的具体类型）.
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum RefObjectType
{
    /// <summary>
    /// 模块（表示应用程序或系统中的一个独立模块，通常包含一组相关功能）.
    /// </summary>
    [Display(Name = "模块")]
    Module,

    /// <summary>
    /// 功能（指定可授予系统用户或角色的一组访问权限或操作权限，对应具体业务功能）.
    /// </summary>
    [Display(Name = "功能")]
    Permission,

    /// <summary>
    /// 参数（表示系统配置或运行时使用的参数设置）.
    /// </summary>
    [Display(Name = "参数")]
    Parameter,
}