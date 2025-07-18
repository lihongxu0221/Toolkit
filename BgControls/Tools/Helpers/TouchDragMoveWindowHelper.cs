using BgControls.Tools.Interop;

namespace BgControls.Tools.Helpers;

public class TouchDragMoveWindowHelper
{
    private const int MaxMoveSpeed = 60;
    private readonly Window _window;
    private InteropValues.POINT? _lastPoint;

    public TouchDragMoveWindowHelper(Window window)
    {
        _window = window;
    }

    public void Start()
    {
        Window window = _window;

        window.PreviewMouseMove += Window_PreviewMouseMove;
        window.PreviewMouseUp += Window_PreviewMouseUp;
        window.LostMouseCapture += Window_LostMouseCapture;
    }

    public void Stop()
    {
        Window window = _window;

        window.PreviewMouseMove -= Window_PreviewMouseMove;
        window.PreviewMouseUp -= Window_PreviewMouseUp;
        window.LostMouseCapture -= Window_LostMouseCapture;
    }

    private void Window_LostMouseCapture(object sender, MouseEventArgs e) => Stop();

    private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e) => Stop();

    private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        _ = InteropMethods.GetCursorPos(out InteropValues.POINT lpPoint);

        if (_lastPoint == null)
        {
            _lastPoint = lpPoint;
            _ = _window.CaptureMouse();
        }

        var dx = lpPoint.X - _lastPoint.Value.X;
        var dy = lpPoint.Y - _lastPoint.Value.Y;

        if (Math.Abs(dx) < MaxMoveSpeed && Math.Abs(dy) < MaxMoveSpeed)
        {
            var handle = new WindowInteropHelper(_window).Handle;

            _ = InteropMethods.GetWindowRect(handle, out var lpRect);
            _ = InteropMethods.SetWindowPos(handle, IntPtr.Zero, lpRect.Left + dx, lpRect.Top + dy, 0, 0, (int)(InteropValues.WindowPositionFlags.SWP_NOSIZE | InteropValues.WindowPositionFlags.SWP_NOZORDER));
        }

        _lastPoint = lpPoint;
    }
}