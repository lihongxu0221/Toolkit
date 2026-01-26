namespace BgControls.Interactivity;

public interface IAttachedObject
{
    DependencyObject? AssociatedObject { get; }

    void Attach(DependencyObject? dependencyObject);

    void Detach();

}