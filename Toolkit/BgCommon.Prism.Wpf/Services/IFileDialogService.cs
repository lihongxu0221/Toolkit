namespace BgCommon.Prism.Wpf.Services;

/// <summary>
/// 文件夹浏览对话框服务接口.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// 显示一个文件选择对话框.
    /// </summary>
    /// <param name="options">文件选择的配置选项.</param>
    /// <returns>一个包含所有被选中文件信息的 AssemblyItem 列表. 如果用户取消，则返回空列表.</returns>
    List<AssemblyItem> ShowOpenFileDialog(OpenFileOptions options); 

    /// <summary>
    /// 显示一个文件夹选择对话框，并静默查找指定后缀的文件.
    /// </summary>
    /// <param name="options">文件夹选择的配置选项.</param>
    /// <returns>一个包含被选中文件夹的列表.</returns>
    List<string> ShowOpenFolderDialog(OpenFolderOptions options);

    /// <summary>
    /// 显示一个文件夹选择对话框，并静默查找指定后缀的文件.
    /// </summary>
    /// <param name="options">文件夹选择的配置选项.</param>
    /// <param name="searchPatterns">要在选中文件夹中搜索的文件模式 (例如 "*.dll", "*.exe").</param>
    /// <returns>一个包含所有在选中文件夹（及其子文件夹）中找到的文件信息的 AssemblyItem 列表.</returns>
    List<AssemblyItem> GetFilesFromFoldersDialog(OpenFolderOptions options, params string[] searchPatterns);

    /// <summary>
    /// 异步地显示文件夹选择对话框，并在后台扫描文件.
    /// </summary>
    /// <param name="options">文件夹选择的配置选项.</param>
    /// <param name="progress">用于接收进度报告的处理器.</param>
    /// <param name="cancellationToken">用于取消操作的令牌.</param>
    /// <param name="searchPatterns">要搜索的文件模式.</param>
    /// <returns>一个包含所有找到的文件信息的列表. 如果操作被取消，则可能返回部分结果或抛出异常.</returns>
    Task<List<AssemblyItem>> GetFilesFromFoldersDialogAsync(
        OpenFolderOptions options,
        IProgress<FileSearchProgressReport>? progress = null,
        CancellationToken cancellationToken = default,
        params string[] searchPatterns);
}