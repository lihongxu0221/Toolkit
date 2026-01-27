namespace RoslynPad.UI;

internal sealed class Disposer : IDisposable
{
    private readonly Action onDispose;

    public Disposer(Action onDispose)
    {
        this.onDispose = onDispose;
    }

    public void Dispose()
    {
        onDispose?.Invoke();
        GC.SuppressFinalize(this);
    }
}