using BgCommon.Localization.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BgCommon.Authority;

[TypeConverter(typeof(EnumLocalizationConverter))]
public enum UserAuthority
{
    [Display(Name ="操作员")]
    Operator = 1,

    [Display(Name = "工程师")]
    Engineer = 60,

    [Display(Name = "管理员")]
    Admin = 99,
}