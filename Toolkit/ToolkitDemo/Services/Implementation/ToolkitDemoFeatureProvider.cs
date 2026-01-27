using BgCommon;
using BgCommon.Prism.Wpf.Authority.Core;
using BgCommon.Prism.Wpf.Modules;
using BgCommon.Prism.Wpf.Modules.Common.Views;
using BgCommon.Prism.Wpf.Modules.Logging.Views;
using ToolkitDemo.Views;

namespace ToolkitDemo.Services;

/// <summary>
/// 模块信息提供.
/// </summary>
internal class ToolkitDemoFeatureProvider : IFeatureProvider
{
    /// <inheritdoc/>
    public IEnumerable<FeatureSeedNode> GetFeatures()
    {
        // 在这里注册自定义模块
        string? defaultView = typeof(DefaultView).ToFullName();

        yield return new FeatureSeedNode(1, null, DemoModuleCodes.HomeView, "首页", typeof(HomeView).AssemblyQualifiedName, SystemRole.Operator)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.HomeView, "查看首页"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.HomeView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer, SystemRole.Operator },
            },
        };

        yield return new FeatureSeedNode(2, null, DemoModuleCodes.ProjectView, "文件管理", defaultView, SystemRole.Engineer)
        {
            Permissions = new List<PermissionSeed>()
            {
                new PermissionSeed(DemoPermissionCodes.ProjectView, "查看加工文件列表"),
                new PermissionSeed(DemoPermissionCodes.ProjectCreate, "创建加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectEdit, "编辑加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectDelete, "删除加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectImport, "导入加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectExport, "导出加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectRename, "重命名加工文件"),
                new PermissionSeed(DemoPermissionCodes.ProjectSaveAs, "另存为加工文件"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>()
            {
                [DemoPermissionCodes.ProjectView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectCreate] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectEdit] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectDelete] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectImport] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectExport] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectRename] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
                [DemoPermissionCodes.ProjectSaveAs] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
            },
            Children = new List<FeatureSeedNode>()
            {
                new FeatureSeedNode(200, 2, DemoModuleCodes.ProjectCreateView, "新建加工文件", defaultView, SystemRole.Engineer, null, false),
                new FeatureSeedNode(201, 2, DemoModuleCodes.ProjectEditView, "修改加工文件", defaultView, SystemRole.Engineer, null, false),
            },
        };

        yield return new FeatureSeedNode(3, null, DemoModuleCodes.VisionView, "视觉编辑", defaultView, SystemRole.Engineer)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.VisionView, "查看视觉编辑"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.VisionView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
            },
        };

        yield return new FeatureSeedNode(4, null, DemoModuleCodes.CADView, "CAD编辑", defaultView, SystemRole.Engineer)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.CADView, "查看CAD编辑"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.CADView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin, SystemRole.Engineer },
            },
        };

        yield return new FeatureSeedNode(5, null, DemoModuleCodes.ParameterView, "参数设置", defaultView, SystemRole.Admin)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.ParameterView, "查看参数设置"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.ParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
            },
            Children = new List<FeatureSeedNode>()
            {
                new FeatureSeedNode(500, 5, DemoModuleCodes.DispensingParameterView, "点胶参数", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.DispensingParameterView, "查看点胶参数"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.DispensingParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
                new FeatureSeedNode(501, 5, DemoModuleCodes.CoatingParameterView, "涂胶参数", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.CoatingParameterView, "查看涂胶参数"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.CoatingParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
                new FeatureSeedNode(502, 5, DemoModuleCodes.AdvancedFunctionsView, "高级功能", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.AdvancedFunctionsView, "查看高级功能"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.AdvancedFunctionsView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
                new FeatureSeedNode(503, 5, DemoModuleCodes.AlignmentParameterView, "对位参数", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.AlignmentParameterView, "查看对位参数"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.AlignmentParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
                new FeatureSeedNode(504, 5, DemoModuleCodes.VariableSpeedParameterView, "变速参数", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.VariableSpeedParameterView, "查看变速参数"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.VariableSpeedParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
                new FeatureSeedNode(505, 5, DemoModuleCodes.AdditionalParameterView, "其它参数", defaultView, SystemRole.Admin, null)
                {
                    Permissions = new List<PermissionSeed>
                    {
                        new (DemoPermissionCodes.AdditionalParameterView, "查看其它参数"),
                    },
                    DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
                    {
                        [DemoPermissionCodes.AdditionalParameterView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
                    },
                },
            },
        };

        yield return new FeatureSeedNode(6, null, DemoModuleCodes.DebugView, "调试维护", defaultView, SystemRole.Admin)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.DebugView, "查看调试维护"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.DebugView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
            },
        };

        yield return new FeatureSeedNode(7, null, DemoModuleCodes.LoggerView, "运行日志", typeof(LoggerMainView).ToFullName(), SystemRole.Operator)
        {
            Permissions = new List<PermissionSeed>
            {
                new (DemoPermissionCodes.LoggerView, "查看运行日志"),
            },
            DefaultRolePermissions = new Dictionary<string, IEnumerable<SystemRole>>
            {
                [DemoPermissionCodes.LoggerView] = new[] { SystemRole.SuperAdmin, SystemRole.Admin },
            },
        };
    }
}
