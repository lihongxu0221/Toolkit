// <copyright file="DllBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToolKit.DecryptTool;

/// <summary>
/// DllBuilder 类用于生成项目并将其编译成 DLL 文件.
/// </summary>
public class DllBuilder
{
    /// <summary>
    /// 一个私有的只读属性，用于存储日志记录器实例.
    /// 该实例在整个 DllBuilder 类中被用来记录信息、调试和错误消息，以便于跟踪程序的执行流程和问题定位.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DllBuilder"/> class.
    /// </summary>
    /// <param name="logger">日志容器.</param>
    public DllBuilder(ILogger logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 异步生成项目并编译成DLL.
    /// </summary>
    /// <param name="outputBasePath">输出项目的基路径.</param>
    /// <param name="projectName">项目的名称.</param>
    /// <param name="sourceBasePath">源文件的基路径.</param>
    /// <param name="selectedFilePaths">要包含在项目中的文件路径集合.</param>
    /// <returns>如果项目生成和编译成功，返回true；否则返回false.</returns>
    public async Task<bool> GenerateAsync(
        string outputBasePath,
        string projectName,
        string sourceBasePath,
        IEnumerable<string> selectedFilePaths)
    {
        var projectPath = Path.Combine(outputBasePath, projectName);

        // 创建一个临时目录来存放编译好的 .resources 文件
        var tempResourcePath = Path.Combine(projectPath, "obj", "precompiled");

        this.logger.Info($"项目生成路径设置为: {projectPath}");

        try
        {
            this.CleanAndCreateDirectory(projectPath);
            Directory.CreateDirectory(tempResourcePath);

            // --- NEW STEP: Pre-compile resources ---
            var resourceFilesToEmbed = this.PrecompileResources(selectedFilePaths, tempResourcePath, sourceBasePath);

            // 2. 创建 .csproj 文件，这次只引用编译好的 .resources 文件和普通文件
            this.CreateCsprojFile(projectPath, projectName, resourceFilesToEmbed);

            // 3. 使用 dotnet CLI 编译项目
            return await this.CompileProjectAsync(projectPath);
        }
        catch (Exception ex)
        {
            this.logger.Error(ex, $"在生成项目 {projectName} 时发生错误。");
            return false;
        }
    }

    /// <summary>
    /// 预编译资源文件。将 .resx 文件编译成 .resources 文件，其他文件保持原样.
    /// </summary>
    /// <param name="sourceFiles">要预编译的源文件集合.</param>
    /// <param name="tempPath">临时目录路径，用于存放编译好的 .resources 文件.</param>
    /// <param name="sourceBasePath">源文件的基路径.</param>
    /// <returns>返回一个包含预编译后的文件路径及其逻辑名称的列表.</returns>
    private List<(string FilePath, string LogicalName)> PrecompileResources(
        IEnumerable<string> sourceFiles,
        string tempPath,
        string sourceBasePath)
    {
        this.logger.Info("开始预编译资源文件...");
        var filesToEmbed = new List<(string, string)>();
        foreach (var sourceFile in sourceFiles)
        {
            var logicalName = Path.GetRelativePath(sourceBasePath, sourceFile).Replace('\\', '.');

            if (Path.GetExtension(sourceFile).Equals(".resx", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // 将 .resx 编译成 .resources
                    var resourceFileName = Path.GetFileNameWithoutExtension(sourceFile) + ".resources";
                    var targetPath = Path.Combine(tempPath, resourceFileName);

                    using (var reader = new ResXResourceReader(sourceFile))
                    {
                        using (var writer = new ResourceWriter(targetPath))
                        {
                            foreach (DictionaryEntry de in reader)
                            {
                                writer.AddResource(de.Key.ToString(), de.Value);
                            }
                        }
                    }

                    // 使用编译后的 .resources 文件进行嵌入
                    // 逻辑名称保持与 .resx 一致（去掉 .resx 后缀）
                    var resxLogicalName = logicalName.Substring(0, logicalName.Length - ".resx".Length);
                    filesToEmbed.Add((targetPath, resxLogicalName));
                    this.logger.Debug($"已将 {sourceFile} 编译到 {targetPath}");
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, $"编译 .resx 文件 {sourceFile} 失败，将跳过此文件。");
                }
            }
            else
            {
                // 对于非 .resx 文件，直接添加
                filesToEmbed.Add((sourceFile, logicalName));
            }
        }

        this.logger.Info("资源文件预编译完成。");
        return filesToEmbed;
    }

