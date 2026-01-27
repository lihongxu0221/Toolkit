namespace RoslynPad.UI;

/// <summary>
/// 文件对话框过滤器.
/// </summary>
public sealed class FileDialogFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileDialogFilter"/> class.
    /// </summary>
    /// <param name="header">过滤器标题.</param>
    /// <param name="extensions">扩展名集合.</param>
    public FileDialogFilter(string header, IList<string> extensions)
    {
        ArgumentNullException.ThrowIfNull(header, nameof(header));
        ArgumentNullException.ThrowIfNull(extensions, nameof(extensions));
        this.Header = header;
        this.Extensions = extensions.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDialogFilter"/> class.
    /// </summary>
    /// <param name="header">过滤器标题.</param>
    /// <param name="extensions">扩展名数组.</param>
    public FileDialogFilter(string header, params string[] extensions)
        : this(header, (IList<string>)extensions)
    {
    }

    /// <summary>
    /// Gets 过滤器标题.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Gets 扩展名列表.
    /// </summary>
    public IReadOnlyList<string> Extensions { get; }

    /// <summary>
    /// 将过滤器转换为对话框使用的字符串格式.
    /// </summary>
    /// <returns>格式化后的过滤器字符串.</returns>
    public override string ToString()
    {
        // 将扩展名列表中的每一项前缀加上星号和点，然后用分号连接，最后与标题组合.
        return $"{this.Header}|{string.Join(";", this.Extensions.Select(e => e.Contains(".") ? e : "*." + e))}";
    }
}