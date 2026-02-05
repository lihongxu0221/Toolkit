namespace BgCommon.Script;

/// <summary>
/// 脚本执行时的全局寄宿对象基类.
/// </summary>
public class ScriptGlobals
{
    /// <summary>
    /// Gets or sets 日志接口，供脚本内部使用.
    /// </summary>
    public Action<string>? Log { get; set; }

    /// <summary>
    /// Gets or sets 宿主上下文数据.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets 允许脚本检查是否已被请求取消执行.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 提供一个通用的异步输出方法.
    /// </summary>
    /// <param name="message">日志信息.</param>
    /// <returns>返回Task.</returns>
    public async Task WriteLogAsync(string message)
    {
        await Task.Run(() => Log?.Invoke(message));
    }
}