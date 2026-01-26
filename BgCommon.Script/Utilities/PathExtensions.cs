namespace BgCommon.Script;

/// <summary>
/// 提供多种PATH相关的扩展方法.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    /// 获取命令脚本的版本。如果在脚本的父文件夹中未找到版本文件（例如 `1.0.0.0.version`），则返回 `0.0.0.0`.
    /// </summary>
    /// <param name="scriptFile">脚本文件路径.</param>
    public static string GetCommandScriptVersion(this string scriptFile)
    {
        string version = "0.0.0.0";

        try
        {
            if (scriptFile.HasText() && File.Exists(scriptFile))
            {
                string? scriptDirectory = Path.GetDirectoryName(scriptFile);
                if (!string.IsNullOrEmpty(scriptDirectory))
                {
                    string? versionFile = Directory.GetFiles(scriptDirectory, "*.version").FirstOrDefault();
                    return Path.GetFileNameWithoutExtension(versionFile ?? "0.0.0.0");
                }
            }
        }
        catch
        {
            // 发生任何错误时，返回默认版本.
        }

        return version;
    }

    /// <summary>
    /// 复制文件.
    /// </summary>
    /// <param name="srcFileName">源文件路径.</param>
    /// <param name="destFileName">目标文件路径.</param>
    /// <param name="ignoreErrors">如果设置为 <c>true</c>，则忽略错误.</param>
    public static void FileCopy(this string srcFileName, string destFileName, bool ignoreErrors = false)
    {
        try
        {
            File.Copy(srcFileName, destFileName, true);
        }
        catch
        {
            if (!ignoreErrors)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// 更改文件的扩展名.
    /// </summary>
    /// <param name="path">文件路径.</param>
    /// <param name="extension">新的扩展名.</param>
    /// <returns>带有新扩展名的路径.</returns>
    public static string ChangeExtension(this string path, string extension) => Path.ChangeExtension(path, extension);

    /// <summary>
    /// 获取文件的扩展名.
    /// </summary>
    /// <param name="path">文件路径.</param>
    /// <returns>文件的扩展名.</returns>
    public static string GetExtension(this string path) => Path.GetExtension(path);

    /// <summary>
    /// 从完整路径中获取文件名部分.
    /// </summary>
    /// <param name="path">文件路径.</param>
    /// <returns>文件名.</returns>
    public static string GetFileName(this string path) => Path.GetFileName(path);

    /// <summary>
    /// 检查目录是否存在.
    /// </summary>
    /// <param name="path">目录路径.</param>
    /// <returns>如果目录存在，则为 true；否则为 false.</returns>
    public static bool DirExists(this string path) => Directory.Exists(path);

    /// <summary>
    /// 获取路径的绝对路径.
    /// </summary>
    /// <param name="path">文件或目录路径.</param>
    /// <returns>绝对路径.</returns>
    public static string GetFullPath(this string path) => Path.GetFullPath(path);

    /// <summary>
    /// 判断路径是否为目录.
    /// </summary>
    /// <param name="path">文件或目录路径.</param>
    /// <returns>
    ///   如果路径是目录，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool IsDir(this string path) => Directory.Exists(path);

    /// <summary>
    /// 判断指定的路径字符串是否有效（不包含无效字符）.
    /// </summary>
    /// <param name="path">路径字符串.</param>
    /// <returns>
    ///   如果路径有效，则为 <c>true</c>；否则为 <c>false</c>.
    /// </returns>
    public static bool IsValidPath(this string path) => path.IndexOfAny(Path.GetInvalidPathChars()) == -1;

    /// <summary>
    /// 一个比 <see cref="Path.Combine(string[])"/> 更方便的API版本.
    /// </summary>
    /// <param name="path">基础路径.</param>
    /// <param name="parts">要组合的路径部分.</param>
    /// <returns>组合后的新路径.</returns>
    public static string PathJoin(this string path, params object[] parts)
    {
        var allParts = new List<string> { path };
        allParts.AddRange(parts.Select(p => p?.ToString() ?? string.Empty));
        return Path.Combine(allParts.ToArray());
    }

    /// <summary>
    /// 获取特殊文件夹的路径.
    /// </summary>
    /// <param name="folder">特殊文件夹枚举.</param>
    /// <returns>文件夹路径.</returns>
    public static string GetPath(this Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);

    /// <summary>
    /// 获取特殊文件夹的路径，并将其与一组字符串数组合成一个新路径.
    /// </summary>
    /// <param name="folder">特殊文件夹枚举.</param>
    /// <param name="parts">要组合的路径部分.</param>
    public static string GetPath(this Environment.SpecialFolder folder, params object[] parts)
    {
        var allParts = new List<string> { Environment.GetFolderPath(folder) };
        allParts.AddRange(parts.Select(p => p?.ToString() ?? string.Empty));
        return Path.Combine(allParts.ToArray());
    }

    /// <summary>
    /// 检查路径是否可写.
    /// </summary>
    internal static bool IsWritable(this string path)
    {
        string testFile = Path.Combine(path, Guid.NewGuid().ToString());
        try
        {
            File.WriteAllText(testFile, string.Empty);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            // 尽力清除测试文件.
            try
            {
                _ = testFile.DeleteIfExists();
            }
            catch { }
        }
    }

    /// <summary>
    /// 如果文件或目录存在，则删除它.
    /// </summary>
    internal static string DeleteIfExists(this string path, bool recursive = false)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }
        catch
        {
            // 静默忽略错误.
        }

        return path;
    }

    /// <summary>
    /// 确保目录存在；如果不存在，则创建它.
    /// </summary>
    /// <param name="path">目录路径.</param>
    /// <param name="rethrow">如果设置为 <c>true</c>，则在发生错误时重新抛出异常.</param>
    /// <returns>如果目录已存在或创建成功，则为 true.</returns>
    public static bool EnsureDir(this string path, bool rethrow = true)
    {
        try
        {
            _ = Directory.CreateDirectory(path);
            return true;
        }
        catch
        {
            if (rethrow)
            {
                throw;
            }

            return false;
        }
    }

    /// <summary>
    /// 确保文件的父目录存在.
    /// </summary>
    /// <param name="file">文件路径.</param>
    /// <param name="rethrow">如果设置为 <c>true</c>，则在发生错误时重新抛出异常.</param>
    /// <returns>成功时的文件路径，否则为空字符串.</returns>
    public static string EnsureFileDir(this string file, bool rethrow = true)
    {
        try
        {
            if (file.GetDirName().EnsureDir())
            {
                return file;
            }
        }
        catch
        {
            if (rethrow)
            {
                throw;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 删除目录及其所有内容.
    /// </summary>
    /// <param name="path">目录路径.</param>
    /// <param name="handleExceptions">如果设置为 <c>true</c>，则处理异常.</param>
    /// <param name="doNotDeleteRoot">如果设置为 <c>true</c>，则不删除根目录本身.</param>
    /// <returns>原始目录路径.</returns>
    public static string DeleteDir(this string path, bool handleExceptions = false, bool doNotDeleteRoot = false)
    {
        if (!Directory.Exists(path))
        {
            return path;
        }

        try
        {
            if (doNotDeleteRoot)
            {
                // 删除所有内容，但保留根目录.
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }

                foreach (string dir in Directory.GetDirectories(path))
                {
                    Directory.Delete(dir, true);
                }
            }
            else
            {
                // 使用.NET的标准方法递归删除整个目录，效率更高且更安全.
                Directory.Delete(path, true);
            }
        }
        catch
        {
            if (!handleExceptions)
            {
                throw;
            }
        }

        return path;
    }

    /// <summary>
    /// 检查文件是否存在.
    /// </summary>
    /// <param name="path">文件路径.</param>
    /// <returns>如果文件存在，则为 true；否则为 false.</returns>
    public static bool FileExists(this string? path)
    {
        try
        {
            return File.Exists(path);
        }
        catch
        {
            // 捕获无效路径等异常.
            return false;
        }
    }

    /// <summary>
    /// 从路径中获取目录名.
    /// </summary>
    /// <param name="path">完整路径.</param>
    /// <returns>目录路径.</returns>
    public static string GetDirName(this string path) => Path.GetDirectoryName(path) ?? string.Empty;

    /// <summary>
    /// 更改路径中的文件名.
    /// </summary>
    /// <param name="path">原始路径.</param>
    /// <param name="fileName">新的文件名.</param>
    /// <returns>包含新文件名的新路径.</returns>
    public static string ChangeFileName(this string path, string fileName) => Path.Combine(path.GetDirName(), fileName);

    /// <summary>
    /// 获取不带扩展名的文件名.
    /// </summary>
    /// <param name="path">文件路径.</param>
    /// <returns>不带扩展名的文件名.</returns>
    public static string GetFileNameWithoutExtension(this string path) => Path.GetFileNameWithoutExtension(path);

    /// <summary>
    /// 规范化路径中的目录分隔符，以确保与目标文件系统兼容.
    /// </summary>
    /// <param name="path">路径字符串.</param>
    /// <returns>规范化后的新路径.</returns>
    public static string PathNormalizeSeparators(this string path)
    {
        return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// 获取指定目录路径下的子目录.
    /// </summary>
    /// <param name="path">目录路径.</param>
    /// <param name="mask">搜索掩码.</param>
    /// <returns>发现的目录列表.</returns>
    public static string[] PathGetDirs(this string path, string mask)
    {
        return Directory.GetDirectories(path, mask);
    }
}