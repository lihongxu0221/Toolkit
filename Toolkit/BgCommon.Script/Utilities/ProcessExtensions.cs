namespace BgCommon.Script;

/// <summary>
/// 包含进程执行结果的类。
/// </summary>
public class ProcessResult
{
    /// <summary>
    /// Gets 进程的退出代码。
    /// </summary>
    public int ExitCode { get; }

    /// <summary>
    /// Gets 进程的标准输出的完整内容。
    /// </summary>
    public string StandardOutput { get; }

    /// <summary>
    /// Gets 进程的标准错误的完整内容。
    /// </summary>
    public string StandardError { get; }

    public ProcessResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }
}

/// <summary>
/// 提供与进程相关的同步和异步扩展方法。
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    /// 同步运行一个进程，等待其完成，并处理其输出。
    /// </summary>
    /// <remarks>
    /// 此方法会阻塞调用线程直到进程退出或超时。在UI或Web应用中请优先使用异步版本 <see cref="RunAsync"/>。
    /// </remarks>
    /// <param name="exeFile">可执行文件的路径。</param>
    /// <param name="arguments">传递给进程的命令行参数。</param>
    /// <param name="workingDirectory">进程的工作目录。</param>
    /// <param name="onOutput">用于处理标准输出的回调函数（逐行处理）。</param>
    /// <param name="onError">用于处理标准错误的回调函数（逐行处理）。</param>
    /// <param name="timeout">等待进程结束的超时时间（毫秒）。-1表示无限期等待。</param>
    /// <param name="terminateOnTimeout">如果超时，是否终止进程。</param>
    /// <returns>进程的退出代码。如果超时或发生错误，可能返回-1。</returns>
    public static int Run(
        this string exeFile,
        string arguments,
        string? workingDirectory = "",
        Action<string>? onOutput = null,
        Action<string>? onError = null,
        int timeout = -1,
        bool terminateOnTimeout = true)
    {
        using (Process process = CreateProcess(exeFile, arguments, workingDirectory))
        {
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    onOutput?.Invoke(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    onError?.Invoke(args.Data);
                }
            };

            _ = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (!process.WaitForExit(timeout))
            {
                if (terminateOnTimeout)
                {
                    try
                    {
                        onOutput?.Invoke($"进程 '{exeFile}' 超时。正在强制终止。");
                        process.Kill();
                    }
                    catch
                    {
                        /* 忽略终止失败的异常 */
                    }
                }

                return -1;
            }

            process.WaitForExit(); // 确保异步事件处理完毕
            return process.ExitCode;
        }
    }

    /// <summary>
    /// 异步运行一个进程，通过回调实时处理输出，并返回进程的退出代码。
    /// </summary>
    /// <remarks>
    /// 此方法不会阻塞调用线程，非常适用于UI和Web应用。
    /// </remarks>
    /// <param name="exeFile">可执行文件的路径。</param>
    /// <param name="arguments">传递给进程的命令行参数。</param>
    /// <param name="workingDirectory">进程的工作目录。</param>
    /// <param name="onOutput">用于处理标准输出的回调函数（逐行处理）。</param>
    /// <param name="onError">用于处理标准错误的回调函数（逐行处理）。</param>
    /// <param name="cancellationToken">用于取消操作的CancellationToken。</param>
    /// <returns>一个表示异步操作的任务，其结果是进程的退出代码。</returns>
    public static Task<int> RunAsync(
        this string exeFile,
        string arguments,
        string? workingDirectory = "",
        Action<string>? onOutput = null,
        Action<string>? onError = null,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<int>();

        Process process = CreateProcess(exeFile, arguments, workingDirectory);
        process.EnableRaisingEvents = true;

        process.Exited += (sender, args) =>
        {
            // 等待所有异步输出读取操作完成，确保回调函数接收到所有输出
            process.WaitForExit();
            _ = tcs.TrySetResult(process.ExitCode);
            process.Dispose();
        };

        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                onOutput?.Invoke(args.Data);
            }
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                onError?.Invoke(args.Data);
            }
        };

        // 注册CancellationToken，如果任务被取消，则终止进程
        _ = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch
            {
                /* 忽略异常 */
            }

            _ = tcs.TrySetCanceled();
        });

        _ = process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return tcs.Task;
    }

    /// <summary>
    /// 异步运行一个进程，收集其所有输出，并在其完成后返回一个包含退出代码和输出内容的结果对象。
    /// </summary>
    /// <param name="exeFile">可执行文件的路径。</param>
    /// <param name="arguments">传递给进程的命令行参数。</param>
    /// <param name="workingDirectory">进程的工作目录。</param>
    /// <param name="cancellationToken">用于取消操作的CancellationToken。</param>
    /// <returns>一个表示异步操作的任务，其结果是一个包含退出代码和完整输出的 <see cref="ProcessResult"/> 对象。</returns>
    public static Task<ProcessResult> RunAsync(
         this string exeFile,
         string arguments,
         string? workingDirectory = "",
         CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<ProcessResult>();
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        Process process = CreateProcess(exeFile, arguments, workingDirectory);
        process.EnableRaisingEvents = true;

        process.Exited += (sender, args) =>
        {
            process.WaitForExit();
            var result = new ProcessResult(process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
            _ = tcs.TrySetResult(result);
            process.Dispose();
        };

        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                _ = outputBuilder.AppendLine(args.Data);
            }
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                _ = errorBuilder.AppendLine(args.Data);
            }
        };

        _ = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch
            {
                /* 忽略异常 */
            }

            _ = tcs.TrySetCanceled();
        });

        _ = process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return tcs.Task;
    }

    /// <summary>
    /// 创建一个Process对象，并配置其启动信息以重定向输出流，但不启动它。
    /// </summary>
    private static Process CreateProcess(string exeFile, string? arguments, string? workingDirectory)
    {
        var process = new Process();
        process.StartInfo.FileName = exeFile;
        process.StartInfo.Arguments = arguments ?? string.Empty;
        process.StartInfo.WorkingDirectory = workingDirectory ?? string.Empty;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.ErrorDialog = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        return process;
    }

    /// <summary>
    /// 检查一个由进程ID命名的临时文件所对应的进程当前是否仍在运行。
    /// </summary>
    /// <remarks>
    /// 此方法假定临时文件的文件名格式为 "PID.extension" (例如 "12345.tmp")。
    /// 它比枚举所有系统进程的性能要高出几个数量级。
    /// </remarks>
    /// <param name="tempFile">临时文件的完整路径。</param>
    /// <returns>如果进程仍在运行，则为 true；否则为 false。</returns>
    public static bool IsOwnerProcessRunning(this string tempFile)
    {
        if (string.IsNullOrEmpty(tempFile))
        {
            return false;
        }

        // 1. 使用 Path.GetFileName 获取文件名，更健壮
        string fileName = Path.GetFileName(tempFile);

        // 2. 使用 Split 获取点号前的部分，比 IndexOf + Substring 更清晰
        string? pidString = fileName.Split('.').FirstOrDefault();

        if (pidString == null || !int.TryParse(pidString, out int pid))
        {
            // 如果文件名格式不正确（没有点号或点号前不是数字），则直接返回 false
            return false;
        }

        try
        {
            // 3. (核心性能优化) 直接尝试通过ID获取进程。
            // 这是检查进程是否存在的最高效方法。
            // 如果进程不存在，它会抛出 ArgumentException，我们捕获这个异常即可。
            using (Process process = Process.GetProcessById(pid))
            {
                // 如果能成功获取到Process对象，说明进程存在。
                // HasExited 是一个廉价的检查，可以处理进程在获取后立即退出的边缘情况。
                return !process.HasExited;
            }
        }
        catch (ArgumentException)
        {
            // 这是预期的异常：当进程ID不存在时，GetProcessById会抛出此异常。
            // 这明确地告诉我们进程没有在运行。
            return false;
        }
        catch (InvalidOperationException)
        {
            // 边缘情况：如果在我们调用 GetProcessById 之后，但在访问 HasExited 之前，
            // 进程退出了，可能会抛出此异常。这也意味着进程没有在运行。
            return false;
        }
    }
}