namespace RoslynPad.Editor;

/// <summary>
/// 上下文操作提供程序接口，用于在编辑器中提供重构或快速修复操作.
/// </summary>
public interface IContextActionProvider
{
    /// <summary>
    /// 获取指定文档范围内的可用操作集合.
    /// </summary>
    /// <param name="offset">文档中的起始偏移量.</param>
    /// <param name="length">选中文本的长度.</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌.</param>
    /// <returns>返回包含可用操作对象的异步任务集合.</returns>
    Task<IEnumerable<object>> GetActionsAsync(int offset, int length, CancellationToken cancellationToken);

    /// <summary>
    /// 获取与指定操作对象关联的执行命令.
    /// </summary>
    /// <param name="action">从 GetActions 方法返回的操作对象.</param>
    /// <returns>返回对应的命令实例，若无对应命令则返回 null.</returns>
    ICommand? GetActionCommand(object action);
}