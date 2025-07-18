namespace BgCommon.Serialization;

/// <summary>
/// ScriptTempFileManager 作为单例实例使用，
/// 用于跟踪在执行期间或涉及脚本编写和调试模式的自定义应用程序执行过程中临时创建的文件。
/// 在应用程序退出时，该类负责删除执行期间创建的临时文件。
/// </summary>
public sealed class ScriptTempFileManager
{
    private static ScriptTempFileManager? _instance;

    private List<string> tempFileNames = new List<string>();

    private static string _FolderName = "Scripts";

    public static ScriptTempFileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ScriptTempFileManager();
            }

            return _instance;
        }
    }

    public int TempFileCount => tempFileNames.Count;

    private ScriptTempFileManager()
    {
        DeleteTempFilesOnInit();
    }

    public void AddTempFileName(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            tempFileNames.Add(fileName);
        }
    }

    ~ScriptTempFileManager()
    {
        DeleteTempFilesOnExit();
    }

    private void DeleteTempFilesOnInit()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(GetScriptTempDir());
        FileInfo[] files = directoryInfo.GetFiles("*.dll");
        foreach (FileInfo fileInfo in files)
        {
            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                // 这里只是占位，不做具体处理，可根据实际情况添加日志等操作
                _ = ex.Message;
            }
        }
    }

    private void DeleteTempFilesOnExit()
    {
        if (tempFileNames.Count <= 0)
        {
            return;
        }

        foreach (string tempFileName in tempFileNames)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetScriptTempDir());
            FileInfo[] files = directoryInfo.GetFiles($"{tempFileName}.*");
            foreach (FileInfo fileInfo in files)
            {
                try
                {
                    fileInfo.Delete();
                }
                catch (Exception ex)
                {
                    // 这里只是占位，不做具体处理，可根据实际情况添加日志等操作
                    _ = ex.Message;
                }
            }
        }
    }

    public static string GetScriptTempDir()
    {
        string text = Path.Combine(Path.GetTempPath(), _FolderName);
        try
        {
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
        }
        catch (Exception ex)
        {
            // 这里只是占位，不做具体处理，可根据实际情况添加日志等操作
            _ = ex.Message;
        }
        return text;
    }
}