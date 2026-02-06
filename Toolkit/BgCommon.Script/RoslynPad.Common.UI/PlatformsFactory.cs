using Microsoft.Win32;
using NuGet.Versioning;
using RoslynPad.Build;
using System.Runtime.InteropServices;

namespace RoslynPad.UI;

/// <summary>
/// 平台工厂类，用于检测和获取系统上安装的 .NET 及 .NET Framework 版本.
/// </summary>
// [Export(typeof(IPlatformsFactory))]
internal class PlatformsFactory : IPlatformsFactory
{
    /// <summary>
    /// 缓存的执行平台列表.
    /// </summary>
    private IReadOnlyList<ExecutionPlatform>? executionPlatforms;

    /// <summary>
    /// 缓存的 .NET 可执行文件及 SDK 路径信息.
    /// </summary>
    private (string dotnetExe, string sdkPath) dotnetPaths;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformsFactory"/> class.
    /// </summary>
    public PlatformsFactory()
    {
        // 初始化时获取所有的 .NET 和 .NET Framework 版本并转换为只读数组.
        this.executionPlatforms = this.GetNetVersions().Concat(this.GetNetFrameworkVersions()).ToArray().AsReadOnly();
    }

    /// <summary>
    /// 获取所有可用的执行平台列表.
    /// </summary>
    /// <returns>返回执行平台集合.</returns>
    public IReadOnlyList<ExecutionPlatform> GetExecutionPlatforms()
    {
        // 返回缓存的平台列表.
        return this.executionPlatforms ?? Array.Empty<ExecutionPlatform>();
    }

    /// <summary>
    /// Gets .NET 可执行文件的路径.
    /// </summary>
    public string DotNetExecutable
    {
        get
        {
            // 通过查找 SDK 方法获取 dotnet 可执行文件路径.
            return this.FindNetSdk().dotnetExe;
        }
    }

    /// <summary>
    /// 获取系统中安装的 .NET (Core) 版本.
    /// </summary>
    /// <returns>返回 .NET 平台枚举集合.</returns>
    private IEnumerable<ExecutionPlatform> GetNetVersions()
    {
        // 获取 SDK 根路径.
        (string _, string sdkPath) = this.FindNetSdk();

        if (string.IsNullOrEmpty(sdkPath))
        {
            return Enumerable.Empty<ExecutionPlatform>();
        }

        // 存储解析出的版本信息.
        var versionInfos = new List<(string name, string tfm, NuGetVersion version)>();

        // 遍历 SDK 目录下的子目录.
        foreach (var sdkDirectoryPath in IOUtilities.EnumerateDirectories(sdkPath))
        {
            var sdkVersionName = Path.GetFileName(sdkDirectoryPath);
            // 尝试解析目录名为 NuGet 版本号.
            if (NuGetVersion.TryParse(sdkVersionName, out NuGetVersion? parsedVersion) && parsedVersion != null && parsedVersion.Major > 1)
            {
                // 根据主版本号判断是 .NET Core 还是 .NET 5+.
                var platformName = parsedVersion.Major < 5 ? ".NET Core" : ".NET";
                var targetFrameworkMoniker = parsedVersion.Major < 5
                    ? $"netcoreapp{parsedVersion.Major}.{parsedVersion.Minor}"
                    : $"net{parsedVersion.Major}.{parsedVersion.Minor}";

                versionInfos.Add((platformName, targetFrameworkMoniker, parsedVersion));
            }
        }

        // 按预览版状态排序，然后按版本号倒序排列.
        return versionInfos.OrderBy(v => v.version.IsPrerelease).ThenByDescending(v => v.version)
            .Select(v => new ExecutionPlatform(v.name, v.tfm, v.version, Architecture.X64, isDotNet: true));
    }

