namespace BgControls.Interactivity;

internal sealed class NameResolvedEventArgs : EventArgs
{
    private readonly object? newObject = null;
    private readonly object? oldObject = null;

    public object? NewObject => newObject;

    public object? OldObject => oldObject;

    public NameResolvedEventArgs(object? oldObject, object? newObject)
    {
        this.oldObject = oldObject;
        this.newObject = newObject;
    }
}