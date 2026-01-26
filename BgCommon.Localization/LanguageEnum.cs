using BgCommon.Localization.Attributes;
using BgCommon.Localization.ComponentModel;

namespace BgCommon.Localization;

/// <summary>
/// 多语言对应的枚举信息
/// </summary>
[TypeConverter(typeof(EnumLocalizationConverter))]
public enum LanguageEnum : int
{
    /// <summary>
    /// 中文简体
    /// </summary>
    [Display(Name = "zh_CN", Description = "zh_CN_Remark")]
    Chinese = 2052,

    /// <summary>
    /// 中文台湾
    /// </summary>
    [Display(Name = "zh_TW", Description = "zh_TW_Remark")]
    ChineseTW = 1028,

    /// <summary>
    /// 英语 - 美国
    /// </summary>
    [Display(Name = "en_US", Description = "en_US_Remark")]
    EnglishUS = 1033,

    /// <summary>
    /// 越南 - 越南
    /// </summary>
    [Display(Name = "vi", Description = "en_US_Remark")]
    Vietnam = 704
}