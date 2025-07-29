namespace ToolKit.DecryptTool;

public class ResourceExtractor
{
    private readonly ILogger logger;

    public ResourceExtractor(ILogger logger)
    {
        this.logger = logger;
    }

    public bool ExtractResources(string dllPath, string outputBasePath)
    {
        this.logger.Info($"开始从 DLL 中提取资源: {dllPath}");
        this.logger.Info($"文件将被还原到: {outputBasePath}");

        try
        {
            // 使用 Assembly.LoadFrom 在非锁定模式下加载程序集
            var assembly = Assembly.LoadFrom(dllPath);
            string[] resourceNames = assembly.GetManifestResourceNames();

            this.logger.Info($"在 DLL 中找到 {resourceNames.Length} 个嵌入式资源。");

            if (resourceNames.Length == 0)
            {
                this.logger.Warn("DLL 中未找到任何嵌入式资源。");
                return true; // 没有资源也算成功完成
            }

            foreach (var resourceName in resourceNames)
            {
                // 将 . 分隔的逻辑资源名转换回 \ 分隔的文件路径
                // 注意：我们的 DllBuilder 将 .resources 文件的逻辑名设置成了不带扩展名的形式
                // 而其他文件则保留了扩展名。我们需要处理这两种情况。

                string relativePath;
                Stream resourceStream = assembly.GetManifestResourceStream(resourceName);

                if (resourceStream == null)
                {
                    this.logger.Warn($"无法获取资源 '{resourceName}' 的流，跳过。");
                    continue;
                }

                // 尝试判断是否是 .resources 文件编译来的
                // .resources 文件的资源名称不包含 .resources 后缀
                try
                {
                    // 如果能被 ResourceManager 读取，说明它是一个 .resources 流
                    using (var reader = new System.Resources.ResourceReader(resourceStream))
                    {
                        relativePath = resourceName.Replace('.', Path.DirectorySeparatorChar) + ".resx";
                        this.logger.Debug($"检测到资源 '{resourceName}' 为 .resources 流，将还原为 .resx 文件。");

                        // 创建一个 .resx 文件并写入内容
                        var fullOutputPath = Path.Combine(outputBasePath, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

                        using (var writer = new System.Resources.ResXResourceWriter(fullOutputPath))
                        {
                            foreach (System.Collections.DictionaryEntry entry in reader)
                            {
                                writer.AddResource(entry.Key.ToString(), entry.Value);
                            }
                        }

                        this.logger.Info($"已还原 .resx 文件: {fullOutputPath}");
                    }
                }
                catch (ArgumentException) // 如果流不是有效的 .resources 格式，会抛出此异常
                {
                    // 这说明它是一个普通的文件流
                    relativePath = resourceName.Replace('.', Path.DirectorySeparatorChar);
                    this.logger.Debug($"检测到资源 '{resourceName}' 为普通文件流。");

                    var fullOutputPath = Path.Combine(outputBasePath, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

                    using (var fileStream = new FileStream(fullOutputPath, FileMode.Create, FileAccess.Write))
                    {
                        // 必须重置流的位置，因为上面的 ResourceReader 可能已经读取过它
                        resourceStream.Position = 0;
                        resourceStream.CopyTo(fileStream);
                    }

                    this.logger.Info($"已还原文件: {fullOutputPath}");
                }
                finally
                {
                    resourceStream?.Dispose();
                }
            }

            this.logger.Info("✔✔✔ 所有资源已成功提取！");
            return true;
        }
        catch (Exception ex)
        {
            this.logger.Error(ex, $"从 DLL 提取资源时发生严重错误。");
            return false;
        }
    }
}