    /// <summary>
    /// 获取系统中安装的 .NET Framework 版本（仅限 Windows）.
    /// </summary>
    /// <returns>返回 .NET Framework 平台集合.</returns>
    private IEnumerable<ExecutionPlatform> GetNetFrameworkVersions()
    {
        // 检查当前是否为 Windows 操作系统且为 x64 架构.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.OSArchitecture == Architecture.X64)
        {
            // 从注册表获取框架名称.
            var targetFrameworkName = PlatformsFactory.GetNetFrameworkName();
            if (!string.IsNullOrEmpty(targetFrameworkName))
            {
                yield return new ExecutionPlatform(".NET Framework x64", targetFrameworkName, null, Architecture.X64, isDotNet: false);
            }
        }
    }

    /// <summary>
    /// 查找并确定 .NET SDK 的安装路径和可执行文件位置.
    /// </summary>
    /// <returns>包含可执行文件路径和 SDK 路径的元组.</returns>
    private (string dotnetExe, string sdkPath) FindNetSdk()
    {
        // 如果已经缓存了路径，则直接返回.
        if (this.dotnetPaths.dotnetExe is not null)
        {
            return this.dotnetPaths;
        }

        // 构建潜在的 dotnet 安装路径列表.
        List<string> candidateSearchPaths = new List<string>();

        // 从环境变量 DOTNET_ROOT 获取.
        var dotnetRootEnv = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(dotnetRootEnv))
        {
            candidateSearchPaths.Add(dotnetRootEnv);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows 下常见的 64 位程序安装目录.
            var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (!string.IsNullOrEmpty(programFiles))
            {
                candidateSearchPaths.Add(Path.Combine(programFiles, "dotnet"));
            }
        }
        else
        {
            // 非 Windows 系统的常见路径.
            candidateSearchPaths.AddRange(new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet"),
                "/usr/lib/dotnet",
                "/usr/lib64/dotnet",
                "/usr/share/dotnet",
                "/usr/local/share/dotnet",
            });
        }

        var dotnetExeFileName = PlatformsFactory.GetDotnetExe();

        // 在所有候选路径中寻找同时包含可执行文件和 sdk 目录的路径.
        var resolvedPaths = (from path in candidateSearchPaths
                             let exePath = Path.Combine(path, dotnetExeFileName)
                             let sdkFolderPath = Path.Combine(path, "sdk")
                             where File.Exists(exePath) && Directory.Exists(sdkFolderPath)
                             select (exePath, sdkFolderPath)).FirstOrDefault();

        // 如果未找到任何路径，初始化为空字符串.
        if (resolvedPaths.exePath is null)
        {
            resolvedPaths = (string.Empty, string.Empty);
        }

        // 更新缓存并返回.
        this.dotnetPaths = resolvedPaths;
        return resolvedPaths;
    }

    /// <summary>
    /// 根据当前操作系统获取 dotnet 可执行文件的名称.
    /// </summary>
    /// <returns>返回 "dotnet.exe" 或 "dotnet".</returns>
    private static string GetDotnetExe()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet";
    }

    /// <summary>
    /// 从注册表中读取安装的 .NET Framework 名称.
    /// </summary>
    /// <returns>返回框架标识符，如 "net48".</returns>
    private static string GetNetFrameworkName()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return string.Empty;
        }

        const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

        // 打开 HKLM 注册表基项读取版本发布值.
        using (RegistryKey? ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
        {
            var releaseValue = ndpKey?.GetValue("Release") as int?;
            if (releaseValue != null)
            {
                return PlatformsFactory.GetNetFrameworkTargetName(releaseValue.Value);
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 根据注册表中的 Release Key 映射对应的目标框架名称.
    /// </summary>
    /// <param name="releaseKey">注册表中的版本发布数值.</param>
    /// <returns>返回 TFM 字符串.</returns>
    /// <exception cref="ArgumentOutOfRangeException">当 releaseKey 低于已知支持的最小版本时抛出.</exception>
    private static string GetNetFrameworkTargetName(int releaseKey)
    {
        return releaseKey switch
        {
            >= 528040 => "net48",
            >= 461808 => "net472",
            >= 461308 => "net471",
            >= 460798 => "net47",
            >= 394802 => "net462",
            >= 394254 => "net461",
            >= 393295 => "net46",
            _ => throw new ArgumentOutOfRangeException(nameof(releaseKey))
        };
    }
}