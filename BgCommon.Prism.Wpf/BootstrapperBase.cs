using BgCommon;
using BgCommon.Localization.DependencyInjection;
using BgCommon.Prism.Wpf.Authority.Services;
using BgCommon.Prism.Wpf.Authority.Services.Implementation;
using BgCommon.Prism.Wpf.Common.ViewModels;
using BgCommon.Prism.Wpf.Common.Views;
using BgCommon.Prism.Wpf.Controls;
using BgCommon.Prism.Wpf.Controls.ViewModels;
using BgCommon.Prism.Wpf.DependencyInjection;
using BgCommon.Prism.Wpf.DependencyInjection.Implementation;
using BgCommon.Prism.Wpf.Services;
using BgCommon.Prism.Wpf.Services.Implementation;
using BgCommon.Prism.Wpf.Views;
using System.IO;

namespace BgCommon.Prism.Wpf;

public abstract class BootstrapperBase : PrismBootstrapper
{
    private readonly Application app;

    public BootstrapperBase(Application app)
        : base()
    {
        this.app = app;
        this.app.Exit += App_Exit;
        this.app.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    /// <summary>
    /// Gets 模块和插件所在文件夹.
    /// </summary>
    protected abstract string[]? ModuleDirectories { get; }

    /// <summary>
    /// 获取初始化服务的实例类型,从InitializationServiceBase实现.
    /// </summary>
    /// <returns>返回 初始化服务的实例类型.</returns>
    protected abstract Type GetInitialServiceType();

    /// <summary>
    /// 获取全局参数服务.
    /// </summary>
    /// <returns>返回 初始化全局参数服务</returns>
    protected abstract Type GetGlobalVarService();

    /// <summary>
    /// 获取用户管理服务.
    /// </summary>
    /// <returns>返回 用户管理服务.</returns>
    protected virtual Type GetUserServiceType()
    {
        return typeof(DefaultUserService);
    }

    /// <summary>
    /// 获取功能模块权限相关服务.
    /// </summary>
    /// <returns>返回 功能模块权限相关服务</returns>
    protected abstract Type GetModuleService();

    /// <summary>
    /// 通过ViewType获取 ViewModelType.
    /// </summary>
    /// <param name="viewType">View类型</param>
    /// <returns>ViewModelType</returns>
    protected virtual Type? GetViewModelType(Type viewType)
    {
        if (viewType != null && viewType.Namespace != null)
        {
            var vmTypeName = $"{viewType.Name}ViewModel";
            if (vmTypeName.EndsWith("ViewViewModel"))
            {
                vmTypeName = vmTypeName.Replace("ViewViewModel", "ViewModel");
            }

            var @namespace = viewType.Namespace;
            if (@namespace.EndsWith(".Views"))
            {
                @namespace = @namespace.Replace(".Views", ".ViewModels");
            }
            else if (@namespace.Contains(".Views."))
            {
                @namespace = @namespace.Replace(".Views.", ".ViewModels.");
            }
            else
            {
                @namespace = $"{@namespace}.ViewModels";
            }

            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = $"{@namespace}.{vmTypeName}, {viewAssemblyName}";
            return Type.GetType(viewModelName);
        }

        return null;
    }

    /// <summary>
    /// 注入BgCommon.Prism.Wpf 中所有的应对注入的.
    /// </summary>
    protected void RegisterBgRequiredTypes(IContainerRegistry containerRegistry)
    {
        if (containerRegistry is not IContainerExtension registry)
        {
            throw new ArgumentException("containerRegistry is not IContainerExtension");
        }

        // 0. 初始化Ioc 容器
        Ioc.Initialize(registry);

        // 1.注入多语言
        _ = containerRegistry.AddStringLocalizer(b =>
        {
            // BgCommon
            b.FromResource<Assets.Localization.TranslationsEnum>(new CultureInfo("zh-CN"), true);
            b.FromResource<Assets.Localization.TranslationsEnum>(new CultureInfo("zh-TW"), true);
            b.FromResource<Assets.Localization.TranslationsEnum>(new CultureInfo("en-US"), true);
            b.FromResource<Assets.Localization.TranslationsEnum>(new CultureInfo("vi"), true);

            b.FromResource<Assets.Localization.Translations>(new CultureInfo("zh-CN"), true);
            b.FromResource<Assets.Localization.Translations>(new CultureInfo("zh-TW"), true);
            b.FromResource<Assets.Localization.Translations>(new CultureInfo("en-US"), true);
            b.FromResource<Assets.Localization.Translations>(new CultureInfo("vi"), true);
        });

        // 2. 注入日志模块
        BgLoggerFactory.Register(containerRegistry);

        // 3. 注入服务
        Type initialService = GetInitialServiceType();
        if (initialService == null || !initialService.IsAssignableTo(typeof(InitializationServiceBase)))
        {
            throw new ArgumentException(Ioc.GetString("InitialServiceType 不能为空，且必须从InitializationServiceBase派生!"));
        }

        Type moduleService = GetModuleService();
        if (moduleService == null || !moduleService.IsAssignableTo(typeof(ModuleServiceBase)))
        {
            throw new ArgumentException(Ioc.GetString("ModuleService 不能为空，且必须从ModuleServiceBase派生!"));
        }

        Type globalVarService = GetGlobalVarService();
        if (globalVarService == null)
        {
            throw new ArgumentException(Ioc.GetString("GlobalVarService不能为空!"));
        }

        Type userService = GetUserServiceType();
        if (userService == null)
        {
            throw new ArgumentException(Ioc.GetString("UserServiceType不能为空!"));
        }

        // 进程初始化服务
        _ = containerRegistry.RegisterSingleton(typeof(IInitializationService), initialService);

        // 全局变量管理服务
        _ = containerRegistry.RegisterSingleton(typeof(IGlobalVarService), globalVarService);

        // 用户管理服务
        _ = containerRegistry.RegisterSingleton(typeof(IUserService), userService);

        // 功能模块权限相关服务
        _ = containerRegistry.RegisterSingleton(typeof(IModuleService), moduleService);

        // Prism 视图自动注入服务.
        _ = containerRegistry.RegisterSingleton(typeof(IRegistrationService), typeof(DynamicRegistrationService));

        // 4. 注入必要的弹窗和视图.
        containerRegistry.RegisterDialogWindow<MessageDialogWindow>();
        containerRegistry.RegisterDialogWindow<MessageDialogWindowEx>(Constraints.SizeableDialog);
        containerRegistry.RegisterDialogWindow<LoginHostWindow>(Constraints.LoginHostWindow);

        containerRegistry.RegisterDialog<SplashScreenView>();
        _ = containerRegistry.RegisterSingleton<SplashScreenViewModel>();

        containerRegistry.RegisterDialog<MessageView, MessageViewModel>();
        containerRegistry.RegisterDialog<InputMessageView, InputMessageViewModel>();

        containerRegistry.RegisterForNavigation<DefaultView>();
        containerRegistry.RegisterForNavigation<ErrorView, ErrorViewModel>();
        containerRegistry.RegisterForNavigation<LoadingView>();
        containerRegistry.RegisterForNavigation<ModuleHostView, ModuleHostViewModel>();
    }

    /// <inheritdoc/>
    protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
    {
        base.RegisterRequiredTypes(containerRegistry);
        RegisterBgRequiredTypes(containerRegistry);
    }

    /// <inheritdoc/>
    protected override async void OnInitialized()
    {
        bool isFaulted = false;
        IInitializationService? initService = null;
        try
        {
            // 获取初始化服务
            initService = Container.Resolve<IInitializationService>();

            // 2.加载其他相关的必要数据
            if (!await initService.RunAsync())
            {
                isFaulted = true;
                return;
            }

            // 在初始化完成后关闭启动画面并显示主窗口
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 显示主窗口
                if (Shell is Window mainWindow)
                {
                    mainWindow.Show();
                }
            });
        }
        catch (Exception ex)
        {
            isFaulted = true;

            // 记录日志
            LogDialog.Error(ex);
        }
        finally
        {
            if (isFaulted)
            {
                // 释放已加载的系统资源
                if (initService != null)
                {
                    initService.Dispose();
                }

                // 初始化失败，关闭应用程序
                Application.Current.Shutdown();
            }
        }
    }

    /// <inheritdoc/>
    protected override IModuleCatalog CreateModuleCatalog()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        List<IModuleCatalogItem> components = new List<IModuleCatalogItem>();

        if (ModuleDirectories != null)
        {
            foreach (var dir in ModuleDirectories)
            {
                string folder = $"{path}\\{dir}";
                if (Directory.Exists(folder))
                {
                    var dirCatelog = new DirectoryModuleCatalog() { ModulePath = folder };
                    try
                    {
                        // 初始化目录模块目录 其中一个加载异常也会全失败
                        dirCatelog.Initialize();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.ToString() + "\r\n";
                        //if (ex.InnerException != null)
                        //{
                        //    msg += ex.InnerException.ToString();
                        //}

                        MessageBox.Show($"无法加载插件模块." + $"Folder:{folder}" + "\r\n" + msg);
                        Environment.Exit(0);
                    }


                    components.AddRange(dirCatelog.Items);
                }
                else
                {
                    Console.WriteLine($"{folder} not found");
                }
            }
        }

        var catelog = new ModuleCatalog();

        foreach (var com in components)
        {
            catelog.Items.Add(com);
        }
        return catelog;
    }

    /// <inheritdoc/>
    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();

        // 默认获取View 的VM的解决方案
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(this.GetViewModelType);
    }

    /// <summary>
    /// 进程推出.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="e">事件参数.</param>
    private void App_Exit(object sender, ExitEventArgs e)
    {
        IInitializationService? initService = Container?.Resolve<IInitializationService>();
        if (initService != null)
        {
            initService.Dispose();
        }
    }

    /// <summary>
    /// 处理程序集解析失败事件，尝试手动加载缺失的程序集.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="args">程序集解析事件参数.</param>
    /// <returns>已加载的程序集，若未能加载则返回 null.</returns>
    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        // AssemblyName assemblyName = new AssemblyName(args.Name);
        // try
        // {
        //     ////Assembly assembly = Encryption.LoadAssembly(AppDomain.CurrentDomain.BaseDirectory + assemblyName.Name);
        //     //bool flag = assembly != null;
        //     //if (flag)
        //     //{
        //     //    return assembly;
        //     //}
        //     string text = Environment.CurrentDirectory + "\\" + assemblyName.Name + ".dll";
        //     Console.WriteLine(text);
        //     bool flag2 = File.Exists(text);
        //     if (flag2)
        //     {
        //         return Assembly.LoadFrom(text);
        //     }
        // }
        // catch (Exception ex)
        // {
        //     System.Windows.MessageBox.Show(assemblyName.Name + ex.ToString());
        // }
        return null;
    }

    /// <summary>
    /// 处理应用程序域中的未处理异常.
    /// </summary>
    /// <remarks>
    /// 当应用程序域中引发未处理异常时调用此方法.
    /// 记录异常详细信息并执行必要操作, 如生成崩溃转储并通知相应的错误处理机制.
    /// 如果异常为致命异常, 会进行额外的日志记录和错误处理.
    /// </remarks>
    /// <param name="sender">未处理异常事件的源, 通常为发生异常的 <see cref="AppDomain"/>.</param>
    /// <param name="e">包含未处理异常信息的 <see cref="UnhandledExceptionEventArgs"/>, 包括异常对象和应用程序是否终止.</param>
    private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        StringBuilder stringBuilder = new StringBuilder();
        bool isTerminating = e.IsTerminating;
        if (isTerminating)
        {
            _ = stringBuilder.Append("A fatal error has occurred in the programme and it will terminate, please contact the developer!\n");
        }

        _ = stringBuilder.Append("Catching unhandled exceptions:");
        bool flag = e.ExceptionObject is Exception;
        if (flag)
        {
            _ = stringBuilder.Append(((Exception)e.ExceptionObject).ToString());
        }
        else
        {
            _ = stringBuilder.Append(e.ExceptionObject);
        }

        LogRun.Error(stringBuilder.ToString());

        // Dump
        // BG.Utility.Helper.MiniDumpHelper.TryDump(AppDomain.CurrentDomain.BaseDirectory + $"Dumps\\软件奔溃_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.dmp");
        bool isTerminating2 = e.IsTerminating;
        if (isTerminating2)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogRun.Fatal(ex);
            }

            _ = await Ioc.ErrorAsync(stringBuilder.ToString());
        }
    }

    /// <summary>
    /// 处理 WPF Dispatcher 未处理异常事件.
    /// 捕获 UI 线程未处理异常, 记录日志并根据异常类型决定是否终止或忽略.
    /// 对特定 COMException 错误码进行特殊处理, 其余异常记录详细信息并通知.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="e">包含异常信息的 DispatcherUnhandledExceptionEventArgs.</param>
    private async void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            if (e.Exception != null)
            {
                // 特定 COMException 错误码处理.
                if (e.Exception is COMException ex && ex != null && ex.ErrorCode == -2147221040)
                {
                    e.Handled = true;
                    return;
                }
                else if (e.Exception?.InnerException is COMException ex_ && ex_ != null && ex_.ErrorCode == -2147221040)
                {
                    e.Handled = true;
                    return;
                }

                e.Handled = true;

                // 针对包含"The attached property"的异常进行特殊日志记录.
                bool flag = e.Exception?.ToString().Contains("The attached property") ?? false;
                if (flag)
                {
                    string text = $"Catch unhandled exceptions:\r\n{e.Exception?.ToString()}\r\n{e.Exception?.StackTrace}";
                    LogRun.Error(text);
                    _ = await Ioc.WarnAsync(text);
                }
            }
        }
        catch (Exception ex)
        {
            // 捕获处理异常时自身抛出的异常, 记录并通知.
            string text = "A fatal error has occurred in the programme and it will terminate, please contact the developer!" + "\r\n" + ex.ToString() + "\r\n" + e.Exception.StackTrace;
            LogRun.Error(text);
            _ = await Ioc.WarnAsync(text);
        }
    }

    /// <summary>
    /// 处理Task调度时产生的未处理的异常.
    /// </summary>
    /// <param name="sender">事件源.</param>
    /// <param name="e">包含异常信息的 UnobservedTaskExceptionEventArgs.</param>
    private async void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        string text = "Catching unhandled exceptions in the thread:" + "\r\n" + e.Exception.ToString() + "\r\n" + e.Exception.StackTrace;
        LogRun.Error(text);
        e.SetObserved();
        _ = await Ioc.ErrorAsync(text);
    }
}
