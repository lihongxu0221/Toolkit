namespace BgCommon.Script;

/// <summary>
/// 视图操作权限.
/// </summary>
public class PermissionCodes : Prism.Wpf.PermissionCodes
{
    private const string Prefix = "BgCommon.RoslynPad";

    /// <summary>
    /// 脚本编辑视图查看.
    /// </summary>
    public const string ScriptEditView = $"{Prefix}.ScriptEdit.View";
}
