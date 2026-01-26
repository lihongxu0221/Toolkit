namespace BgCommon.Prism.Wpf.Services;

/// <summary>
/// 提供一个用于从外部程序集文件中导入特定插件类型的功能.
/// </summary>
public interface IPluginFinderService
{
    /// <summary>
    /// 从指定的程序集文件中，反射出所有实现了指定插件类型的具体、可实例化的类.
    /// </summary>
    /// <param name="filePath">程序集的路径</param>
    /// <param name="pluginTypes">一个或多个插件接口或基类类型. 例如: typeof(IPlugin).</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果用户取消了选择，则返回一个空列表.
    /// </returns>
    List<PluginInfo> FindPluginsInFile(string filePath, params Type[] pluginTypes);

    /// <summary>
    /// 【异步】弹出一个文件选择对话框，允许用户选择一个或多个程序集文件(*.dll, *.exe)，
    /// 然后从中反射出所有实现了指定插件类型的具体、可实例化的类.
    /// </summary>
    /// <param name="options">文件对话框的配置选项.</param>
    /// <param name="pluginTypes">一个或多个插件接口或基类类型. 例如: typeof(IPlugin), typeof(BaseViewModel).</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果用户取消了选择，则返回一个空列表.
    /// </returns>
    Task<List<PluginInfo>> ImportPluginsAsync(OpenFileOptions options, params Type[] pluginTypes);

    /// <summary>
    /// 【同步】弹出一个文件选择对话框，允许用户选择一个或多个程序集文件(*.dll, *.exe)，
    /// 然后从中反射出所有实现了指定插件类型的具体、可实例化的类.
    /// 【警告】此方法会阻塞UI线程，直到用户关闭文件对话框。建议优先使用异步版本.
    /// </summary>
    /// <param name="options">文件对话框的完整配置选项.</param>
    /// <param name="pluginTypes">一个或多个插件接口或基类类型. 例如: typeof(IPlugin), typeof(BaseViewModel).</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果用户取消了选择，则返回一个空列表.
    /// </returns>
    List<PluginInfo> ImportPlugins(OpenFileOptions options, params Type[] pluginTypes);

    /// <summary>
    /// 【异步】弹出一个文件夹选择对话框，让用户选择一个或多个文件夹，并扫描其中的插件.
    /// </summary>
    /// <param name="options">文件夹对话框的配置选项.</param>
    /// <param name="searchPatterns">要在选中文件夹中搜索的文件模式.</param>
    /// <param name="pluginTypes">要查找的插件类型.</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果用户取消了选择，则返回一个空列表.
    /// </returns>
    Task<List<PluginInfo>> ImportPluginsFromSelectedDirectoryAsync(OpenFolderOptions options, string[] searchPatterns, params Type[] pluginTypes);

    /// <summary>
    /// 【同步】弹出一个文件夹选择对话框，让用户选择一个或多个文件夹，并扫描其中的插件.
    /// </summary>
    /// <param name="options">文件夹对话框的配置选项.</param>
    /// <param name="searchPatterns">要在选中文件夹中搜索的文件模式.</param>
    /// <param name="pluginTypes">要查找的插件类型.</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果用户取消了选择，则返回一个空列表.
    /// </returns>
    List<PluginInfo> ImportPluginsFromSelectedDirectory(OpenFolderOptions options, string[] searchPatterns, params Type[] pluginTypes);

    /// <summary>
    /// 【异步】从指定的文件夹（及其所有子文件夹）中，扫描所有程序集文件(*.dll, *.exe)，
    /// 然后从中反射出所有实现了指定插件类型的具体、可实例化的类.
    /// </summary>
    /// <param name="directoryPath">要扫描的根文件夹的完整路径.</param>
    /// <param name="searchOption">搜索选项 (顶层目录或所有子目录).</param>
    /// <param name="searchPatterns">要搜索的文件模式 (例如 "*.dll", "*.exe"). 如果不提供，则默认为 "*.*".</param>
    /// <param name="pluginTypes">一个或多个插件接口或基类类型. 例如: typeof(IPlugin).</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果文件夹不存在或未找到任何符合条件的类型，则返回一个空列表.
    /// </returns>
    Task<List<PluginInfo>> ImportPluginsFromDirectoryAsync(string directoryPath, SearchOption searchOption, string[] searchPatterns, params Type[] pluginTypes);

    /// <summary>
    /// 【同步】从指定的文件夹（及其所有子文件夹）中，扫描所有程序集文件(*.dll, *.exe)，
    /// 然后从中反射出所有实现了指定插件类型的具体、可实例化的类. (同步版本)
    /// </summary>
    /// <param name="directoryPath">要扫描的根文件夹的完整路径.</param>
    /// <param name="searchOption">搜索选项 (顶层目录或所有子目录).</param>
    /// <param name="searchPatterns">要搜索的文件模式 (例如 "*.dll", "*.exe"). 如果不提供，则默认为 "*.*".</param>
    /// <param name="pluginTypes">一个或多个插件接口或基类类型. 例如: typeof(IPlugin).</param>
    /// <returns>
    /// 一个包含所有找到的、符合条件的类型的列表.
    /// 如果文件夹不存在或未找到任何符合条件的类型，则返回一个空列表.
    /// </returns>
    List<PluginInfo> ImportPluginsFromDirectory(string directoryPath, SearchOption searchOption, string[] searchPatterns, params Type[] pluginTypes);
}