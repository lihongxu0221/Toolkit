namespace BgCommon.Localization;

/// <summary>
/// YAML实现
/// </summary>
public static class YamlDictionariesDeserializer
{
    private const string DefaultNamespace = "default";

    public static IDictionary<string, IDictionary<string, string>> FromString(string input)
    {
        Dictionary<string, IDictionary<string, string>> result = new();
        string[] lines = input.Split(
            new char[] { '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries
        );
        string currentNamespace = DefaultNamespace;
        int currentIndentation = 0;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // 忽略注释
            if (trimmedLine.StartsWith("#"))
            {
                continue;
            }

            // 检查命名空间
            if (trimmedLine.EndsWith(":"))
            {
                currentNamespace = trimmedLine.TrimEnd(':');
                currentIndentation = line.IndexOf(trimmedLine);
                continue;
            }

            // 检查嵌套命名空间
            if (trimmedLine.StartsWith("- "))
            {
                currentNamespace += "." + trimmedLine.TrimStart('-').Trim();
                continue;
            }

            // 检查命名空间是否结束
            if (line.IndexOf(trimmedLine) < currentIndentation)
            {
                currentNamespace = DefaultNamespace;
                currentIndentation = 0;
            }

            // 拆分键和值
            string[] parts = trimmedLine.Split(new[] { ':' }, 2).Select(p => p.Trim()).ToArray();

            if (parts.Length != 2)
            {
                continue;
            }

            // 从值中删除开始和结束引号，但将引号保留在字符串中
            string value = RemoveStartEndQuotes(parts[1]);

            // 添加到词典
            AddToDictionary(result, currentNamespace, parts[0], value);
        }

        return result;
    }

    private static string RemoveStartEndQuotes(string value)
    {
        if (
            value.StartsWith("'") && value.EndsWith("'")
            || value.StartsWith("\"") && value.EndsWith("\"")
        )
        {
            value = value.Substring(1, value.Length - 2);
        }

        return value;
    }

    private static void AddToDictionary(
        IDictionary<string, IDictionary<string, string>> dict,
        string namespaceKey,
        string key,
        string value
    )
    {
        if (!dict.ContainsKey(namespaceKey))
        {
            dict[namespaceKey] = new Dictionary<string, string>();
        }

        dict[namespaceKey][key] = value;

        // 如果它不是嵌套名称空间，也添加到默认名称空间
        if (!namespaceKey.Contains("."))
        {
            if (!dict.ContainsKey(DefaultNamespace))
            {
                dict[DefaultNamespace] = new Dictionary<string, string>();
            }

            dict[DefaultNamespace][key] = value;
        }
    }
}