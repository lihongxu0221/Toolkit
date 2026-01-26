namespace BgCommon.Script;

/// <summary>
/// 各种字符串扩展.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 确定字符串是否为空（或为 null）.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <returns>
    /// 如果指定的文本为空，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool IsEmpty(this string text) => string.IsNullOrEmpty(text);

    /// <summary>
    /// 确定字符串是否不为空（或不为 null）.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <returns>
    ///   如果 [不为空] [指定的文本]，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool IsNotEmpty(this string text) => !string.IsNullOrEmpty(text);

    /// <summary>
    /// 确定此实例是否包含文本.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <returns>
    ///   如果指定的文本有文本，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool HasText(this string? text) => !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);

    /// <summary>
    /// 从字符串的开头和结尾修剪单个字符.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="trimChars">要修剪的字符.</param>
    /// <returns>修剪的结果.</returns>
    public static string TrimSingle(this string text, params char[] trimChars)
    {
        if (text.IsEmpty())
        {
            return text;
        }

        var startOffset = trimChars.Contains(text[0]) ? 1 : 0;
        var endOffset = trimChars.Contains(text.Last()) ? 1 : 0;

        if (startOffset != 0 || endOffset != 0)
        {
            return text.Substring(startOffset, (text.Length - startOffset) - endOffset);
        }
        else
        {
            return text;
        }
    }

    /// <summary>
    /// 获取行.
    /// </summary>
    /// <param name="str">字符串.</param>
    /// <returns>方法结果.</returns>
    public static string[] GetLines(this string str) => str.Replace("\r\n", "\n").Split('\n');

    /// <summary>
    /// 确定此字符串是否包含由模式定义的子字符串.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="pattern">模式.</param>
    /// <param name="ignoreCase">如果设置为 <c>true</c> 则 [忽略大小写].</param>
    /// <returns>
    ///   如果 [包含] [指定的模式]，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool Contains(this string text, string pattern, bool ignoreCase)
        => text.Contains(pattern, ignoreCase ? StringComparison.OrdinalIgnoreCase : default(StringComparison));

    /// <summary>
    /// 比较两个字符串.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="pattern">模式.</param>
    /// <param name="ignoreCase">如果设置为 <c>true</c> 则 [忽略大小写].</param>
    /// <returns>测试结果.</returns>
    public static bool SameAs(this string text, string pattern, bool ignoreCase = true)
        => string.Compare(text, pattern, ignoreCase) == 0;

    /// <summary>
    /// 检查给定的字符串是否与提供的任何模式匹配.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="patterns">模式.</param>
    /// <returns>方法结果.</returns>
    public static bool IsOneOf(this string text, params string[] patterns)
        => patterns.Any(x => x == text);

    /// <summary>
    /// 按指定的分隔符连接字符串.
    /// </summary>
    /// <param name="values">值.</param>
    /// <param name="separator">分隔符.</param>
    /// <returns>方法结果.</returns>
    public static string JoinBy(this IEnumerable<string> values, string separator)
        => string.Join(separator, values);
}