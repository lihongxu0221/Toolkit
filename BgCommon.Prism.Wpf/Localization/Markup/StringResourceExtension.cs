namespace BgCommon.Prism.Wpf.Localization.Markup;

/// <summary>
/// 嵌入资源字符串绑定扩展.
/// </summary>
public class StringResourceExtension : MarkupBindableBaseExtension
{
    private string key = string.Empty;
    private string stringFormat = string.Empty;

    /// <summary>
    /// Gets or sets 绑定的键.
    /// </summary>
    [ConstructorArgument("key")]
    public string Key
    {
        get => this.key;
        set => _ = this.SetProperty(ref this.key, value);
    }

    /// <summary>
    /// Gets or sets 字符串格式化.
    /// </summary>
    public string StringFormat
    {
        get => this.stringFormat;
        set => _ = this.SetProperty(ref this.stringFormat, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringResourceExtension"/> class.
    /// </summary>
    public StringResourceExtension()
        : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringResourceExtension"/> class.
    /// </summary>
    /// <param name="key">绑定的键.</param>
    public StringResourceExtension(string key)
    {
        this.Key = key;
    }

    /// <inheritdoc />
    protected override void SetValue()
    {
        if (this.ExecuteAssembly == null)
        {
            this.Value = this.key;
            return;
        }

        string value = Ioc.GetString(this.ExecuteAssembly, this.Key);

        if (!string.IsNullOrEmpty(this.StringFormat))
        {
            this.Value = string.Format(this.StringFormat, value);
        }
        else
        {
            this.Value = value;
        }
    }
}