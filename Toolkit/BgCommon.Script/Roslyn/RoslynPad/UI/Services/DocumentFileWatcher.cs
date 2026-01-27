namespace RoslynPad.UI;

public enum DocumentFileChangeType
{
    Created,
    Deleted,
    Renamed
}

public class DocumentFileChanged
{
    public DocumentFileChangeType Type { get; }

    public string Path { get; }

    public string? NewPath { get; }

    public DocumentFileChanged(DocumentFileChangeType type, string path, string? newPath = null)
    {
        Type = type;
        Path = path;
        NewPath = newPath;
    }
}

public class DocumentFileWatcher : IDisposable, IObservable<DocumentFileChanged>
{
    private readonly IUIService uiService;
    private readonly FileSystemWatcher fileSystemWatcher;
    private readonly List<IObserver<DocumentFileChanged>> observers = new List<IObserver<DocumentFileChanged>>();
    private bool isDisposed;

    [ImportingConstructor]
    public DocumentFileWatcher(IUIService uiService)
    {
        this.uiService = uiService;
        observers = [];
        fileSystemWatcher = new FileSystemWatcher();
        fileSystemWatcher.Created += OnChanged;
        fileSystemWatcher.Renamed += OnRenamed;
        fileSystemWatcher.Deleted += OnChanged;
        fileSystemWatcher.IncludeSubdirectories = true;
    }

    public string Path
    {
        get => fileSystemWatcher.Path;
        set
        {
            var exists = Directory.Exists(value);
            if (exists)
            {
                fileSystemWatcher.Path = value;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            else
            {
                fileSystemWatcher.EnableRaisingEvents = false;
            }

            observers.Clear();
        }
    }

    private void OnChanged(object? sender, FileSystemEventArgs e)
    {
        Publish(new DocumentFileChanged(ToDocumentFileChangeType(e.ChangeType), e.FullPath));
    }

    private void OnRenamed(object? sender, RenamedEventArgs e)
    {
        Publish(new DocumentFileChanged(ToDocumentFileChangeType(e.ChangeType), e.OldFullPath, e.FullPath));
    }

    private void Publish(DocumentFileChanged documentFileChanged)
    {
        _ = this.uiService.RunOnUIThreadAsync(() =>
        {
            foreach (IObserver<DocumentFileChanged> observer in observers.ToArray())
            {
                observer.OnNext(documentFileChanged);
            }
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (!isDisposed)
        {
            if (isDisposing)
            {
                fileSystemWatcher.Created -= OnChanged;
                fileSystemWatcher.Renamed -= OnRenamed;
                fileSystemWatcher.Deleted -= OnChanged;
                fileSystemWatcher?.Dispose();
            }

            isDisposed = true;
        }
    }

    public IDisposable Subscribe(IObserver<DocumentFileChanged> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Disposer(() => observers.Remove(observer));
    }

    private static DocumentFileChangeType ToDocumentFileChangeType(WatcherChangeTypes changeType)
    {
        return changeType switch
        {
            WatcherChangeTypes.Created => DocumentFileChangeType.Created,
            WatcherChangeTypes.Deleted => DocumentFileChangeType.Deleted,
            WatcherChangeTypes.Renamed => DocumentFileChangeType.Renamed,
            _ => throw new ArgumentOutOfRangeException(nameof(changeType), changeType, null),
        };
    }
}
