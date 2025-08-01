namespace BgCommon;

/// <summary>
/// 文件夹信息扩展方法.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 复制一个文件夹的所有内容到另外一个文件夹
    /// </summary>
    /// <param name="sourceFolder">源文件夹</param>
    /// <param name="destFolder">目标文件夹</param>
    public static bool CopyFolder(string sourceFolder, string destFolder)
    {
        bool result = true;

        // 如果目标文件夹不存在，则创建它
        if (!Directory.Exists(destFolder))
        {
            _ = Directory.CreateDirectory(destFolder);
        }

        // 获取源文件夹中的所有文件
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFilePath = Path.Combine(destFolder, fileName);
            File.Copy(file, destFilePath);
        }

        // 获取源文件夹中的所有子文件夹
        string[] subFolders = Directory.GetDirectories(sourceFolder);
        foreach (string subFolder in subFolders)
        {
            string folderName = Path.GetFileName(subFolder);
            string destSubFolderPath = Path.Combine(destFolder, folderName);
            result = CopyFolder(subFolder, destSubFolderPath);
        }

        return result;
    }

    /// <summary>
    /// 通过一次性深度扫描来异步构建一个包含所有有效目录路径的哈希集合缓存。
    /// 这个方法是线程安全的，并且支持取消操作。
    /// 当扫描根目录为驱动器根目录时，会自动限制扫描深度以提高性能。
    /// </summary>
    /// <param name="baseDirectoryInfo">要扫描的根目录。</param>
    /// <param name="searchPatterns">文件搜索模式数组，例如 ["*.jpg", "*.png"]。</param>
    /// <param name="token">用于中止操作的取消令牌。</param>
    /// <param name="allowMaxDepth">驱动器目录可以下推最深的层级。</param>
    /// <returns>一个包含所有有效目录完整路径的 HashSet。</returns>
    public static async Task<HashSet<string>> BuildValidDirectoryCacheAsync(this DirectoryInfo? baseDirectoryInfo, string[] searchPatterns, CancellationToken token, int allowMaxDepth = 5)
    {
        return await Task.Run(
            () =>
            {
                var sw = new Stopwatch();
                sw.Start();

                if (token.IsCancellationRequested || baseDirectoryInfo == null)
                {
                    return new HashSet<string>();
                }

                // 使用 ConcurrentDictionary 模拟线程安全的 HashSet。
                var validPathsConcurrent = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
                string basePath = baseDirectoryInfo.FullName;

                // =================【修改点 1】=================
                // 检查是否为驱动器根目录 (例如 C:\)，如果是，则限制扫描深度。
                // 这是为了防止在选择整个驱动器时进行不必要且耗时极长的全盘扫描。
                var maxDepth = -1; // 默认-1，代表无限深度
                if (baseDirectoryInfo.Parent == null)
                {
                    maxDepth = allowMaxDepth; // 限制扫描深度为2层 (根目录 -> 子目录 -> 子子目录)
                    Trace.TraceInformation($"检测到扫描根目录为驱动器根目录，已自动限制扫描深度为 {maxDepth} 层。");
                }

                // =================【修改点 2 & 优化】=================
                // 直接将 searchPatterns 传递给 SafeEnumerateFiles，让文件系统进行预筛选，性能更高。
                // 同时传入计算出的 maxDepth。
                IEnumerable<FileInfo>? allFiles = baseDirectoryInfo.SafeEnumerateFiles(searchPatterns, token, maxDepth);
                if (allFiles == null)
                {
                    sw.Stop();
                    Trace.TraceWarning($"执行BuildValidDirectoryCacheAsync() 提前退出，耗费 {sw.ElapsedMilliseconds} ms");
                    return new HashSet<string>();
                }

                try
                {
                    _ = Parallel.ForEach(
                        allFiles,
                        new ParallelOptions { CancellationToken = token },
                        (file, loopState) =>
                        {
                            // =================【修改点 3 & 简化】=================
                            // 因为 SafeEnumerateFiles 已经根据 searchPatterns 筛选了文件，
                            // 所以这里不再需要进行 EndsWith 检查，直接处理文件即可。
                            // 这简化了逻辑并提高了效率。
                            DirectoryInfo? currentDir = file.Directory;
                            while (currentDir != null)
                            {
                                // 确保我们不会追溯到根目录之外
                                if (!currentDir.FullName.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }

                                // 使用 TryAdd，它是线程安全的。如果添加成功，则继续向上追溯父目录。
                                // 如果添加失败，意味着该路径已被其他线程添加，我们可以立即停止对该分支的追溯，
                                // 因为它的所有父路径也必然被添加过了。
                                if (!validPathsConcurrent.TryAdd(currentDir.FullName, 0))
                                {
                                    break;
                                }

                                // 如果已经添加了根目录本身，就无需再向上追溯了。
                                if (string.Equals(currentDir.FullName, basePath, StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }

                                currentDir = currentDir.Parent;
                            }
                        });
                }
                catch (OperationCanceledException)
                {
                    // Parallel.ForEach 会在取消时抛出此异常，我们捕获它以正常处理
                    Trace.TraceInformation("BuildValidDirectoryCacheAsync 操作被用户取消。");
                }

                sw.Stop();
                Trace.TraceWarning($"执行BuildValidDirectoryCacheAsync() 方法，耗费 {sw.ElapsedMilliseconds} ms");

                return new HashSet<string>(validPathsConcurrent.Keys, StringComparer.OrdinalIgnoreCase);
            },
            token
        );
    }

    /// <summary>
    /// 安全地、迭代地枚举一个目录及其所有子目录中的文件，直到达到指定的最大深度。
    /// </summary>
    /// <param name="directory">要搜索的根目录。</param>
    /// <param name="searchPatterns">文件搜索模式数组。</param>
    /// <param name="token">取消令牌。</param>
    /// <param name="maxDepth">
    /// 要遍历的最大深度。根目录为第0层。
    /// - 值为 0 表示只搜索根目录本身。
    /// - 值为 1 表示搜索根目录及其直接子目录。
    /// - 值为 -1 表示不限制深度，进行无限层级的遍历。
    /// </param>
    /// <returns>一个可枚举的文件信息集合。</returns>
    public static IEnumerable<FileInfo> SafeEnumerateFiles(this DirectoryInfo directory, string[] searchPatterns, CancellationToken token, int maxDepth = -1)
    {
        // =================【修改点 4】=================
        // 使用栈（Stack）进行迭代，存储一个包含路径和深度的元组，以避免递归并跟踪层级
        var stack = new Stack<(DirectoryInfo Dir, int Depth)>();
        stack.Push((directory, 0)); // 根目录的深度为 0

        while (stack.Count > 0)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }

            var (currentDir, currentDepth) = stack.Pop();

            // 枚举并返回当前目录中的匹配文件
            foreach (var pattern in searchPatterns)
            {
                IEnumerable<FileInfo> files;
                try
                {
                    files = currentDir.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly);
                }
                catch (UnauthorizedAccessException)
                {
                    continue; // 无法访问，跳过此模式
                }
                catch (IOException) // 例如路径过长
                {
                    continue;
                }

                foreach (FileInfo file in files)
                {
                    yield return file;
                }
            }

            // 如果设置了最大深度，并且当前深度已达到或超过该深度，则不再将子目录压入栈中
            if (maxDepth != -1 && currentDepth >= maxDepth)
            {
                continue;
            }

            // 将所有可访问的非隐藏、非系统子目录推入栈中，以便后续处理
            try
            {
                foreach (DirectoryInfo subDir in currentDir.EnumerateDirectories())
                {
                    if ((subDir.Attributes & FileAttributes.Hidden) == 0 && (subDir.Attributes & FileAttributes.System) == 0)
                    {
                        // 子目录的深度加 1
                        stack.Push((subDir, currentDepth + 1));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 无法访问子目录列表，跳过
                continue;
            }
            catch (IOException)
            {
                // 处理其他可能的I/O错误
                continue;
            }
        }
    }
}

