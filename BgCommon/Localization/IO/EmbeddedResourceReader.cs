namespace BgCommon.Localization.IO;

public static class EmbeddedResourceReader
{
    private static readonly Regex MultipleDotRegex = new(@"\.{2,}", RegexOptions.Compiled);

    /// <summary>
    /// 从指定程序集中读取资源的内容.
    /// </summary>
    /// <param name="path">程序集中资源的路径.</param>
    /// <param name="assembly">包含资源的程序集.</param>
    /// <returns>资源的内容为字符串，如果找不到资源，则为null.</returns>
    public static string? ReadToEnd(string path, Assembly assembly)
    {
        string assemblyName = assembly.GetName().Name;
        string resourceName = $"{assemblyName}.{
            path.Replace(assemblyName, string.Empty, StringComparison.InvariantCultureIgnoreCase)}";


        resourceName = MultipleDotRegex.Replace(
            resourceName.Replace("\\", ".").Replace("/", "."),
            "."
        );

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            return null;
        }

        using StreamReader reader = new(stream);

        return reader.ReadToEnd();
    }
}