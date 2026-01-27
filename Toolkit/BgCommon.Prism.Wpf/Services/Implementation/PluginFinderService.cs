namespace BgCommon.Prism.Wpf.Services;

/// <summary>
/// 提供了查找和导入插件功能的具体实现.
/// </summary>
internal class PluginFinderService : IPluginFinderService
{
    private readonly IFileDialogService fileDialogService;

    public PluginFinderService(IFileDialogService fileDialogService)
    {
        this.fileDialogService = fileDialogService;
    }

    /// <inheritdoc/>
    public List<PluginInfo> FindPluginsInFile(string filePath, params Type[] pluginTypes)
    {
        var foundTypes = new List<PluginInfo>();

        // 使用 Assembly.LoadFrom 加载程序集
        var assembly = Assembly.LoadFrom(filePath);

        // 遍历程序集中的所有可访问类型
        foreach (Type type in assembly.GetExportedTypes()) // GetExportedTypes() 只返回公共类型
        {
            // 核心过滤逻辑
            if (type.IsClass && !type.IsAbstract &&
                pluginTypes.All(pluginType => pluginType.IsAssignableFrom(type)))
            {
                foundTypes.Add(new(type));
            }
        }

        return foundTypes;
    }

    /// <inheritdoc/>
    public async Task<List<PluginInfo>> ImportPluginsAsync(OpenFileOptions options, params Type[] pluginTypes)
    {
        // 异步版本只是将同步的核心逻辑放到了后台线程，
        // 但文件对话框的显示本身是阻塞的，所以这里的异步主要是为了遵循“异步贯穿始终”的模式。
        return await Task.Run(() => ImportPlugins(options, pluginTypes));
    }

    /// <inheritdoc/>
    public List<PluginInfo> ImportPlugins(OpenFileOptions options, params Type[] pluginTypes)
    {
        // 1. 弹窗选择指定扩展名的文件.
        List<AssemblyItem> selectedFiles = fileDialogService.ShowOpenFileDialog(options);
        if (!selectedFiles.Any())
        {
            return new List<PluginInfo>(); // 用户取消
        }

        // 2. 用户选择了文件，开始处理
        return selectedFiles.SelectMany(file => FindPluginsInFile(file.FilePath, pluginTypes)).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<PluginInfo>> ImportPluginsFromSelectedDirectoryAsync(OpenFolderOptions options, string[] searchPatterns, params Type[] pluginTypes)
    {
        return await Task.Run(() => ImportPluginsFromSelectedDirectory(options, searchPatterns, pluginTypes));
    }

    /// <inheritdoc/>
    public List<PluginInfo> ImportPluginsFromSelectedDirectory(OpenFolderOptions options, string[] searchPatterns, params Type[] pluginTypes)
    {
        // 步骤 1: 【UI交互】调用 IFileDialogService 弹出文件夹选择框
        List<string> selectedFolders = fileDialogService.ShowOpenFolderDialog(options);
        if (!selectedFolders.Any())
        {
            return new List<PluginInfo>(); // 用户取消
        }

        // 步骤 2: 遍历所有选中的文件夹，并调用已有的、非交互式的目录扫描方法
        return selectedFolders.SelectMany(folderPath => ImportPluginsFromDirectory(
                folderPath,
                options.SearchOption, // 使用 options 中定义的搜索深度
                searchPatterns,
                pluginTypes))
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<List<PluginInfo>> ImportPluginsFromDirectoryAsync(string directoryPath, SearchOption searchOption, string[] searchPatterns, params Type[] pluginTypes)
    {
        return await Task.Run(() => ImportPluginsFromDirectory(directoryPath, searchOption, searchPatterns, pluginTypes));
    }

    /// <inheritdoc/>
    public List<PluginInfo> ImportPluginsFromDirectory(string directoryPath, SearchOption searchOption, string[] searchPatterns, params Type[] pluginTypes)
    {
        if (!Directory.Exists(directoryPath))
        {
            // 如果指定的文件夹不存在，直接返回空列表
            LogRun.Warn($"尝试从不存在的目录加载插件: {directoryPath}");
            return new List<PluginInfo>();
        }

        // 步骤 1: 直接、静默地在指定路径下查找文件
        var patternsToSearch = searchPatterns;
        if (patternsToSearch == null || patternsToSearch.Length == 0)
        {
            patternsToSearch = new[] { "*.*" };
        }

        var assemblyFiles = patternsToSearch.SelectMany(
            pattern => Directory.EnumerateFiles(directoryPath, pattern, searchOption));

        // 步骤 2: 从找到的文件中查找插件
        return assemblyFiles.SelectMany(filePath => FindPluginsInFile(filePath, pluginTypes))
                            .ToList();
    }
}