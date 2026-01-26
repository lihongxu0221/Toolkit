namespace BgCommon.Prism.Wpf.Authority;

/// <summary>
/// 自定义特性，用于标记一个 SystemRole 枚举成员
/// 是否被允许用于无密码的系统角色登录.
/// </summary>
[AttributeUsage(AttributeTargets.Field)] // 这个特性只能用在枚举的字段上
public sealed class AllowSystemLoginAttribute : Attribute
{
}