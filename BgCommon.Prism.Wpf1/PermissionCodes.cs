namespace BgCommon.Prism.Wpf;

/// <summary>
/// 定义系统中所有权限码的常量.
/// </summary>
public class PermissionCodes
{
    // 用户管理
    public const string UserView = "User.View";
    public const string UserCreate = "User.Create";
    public const string UserEdit = "User.Edit";
    public const string UserDelete = "User.Delete";

    // 角色管理
    public const string RoleView = "Role.View";
    public const string RoleCreate = "Role.Create";
    public const string RoleEdit = "Role.Edit";
    public const string RoleDelete = "Role.Delete";

    // 模块管理
    public const string ModuleView = "Module.View";
    public const string ModuleCreate = "Module.Create";
    public const string ModuleEdit = "Module.Edit";
    public const string ModuleDelete = "Module.Delete";
}