namespace BgCommon.Serialization.Implementation.Internal;

public sealed class WeakChangedEventDelegate : WeakReference
{
    private readonly MethodInfo _handlerMethod;

    public WeakChangedEventDelegate(object handlerInstance, MethodInfo handlerMethod)
        : base(handlerInstance)
    {
        if (handlerMethod == null)
        {
            throw new ArgumentNullException("handlerMethod");
        }
        _handlerMethod = handlerMethod;
    }

    public void RegisterChangedHandler(IChangedEvent source)
    {
        if (IsAlive)
        {
            source.Changed += ChangedHandler;
        }
    }

    public void UnregisterChangedHandler(IChangedEvent source)
    {
        if (IsAlive)
        {
            source.Changed -= ChangedHandler;
        }
    }

    private void ChangedHandler(object sender, ChangedEventArgs e)
    {
        if (IsAlive)
        {
            _handlerMethod.Invoke(Target, new object[2] { sender, e });
        }
        else if (sender is IChangedEvent changedEvent)
        {
            changedEvent.Changed -= ChangedHandler;
        }
    }
}