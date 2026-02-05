namespace BgCommon.Script;

/// <summary>
/// 表示脚本执行结果的类，包含成功状态、返回消息、结果对象及可能的异常信息.
/// </summary>
public class ScriptResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptResult"/> class.
    /// </summary>
    internal ScriptResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptResult"/> class.
    /// </summary>
    /// <param name="success">指示脚本执行是否成功的布尔值.</param>
    /// <param name="message">执行结果的描述信息.</param>
    /// <param name="result">脚本执行返回的可选结果对象.</param>
    /// <param name="exception">脚本执行过程中产生的可选异常信息.</param>
    public ScriptResult(bool success, string message, object? result = null, Exception? exception = null)
    {
        // 验证消息参数是否为空.
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        // 为只读属性赋值，显式使用 this 标识.
        this.Success = success;
        this.Message = message;
        this.Result = result;
        this.Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether 脚本执行是否成功.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets 脚本执行过程中产生的异常.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets 执行结果的描述消息.
    /// </summary>
    public string Message { get; } = string.Empty;

    /// <summary>
    /// Gets 脚本执行返回的结果对象.
    /// </summary>
    public object? Result { get; }

    /// <summary>
    /// 方便在 UI 上显示格式化后的错误.
    /// </summary>
    /// <returns>返回 格式化异常信息.</returns>
    public string GetFormattedError()
    {
        if (this.Exception is ScriptCompilationException compileEx)
        {
            return compileEx.Message; // 已经包含了格式化的行号和错误码
        }

        return this.Exception?.Message ?? this.Message;
    }
}