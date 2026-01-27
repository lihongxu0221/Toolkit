namespace BgCommon.Script.CodeDom;

/// <summary>
/// 一个用于管理临时文件路径的集合。
/// 该集合会在被释放时（通过 using 语句或手动调用 Dispose）自动删除其跟踪的所有文件。
/// </summary>
public class TempFileCollection : Collection<string>, IDisposable
{
    private bool _disposed;

    public TempFileCollection()
        : base()
    {
    }

    ~TempFileCollection()
    {
        Dispose(false);
    }

    /// <summary>
    /// 删除集合中跟踪的所有临时文件。
    /// 此方法会忽略任何因文件不存在或无法删除而引发的错误。
    /// </summary>
    public void DeleteAllFiles()
    {
        foreach (var file in Items)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // 忽略删除失败的异常，确保清理过程尽可能继续执行。
            }
        }
    }

    /// <summary>
    /// 释放资源，并删除集合中引用的所有临时文件。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        // 通知垃圾回收器不需要再调用此对象的析构函数。
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 核心的释放逻辑。
    /// </summary>
    /// <param name="disposing">如果为 true，表示由用户代码调用，可以释放托管和非托管资源。如果为 false，表示由终结器调用，仅能释放非托管资源。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 在这里可以释放其他托管资源（如果有的话）。
            }

            // 释放我们的非托管资源（文件）。
            DeleteAllFiles();
            _disposed = true;
        }
    }
}