    private void CleanAndCreateDirectory(string path)
    {
        this.logger.Debug($"正在清理和创建目录: {path}");
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        Directory.CreateDirectory(path);
        this.logger.Info("项目目录创建成功。");
    }

    /// <summary>
    /// 创建一个 .csproj 文件，用于编译项目.
    /// </summary>
    /// <param name="projectPath">项目的根路径.</param>
    /// <param name="projectName">项目的名称.</param>
    /// <param name="filesToEmbed">需要嵌入到项目中的文件及其逻辑名称的列表.</param>
    private void CreateCsprojFile(
        string projectPath,
        string projectName,
        List<(string FilePath, string LogicalName)> filesToEmbed)
    {
        var csprojPath = Path.Combine(projectPath, $"{projectName}.csproj");
        this.logger.Info($"正在创建 .csproj 文件: {csprojPath}");

        var csprojContent = new StringBuilder();
        csprojContent.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        csprojContent.AppendLine("  <PropertyGroup>");
        csprojContent.AppendLine("    <TargetFramework>net8.0</TargetFramework>");

        // 默认情况下，根命名空间是项目名称，我们需要清空它，以便我们的逻辑名能精确匹配
        csprojContent.AppendLine("    <RootNamespace></RootNamespace>");
        csprojContent.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        csprojContent.AppendLine("    <Nullable>enable</Nullable>");
        csprojContent.AppendLine("    <OutputType>Library</OutputType>");
        csprojContent.AppendLine("    <!--生成输出路径-->");
        csprojContent.AppendLine("    <OutputPath>..\\..\\..\\ProjectDlls\\</OutputPath>");
        csprojContent.AppendLine("    <!-- (使用通用输出目录) 启用后无法将Nuget引用的DLL复制到输出目录-->");
        csprojContent.AppendLine("    <UseCommonOutputDirectory>false</UseCommonOutputDirectory>");
        csprojContent.AppendLine("    <!--生成的输出路径不带Target-->");
        csprojContent.AppendLine("    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>");
        csprojContent.AppendLine(
            "    <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>");
        csprojContent.AppendLine("  </PropertyGroup>");
        csprojContent.AppendLine();
        csprojContent.AppendLine("  <ItemGroup>");

        foreach (var (filePath, logicalName) in filesToEmbed)
        {
            // 注意：这里不再使用 Link 属性，而是直接将逻辑名称作为清单资源名
            // <LogicalName> 指定了嵌入后的资源名
            // <Include> 指向文件路径
            var includePath = Path.GetRelativePath(projectPath, filePath).Replace('\\', '/');
            csprojContent.AppendLine($"    <EmbeddedResource Include=\"{includePath}\">");
            csprojContent.AppendLine($"      <LogicalName>{logicalName}</LogicalName>");
            csprojContent.AppendLine($"      <CustomToolNamespace>{logicalName}</CustomToolNamespace>");
            csprojContent.AppendLine($"    </EmbeddedResource>");
            this.logger.Debug($"添加资源: {includePath} -> (LogicalName) {logicalName}");
        }

        csprojContent.AppendLine("  </ItemGroup>");
        csprojContent.AppendLine("</Project>");

        File.WriteAllText(csprojPath, csprojContent.ToString());
        this.logger.Info(".csproj 文件内容已生成并写入。");
    }

    /// <summary>
    /// 使用 'dotnet build' 命令编译指定路径的项目.
    /// </summary>
    /// <param name="projectPath">要编译的项目的路径.</param>
    /// <returns>如果项目编译成功则返回 true，否则返回 false.</returns>
    private async Task<bool> CompileProjectAsync(string projectPath)
    {
        this.logger.Info("开始使用 'dotnet build' 命令编译项目...");
        var buildArguments = $"build \"{projectPath}\" -c Release -p:ManagePackageVersionsCentrally=false";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = buildArguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = projectPath,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            },
            EnableRaisingEvents = true,
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                outputBuilder.AppendLine(args.Data);
            }
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                errorBuilder.AppendLine(args.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        var output = outputBuilder.ToString();
        var errors = errorBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(output))
        {
            this.logger.Info($"[dotnet build] 标准输出:\n{output}");
        }

        if (!string.IsNullOrWhiteSpace(errors))
        {
            this.logger.Warn($"[dotnet build] 标准错误输出:\n{errors}");
        }

        if (process.ExitCode == 0)
        {
            this.logger.Info("项目编译成功！");
            return true;
        }
        else
        {
            this.logger.Error($"项目编译失败！退出码: {process.ExitCode}");
            return false;
        }
    }
}