// /// <summary>
// /// 文件夹信息扩展方法.
// /// </summary>
// public static partial class Extensions
// {
//    /// <summary>
//    /// 通过一次性深度扫描来异步构建一个包含所有有效目录路径的哈希集合缓存。
//    /// 这个方法是线程安全的，并且支持取消操作。
//    /// </summary>
//    /// <param name="baseDirectoryInfo">要扫描的根目录。</param>
//    /// <param name="searchPatterns">文件搜索模式数组，例如 ["*.jpg", "*.png"]。</param>
//    /// <param name="token">用于中止操作的取消令牌。</param>
//    /// <returns>一个包含所有有效目录完整路径的 HashSet。</returns>
//    public static async Task<HashSet<string>> BuildValidDirectoryCacheAsync(this DirectoryInfo? baseDirectoryInfo, string[] searchPatterns, CancellationToken token)
//    {
//        return await Task.Run(
//            () =>
//            {
//                var sw = new Stopwatch();
//                sw.Start();
//
//                if (token.IsCancellationRequested || baseDirectoryInfo == null)
//                {
//                    return new HashSet<string>();
//                }
//
//                // 使用 ConcurrentDictionary 模拟线程安全的 HashSet。
//                // Key 是路径，Value 可以是任何东西，通常用 byte 因为它占用内存最小。
//                var validPathsConcurrent = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
//
//                IEnumerable<FileInfo>? allFiles = baseDirectoryInfo.SafeEnumerateFiles(new[] { "*" }, token);
//                if (allFiles == null)
//                {
//                    sw.Stop();
//                    Trace.TraceWarning($"执行BuildValidDirectoryCacheAsync() 提前退出，耗费 {sw.ElapsedMilliseconds} ms");
//                    return new HashSet<string>();
//                }
//
//                var matchers = searchPatterns.Select(p => p.TrimStart('*')).ToArray();
//                string basePath = baseDirectoryInfo.FullName;
//
//                try
//                {
//                    _ = Parallel.ForEach(
//                        allFiles,
//                        new ParallelOptions { CancellationToken = token }, // 传递取消令牌
//                        (file, loopState) =>
//                        {
//                            // 在循环体内，逻辑与之前类似，但现在它在多个线程上同时运行
//                            bool isMatch = false;
//                            foreach (var m in matchers)
//                            {
//                                if (file.Name.EndsWith(m, StringComparison.OrdinalIgnoreCase))
//                                {
//                                    isMatch = true;
//                                    break;
//                                }
//                            }
//
//                            if (isMatch)
//                            {
//                                DirectoryInfo? currentDir = file.Directory;
//                                while (currentDir != null)
//                                {
//                                    if (!currentDir.FullName.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) break;
//
//                                    // 使用 TryAdd，它是线程安全的
//                                    if (!validPathsConcurrent.TryAdd(currentDir.FullName, 0))
//                                    {
//                                        // 如果添加失败，说明该路径已被其他线程添加，
//                                        // 这是一个更高效的检查和添加方式。
//                                        // 同样可以利用这个特性提前退出。
//                                        break;
//                                    }
//
//                                    if (string.Equals(currentDir.FullName, baseDirectoryInfo.FullName, StringComparison.OrdinalIgnoreCase)) break;
//
//                                    currentDir = currentDir.Parent;
//                                }
//                            }
//                        });
//                }
//                catch (OperationCanceledException)
//                {
//                    // Parallel.ForEach 会在取消时抛出此异常，我们捕获它以正常处理
//                }
//
//                sw.Stop();
//                Trace.TraceWarning($"执行BuildValidDirectoryCacheAsync() 方法，耗费 {sw.ElapsedMilliseconds} ms");
//
//                // 最后，将线程安全的 ConcurrentDictionary 转换回 HashSet 以便返回。
//                // 此时已经没有多线程写入了，所以这个转换是安全的。
//                return new HashSet<string>(validPathsConcurrent.Keys, StringComparer.OrdinalIgnoreCase);
//            },
//            token
//        );
//    }
//
//    /// <summary>
//    /// 安全地、迭代地枚举一个目录及其所有子目录中的文件，直到达到指定的最大深度。
//    /// </summary>
//    /// <param name="directory">要搜索的根目录。</param>
//    /// <param name="searchPatterns">文件搜索模式数组。</param>
//    /// <param name="token">取消令牌。</param>
//    /// <param name="maxDepth">要遍历的最大深度。-1 表示无限深度。</param>
//    /// <returns>一个可枚举的文件信息集合。</returns>
//    public static IEnumerable<FileInfo> SafeEnumerateFiles(this DirectoryInfo directory, string[] searchPatterns, CancellationToken token, int maxDepth = -1)
//    {
//        #region MyRegion
//        // 使用栈（Stack）进行迭代，避免深度递归导致的堆栈溢出
//        var stack = new Stack<string>();
//        stack.Push(directory.FullName);
//        while (stack.Count > 0)
//        {
//            // 检查是否已取消操作
//            if (token.IsCancellationRequested)
//            {
//                yield break; // 取消操作，退出迭代
//            }
//
//            string currentDirPath = stack.Pop();
//            DirectoryInfo currentDir;
//            try
//            {
//                currentDir = new DirectoryInfo(currentDirPath);
//
//                if (currentDir.Parent == null ||
//                    ((currentDir.Attributes & FileAttributes.Hidden) == 0 && (currentDir.Attributes & FileAttributes.System) == 0))
//                {
//                    // 如果当前目录不是隐藏或系统目录，则继续处理
//                    // 将所有子目录推入栈中，以便后续处理
//                    foreach (DirectoryInfo subDir in currentDir.EnumerateDirectories())
//                    {
//                        if ((subDir.Attributes & FileAttributes.Hidden) == 0 && (subDir.Attributes & FileAttributes.System) == 0)
//                        {
//                            stack.Push(subDir.FullName);
//                        }
//                    }
//                }
//                else
//                {
//                    // 如果是隐藏或系统目录，则跳过
//                    continue;
//                }
//            }
//            catch (UnauthorizedAccessException)
//            {
//                // 如果无法访问当前目录的子目录列表，则跳过
//                continue;
//            }
//            catch (IOException)
//            {
//                // 处理其他可能的I/O错误，例如路径太长
//                continue;
//            }
//
//            // 枚举并返回当前目录中的文件
//            foreach (var pattern in searchPatterns)
//            {
//                IEnumerable<FileInfo> files;
//                try
//                {
//                    // 步骤 1: 尝试获取文件枚举器。这是唯一会抛出异常的地方。
//                    files = currentDir.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly);
//                }
//                catch (UnauthorizedAccessException)
//                {
//                    // 如果无法访问这个模式的文件列表，就直接跳到下一个模式。
//                    continue;
//                }
//
//                // 如果上一步成功，这里的循环就不会抛出权限异常。
//                foreach (FileInfo file in files)
//                {
//                    yield return file;
//                }
//            }
//        }
//
//        #endregion
//    }
// }