//using BgCommon.Prism.Wpf.Authority;
//using BgCommon.Prism.Wpf.Authority.Modules.User.Views;
//using BgCommon.Prism.Wpf.Authority.Services.Implementation;
//using BgCommon.Prism.Wpf.Common.Views;
//using ToolkitDemo.Views;
//using ModuleInfo = BgCommon.Prism.Wpf.Authority.Entities.ModuleInfo;

//namespace ToolkitDemo.Services;

//internal class ModuleService : ModuleServiceBase
//{
//    public ModuleService(IContainerExtension container)
//        : base(container)
//    {
//    }

//    public override Task<bool> InitializeAsync()
//    {
//        string? defaultView = typeof(DefaultView).AssemblyQualifiedName;

//        this.Modules.Clear();
//        this.Modules.Add(new ModuleInfo(UserAuthority.Operator, 1, "首页", typeof(Views.HomeView).AssemblyQualifiedName, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 2, "文件管理", defaultView, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 3, "视觉编辑", typeof(Views.ModuleHostView).AssemblyQualifiedName, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 4, "CAD编辑", defaultView, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 5, "参数设置", typeof(Views.ModuleHostView).AssemblyQualifiedName, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 6, "调试维护", typeof(Views.ModuleHostView).AssemblyQualifiedName, true, 0));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Operator, 7, "运行日志", typeof(BgLogger.Logging.Views.MainView).AssemblyQualifiedName, true, 0));

//        // 视觉模块
//        this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 30, "视觉编辑", defaultView, true, 3));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Operator, 31, "视觉说明", defaultView, true, 3));

//        // CAD模块
//        // this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 40, "曲线编辑", typeof(CADEditView).AssemblyQualifiedName, true, 4));
//        // this.Modules.Add(new ModuleInfo(UserAuthority.Operator, 41, "编辑说明", typeof(CADEditInstructionView).AssemblyQualifiedName, true, 4));

//        // 参数设置
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 50, "点胶参数", defaultView, true, 5));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 51, "涂胶参数", defaultView, true, 5));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 52, "高级功能", defaultView, true, 5));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 53, "对位参数", defaultView, true, 5));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 54, "变速参数", defaultView, true, 5));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 55, "其它参数", defaultView, true, 5));

//        // 调试维护
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 60, "硬件调试", defaultView, true, 6));

//        // this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 61, "轴参数", typeof(MotionAxisSettingView).AssemblyQualifiedName, true, 6));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 62, "轴/IO配置", defaultView, true, 6));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 63, "IO面板", defaultView, true, 6));
//        // this.Modules.Add(new ModuleInfo(UserAuthority.Engineer, 64, "运行日志", typeof(BgLogger.Logging.Views.MainView).AssemblyQualifiedName, true, 6));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 64, "相机标定", defaultView, true, 6));

//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 65, "软件维护", defaultView, true, 6));
//        this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 66, "用户维护", typeof(UserManagementView).AssemblyQualifiedName, true, 6));

//        // this.Modules.Add(new ModuleInfo(UserAuthority.Admin, 66, "生产历史", defaultView, true, 6));

//        return Task.FromResult(true);
//    }
//}
