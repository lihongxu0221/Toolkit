using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;

namespace RoslynPad.Roslyn;

/// <summary>
/// Roslyn 宿主接口，提供文档管理、服务检索及程序集引用创建等核心功能.
/// </summary>
public interface IRoslynHost
{
    /// <summary>
    /// Gets 解析选项.
    /// </summary>
    ParseOptions ParseOptions { get; }

    /// <summary>
    /// 获取指定类型的通用服务.
    /// </summary>
    /// <typeparam name="TService">要获取的服务类型.</typeparam>
    /// <returns>返回请求的服务实例.</returns>
    TService GetService<TService>();

    /// <summary>
    /// 获取与特定文档关联的工作区级服务.
    /// </summary>
    /// <typeparam name="TService">服务类型，必须继承自 <see cref="IWorkspaceService"/>.</typeparam>
    /// <param name="documentId">文档的唯一标识符.</param>
    /// <returns>返回关联的工作区服务实例.</returns>
    TService GetWorkspaceService<TService>(DocumentId documentId)
        where TService : IWorkspaceService;

    /// <summary>
    /// 根据提供的参数向宿主添加新文档.
    /// </summary>
    /// <param name="args">创建文档所需的配置参数.</param>
    /// <returns>返回新创建文档的标识符.</returns>
    DocumentId AddDocument(DocumentCreationArgs args);

    /// <summary>
    /// 根据文档标识符获取文档对象实例.
    /// </summary>
    /// <param name="documentId">文档的唯一标识符.</param>
    /// <returns>返回文档对象；如果找不到对应的文档，则返回 null.</returns>
    Document? GetDocument(DocumentId documentId);

    /// <summary>
    /// 关闭指定标识符的文档并清理相关资源.
    /// </summary>
    /// <param name="documentId">要关闭的文档标识符.</param>
    void CloseDocument(DocumentId documentId);

    /// <summary>
    /// 根据指定的文件路径创建元数据引用.
    /// </summary>
    /// <param name="location">程序集在文件系统中的完整路径.</param>
    /// <returns>返回元数据引用实例.</returns>
    MetadataReference CreateMetadataReference(string location);
}