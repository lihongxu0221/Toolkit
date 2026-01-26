namespace ToolkitDemo;

/// <summary>
/// 模块视图编码.
/// </summary>
public class DemoModuleCodes
{
    public const string HomeView = nameof(HomeView);
    public const string ProjectView = nameof(ProjectView);
    public const string ProjectCreateView = nameof(ProjectCreateView);
    public const string ProjectEditView = nameof(ProjectEditView);
    public const string VisionView = nameof(VisionView);
    public const string CADView = nameof(CADView);
    public const string ParameterView = nameof(ParameterView);
    public const string DispensingParameterView = nameof(DispensingParameterView);
    public const string CoatingParameterView = nameof(CoatingParameterView);
    public const string AdvancedFunctionsView = nameof(AdvancedFunctionsView);
    public const string AlignmentParameterView = nameof(AlignmentParameterView);
    public const string VariableSpeedParameterView = nameof(VariableSpeedParameterView);
    public const string AdditionalParameterView = nameof(AdditionalParameterView);
    public const string DebugView = nameof(DebugView);
    public const string LoggerView = nameof(LoggerView);
}

/// <summary>
/// 操作权限编码.
/// </summary>
public class DemoPermissionCodes
{
    public const string PermissionPrefix = "ToolkitDemo.";

    public const string HomeView = $"{PermissionPrefix}Home.View";

    public const string ProjectView = $"{PermissionPrefix}Project.View";

    public const string ProjectCreate = $"{PermissionPrefix}Project.Create";

    public const string ProjectEdit = $"{PermissionPrefix}Project.Edit";

    public const string ProjectDelete = $"{PermissionPrefix}Project.Delete";

    public const string ProjectImport = $"{PermissionPrefix}Project.Import";

    public const string ProjectExport = $"{PermissionPrefix}Project.Export";

    public const string ProjectRename = $"{PermissionPrefix}Project.Rename";

    public const string ProjectSaveAs = $"{PermissionPrefix}Project.SaveAs";

    public const string VisionView = $"{PermissionPrefix}Vision.View";

    public const string CADView = $"{PermissionPrefix}CAD.View";

    public const string ParameterView = $"{PermissionPrefix}Parameter.View";

    public const string DispensingParameterView = $"{PermissionPrefix}DispensingParameter.View";

    public const string CoatingParameterView = $"{PermissionPrefix}CoatingParameter.View";

    public const string AdvancedFunctionsView = $"{PermissionPrefix}AdvancedFunctions.View";

    public const string AlignmentParameterView = $"{PermissionPrefix}AlignmentParameter.View";

    public const string VariableSpeedParameterView = $"{PermissionPrefix}VariableSpeedParameter.View";

    public const string AdditionalParameterView = $"{PermissionPrefix}AdditionalParameter.View";

    public const string DebugView = $"{PermissionPrefix}Debug.View";

    public const string LoggerView = $"{PermissionPrefix}Logger.View";
}