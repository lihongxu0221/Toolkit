namespace BgCommon.Localization.Markup;

/// <summary>
/// 嵌入资源字符串绑定扩展。
/// </summary>
// [MarkupExtensionReturnType(typeof(BindingExpression))]
public class StringLocalizerExtension : MarkupBindableBaseExtension
{
    private string key = string.Empty;
    private string stringFormat = string.Empty;

    /// <summary>
    /// Gets or sets 绑定的键。
    /// </summary>
    [ConstructorArgument("key")]
    public string Key
    {
        get { return key; }
        set { _ = SetProperty(ref key, value); }
    }

    /// <summary>
    /// Gets or sets 字符串格式化。
    /// </summary>
    public string StringFormat
    {
        get => stringFormat;
        set => _ = SetProperty(ref stringFormat, value);
    }

    public StringLocalizerExtension()
        : this(string.Empty)
    {
    }

    public StringLocalizerExtension(string key)
    {
        this.Key = key;
    }

    /// <inheritdoc />
    protected override void SetValue()
    {
        // if (Provider == null)
        // {
        //     Value = key;
        //     return;
        // }
        // string value = Provider.GetString(Key, ExecuteAssembly.GetName()?.Name);
        // if (!string.IsNullOrEmpty(StringFormat))
        // {
        //     Value = string.Format(StringFormat, value);
        // }
        // else
        // {
        //     Value = value;
        // }

        if (Ioc.Instance == null)
        {
            Value = key;
            return;
        }

        string value = Ioc.Instance.GetString(ExecuteAssembly.GetName()?.Name, Key);

        if (!string.IsNullOrEmpty(StringFormat))
        {
            Value = string.Format(StringFormat, value);
        }
        else
        {
            Value = value;
        }
    }
}

///// <summary>
///// 提供在XAML中本地化字符串的标记扩展。
///// </summary>
///// <remarks>
///// 此类扩展<see cref=“MarkupExtension”/>并重写<see cred=“ProvideValue”/>方法以返回本地化字符串。
///// </remarks>
//[ContentProperty(nameof(Text))]
//[MarkupExtensionReturnType(typeof(string))]
//public class StringLocalizerExtension : MarkupExtension
//{
//    /// <summary>
//    /// Gets or sets 获取或设置要本地化的文本。
//    /// </summary>
//    public string? Text { get; set; }

//    /// <summary>
//    /// Gets or sets 资源文件命名空间
//    /// </summary>
//    public string? Namespace { get; set; }

//    /// <summary>
//    /// Gets 提供者密钥。
//    /// </summary>
//    public string ProviderKey { get; private set; } = string.Empty;

//    /// <summary>
//    /// Initializes a new instance of the <see cref="StringLocalizerExtension"/> class.
//    /// 初始化<see cref=“StringLocalizerExtension”/>类的新实例。
//    /// </summary>
//    public StringLocalizerExtension() { }

//    /// <summary>
//    /// Initializes a new instance of the <see cref="StringLocalizerExtension"/> class.
//    /// 使用指定的文本初始化<see cref=“StringLocalizerExtension”/>类的新实例。
//    /// </summary>
//    /// <param name="text">The text to be localized.</param>
//    public StringLocalizerExtension(string? text)
//        : this(text, string.Empty)
//    {

//    }

//    /// <summary>
//    /// Initializes a new instance of the <see cref="StringLocalizerExtension"/> class with the specified text and namespace.
//    /// 使用指定的文本初始化<see cref=“StringLocalizerExtension”/>类的新实例。
//    /// </summary>
//    /// <param name="text">The text to be localized.</param>
//    /// <param name="textNamespace">The namespace of the text to be localized.</param>
//    public StringLocalizerExtension(string? text, string? textNamespace)
//    {
//        Text = EscapeText(text);
//        Namespace = textNamespace;
//    }

//    /// <summary>
//    /// 返回<see-cref=“Text”/>属性的本地化字符串。
//    /// </summary>
//    /// <param name="serviceProvider">为标记扩展提供服务的对象。</param>
//    /// <returns>本地化字符串，如果找不到本地化，则为原始文本。</returns>
//    public override object? ProvideValue(IServiceProvider serviceProvider)
//    {
//        if (Text is null)
//        {
//            return string.Empty;
//        }

//        CultureInfo currentCulture =
//            LocalizationProviderFactory.GetInstance(ProviderKey)?.GetCulture()
//            ?? CultureInfo.CurrentUICulture;

//        string? selectedNamespace = Namespace?.ToLowerInvariant() ?? default;

//        LocalizationSet? localizationSet = LocalizationProviderFactory
//            .GetInstance(ProviderKey)
//            ?.GetLocalizationSet(currentCulture, selectedNamespace);

//        if (localizationSet is null)
//        {
//            return Text;
//        }

//        return localizationSet.Strings.FirstOrDefault(s => s.Key == Text).Value ?? Text;
//    }

//    /// <summary>
//    /// 转义字符串中的特殊字符。
//    /// </summary>
//    /// <param name="text">需要转义的字符串.</param>
//    /// <returns>转义后的字符串.</returns>
//    private static string EscapeText(string? text)
//    {
//        if (text is null)
//        {
//            return string.Empty;
//        }

//        return text.Replace("&amp;", "&")
//            .Replace("&lt;", "<")
//            .Replace("&gt;", ">")
//            .Replace("&quot;", "\"")
//            .Replace("&apos;", "'")
//            .Trim();
//    }
//}