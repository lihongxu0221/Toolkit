using Standard;

namespace Microsoft.Windows.Shell;

internal class WindowChromeWorker : DependencyObject
{
    private const SWP _SwpFlags = SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOSIZE | SWP.NOZORDER;

    private readonly List<KeyValuePair<WM, MessageHandler>> _messageTable;
    public static readonly DependencyProperty WindowChromeWorkerProperty = DependencyProperty.RegisterAttached("WindowChromeWorker", typeof(WindowChromeWorker), typeof(WindowChromeWorker), new PropertyMetadata(null, new PropertyChangedCallback(WindowChromeWorker.OnChromeWorkerChanged)));
    private static readonly HT[,] _HitTestBorders;

    private Window _window;
    private HwndSource _hwndSource;
    private WindowChrome _chromeInfo;
    private IntPtr _hwnd = IntPtr.Zero;
    private bool _isHooked;
    private bool _isFixedUp;
    private bool _isUserResizing;
    private bool _hasUserMovedWindow;
    private Point _windowPosAtStartOfUserMove = default(Point);
    private int _blackGlassFixupAttemptCount;
    private WindowState _lastRoundingState;
    private WindowState _lastMenuState;
    private bool _isGlassEnabled;

    private delegate void _Action();

    static WindowChromeWorker()
    {
        HT[,] array = new HT[3, 3];
        array[0, 0] = HT.TOPLEFT;
        array[0, 1] = HT.TOP;
        array[0, 2] = HT.TOPRIGHT;
        array[1, 0] = HT.LEFT;
        array[1, 1] = HT.CLIENT;
        array[1, 2] = HT.RIGHT;
        array[2, 0] = HT.BOTTOMLEFT;
        array[2, 1] = HT.BOTTOM;
        array[2, 2] = HT.BOTTOMRIGHT;
        _HitTestBorders = array;
    }

    public WindowChromeWorker()
    {
        _messageTable = new List<KeyValuePair<WM, MessageHandler>>
        {
            new KeyValuePair<WM, MessageHandler>(WM.SETTEXT, new MessageHandler(_HandleSetTextOrIcon)),
            new KeyValuePair<WM, MessageHandler>(WM.SETICON, new MessageHandler(_HandleSetTextOrIcon)),
            new KeyValuePair<WM, MessageHandler>(WM.NCACTIVATE, new MessageHandler(_HandleNCActivate)),
            new KeyValuePair<WM, MessageHandler>(WM.NCCALCSIZE, new MessageHandler(_HandleNCCalcSize)),
            new KeyValuePair<WM, MessageHandler>(WM.NCHITTEST, new MessageHandler(_HandleNCHitTest)),
            new KeyValuePair<WM, MessageHandler>(WM.NCRBUTTONUP, new MessageHandler(_HandleNCRButtonUp)),
            new KeyValuePair<WM, MessageHandler>(WM.SIZE, new MessageHandler(_HandleSize)),
            new KeyValuePair<WM, MessageHandler>(WM.WINDOWPOSCHANGED, new MessageHandler(_HandleWindowPosChanged)),
            new KeyValuePair<WM, MessageHandler>(WM.DWMCOMPOSITIONCHANGED, new MessageHandler(_HandleDwmCompositionChanged))
        };
        if (Utility.IsPresentationFrameworkVersionLessThan4)
        {
            _messageTable.AddRange(new KeyValuePair<WM, MessageHandler>[]
            {
                new KeyValuePair<WM, MessageHandler>(WM.WININICHANGE, new MessageHandler(_HandleSettingChange)),
                new KeyValuePair<WM, MessageHandler>(WM.ENTERSIZEMOVE, new MessageHandler(_HandleEnterSizeMove)),
                new KeyValuePair<WM, MessageHandler>(WM.EXITSIZEMOVE, new MessageHandler(_HandleExitSizeMove)),
                new KeyValuePair<WM, MessageHandler>(WM.MOVE, new MessageHandler(_HandleMove))
            });
        }
    }

    public void SetWindowChrome(WindowChrome newChrome)
    {
        VerifyAccess();
        if (newChrome == _chromeInfo)
        {
            return;
        }

        if (_chromeInfo != null)
        {
            _chromeInfo.PropertyChangedThatRequiresRepaint -= OnChromePropertyChangedThatRequiresRepaint;
        }

        _chromeInfo = newChrome;
        if (_chromeInfo != null)
        {
            _chromeInfo.PropertyChangedThatRequiresRepaint += OnChromePropertyChangedThatRequiresRepaint;
        }

        ApplyNewCustomChrome();
    }

    private void OnChromePropertyChangedThatRequiresRepaint(object sender, EventArgs e)
    {
        _UpdateFrameState(true);
    }

    private static void OnChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Window window = (Window)d;
        WindowChromeWorker windowChromeWorker = (WindowChromeWorker)e.NewValue;
        windowChromeWorker.SetWindow(window);
    }

    private void SetWindow(Window window)
    {
        _window = window;
        _hwnd = new WindowInteropHelper(_window).Handle;
        if (Utility.IsPresentationFrameworkVersionLessThan4)
        {
            Utility.AddDependencyPropertyChangeListener(_window, Control.TemplateProperty, new EventHandler(OnWindowPropertyChangedThatRequiresTemplateFixup));
            Utility.AddDependencyPropertyChangeListener(_window, FrameworkElement.FlowDirectionProperty, new EventHandler(OnWindowPropertyChangedThatRequiresTemplateFixup));
        }

        _window.Closed += UnsetWindow;
        if (IntPtr.Zero != _hwnd)
        {
            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _window.ApplyTemplate();
            if (_chromeInfo != null)
            {
                ApplyNewCustomChrome();
                return;
            }
        }
        else
        {
            _window.SourceInitialized += delegate (object sender, EventArgs e)
            {
                _hwnd = new WindowInteropHelper(_window).Handle;
                _hwndSource = HwndSource.FromHwnd(_hwnd);
                if (_chromeInfo != null)
                {
                    ApplyNewCustomChrome();
                }
            };
        }
    }

    private void UnsetWindow(object? sender, EventArgs e)
    {
        if (Utility.IsPresentationFrameworkVersionLessThan4)
        {
            Utility.RemoveDependencyPropertyChangeListener(_window, Control.TemplateProperty, new EventHandler(OnWindowPropertyChangedThatRequiresTemplateFixup));
            Utility.RemoveDependencyPropertyChangeListener(_window, FrameworkElement.FlowDirectionProperty, new EventHandler(OnWindowPropertyChangedThatRequiresTemplateFixup));
        }

        if (_chromeInfo != null)
        {
            _chromeInfo.PropertyChangedThatRequiresRepaint -= OnChromePropertyChangedThatRequiresRepaint;
        }

        _RestoreStandardChromeState(true);
    }

    public static WindowChromeWorker GetWindowChromeWorker(Window window)
    {
        Verify.IsNotNull<Window>(window, "window");
        return (WindowChromeWorker)window.GetValue(WindowChromeWorker.WindowChromeWorkerProperty);
    }

    public static void SetWindowChromeWorker(Window window, WindowChromeWorker chrome)
    {
        Verify.IsNotNull<Window>(window, "window");
        window.SetValue(WindowChromeWorker.WindowChromeWorkerProperty, chrome);
    }

    private void OnWindowPropertyChangedThatRequiresTemplateFixup(object sender, EventArgs e)
    {
        if (_chromeInfo != null && _hwnd != IntPtr.Zero)
        {
            _ = _window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(FixupFrameworkIssues));
        }
    }

    private void ApplyNewCustomChrome()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        if (_chromeInfo == null)
        {
            _RestoreStandardChromeState(false);
            return;
        }

        if (!_isHooked)
        {
            _hwndSource.AddHook(new HwndSourceHook(_WndProc));
            _isHooked = true;
        }

        FixupFrameworkIssues();
        _UpdateSystemMenu(new WindowState?(_window.WindowState));
        _UpdateFrameState(true);
        _ = NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOSIZE | SWP.NOZORDER);
    }

    private void FixupFrameworkIssues()
    {
        if (!Utility.IsPresentationFrameworkVersionLessThan4)
        {
            return;
        }

        if (_window.Template == null)
        {
            return;
        }

        if (VisualTreeHelper.GetChildrenCount(_window) == 0)
        {
            _ = _window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(FixupFrameworkIssues));
            return;
        }

        FrameworkElement frameworkElement = (FrameworkElement)VisualTreeHelper.GetChild(_window, 0);
        RECT windowRect = NativeMethods.GetWindowRect(_hwnd);
        RECT rect = _GetAdjustedWindowRect(windowRect);
        Rect rect2 = DpiHelper.DeviceRectToLogical(new Rect((double)windowRect.Left, (double)windowRect.Top, (double)windowRect.Width, (double)windowRect.Height));
        Rect rect3 = DpiHelper.DeviceRectToLogical(new Rect((double)rect.Left, (double)rect.Top, (double)rect.Width, (double)rect.Height));
        Thickness thickness = new Thickness(rect2.Left - rect3.Left, rect2.Top - rect3.Top, rect3.Right - rect2.Right, rect3.Bottom - rect2.Bottom);
        frameworkElement.Margin = new Thickness(0.0, 0.0, -(thickness.Left + thickness.Right), -(thickness.Top + thickness.Bottom));
        if (_window.FlowDirection == FlowDirection.RightToLeft)
        {
            frameworkElement.RenderTransform = new MatrixTransform(1.0, 0.0, 0.0, 1.0, -(thickness.Left + thickness.Right), 0.0);
        }
        else
        {
            frameworkElement.RenderTransform = null;
        }

        if (!_isFixedUp)
        {
            _hasUserMovedWindow = false;
            _window.StateChanged += _FixupRestoreBounds;
            _isFixedUp = true;
        }
    }

    private void _FixupWindows7Issues()
    {
        if (_blackGlassFixupAttemptCount > 5)
        {
            return;
        }
        if (Utility.IsOSWindows7OrNewer && NativeMethods.DwmIsCompositionEnabled())
        {
            _blackGlassFixupAttemptCount++;
            bool flag = false;
            try
            {
                flag = (NativeMethods.DwmGetCompositionTimingInfo(_hwnd) != null);
            }
            catch (Exception)
            {
            }
            if (!flag)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new WindowChromeWorker._Action(_FixupWindows7Issues));
                return;
            }
            _blackGlassFixupAttemptCount = 0;
        }
    }

    private void _FixupRestoreBounds(object sender, EventArgs e)
    {
        if ((_window.WindowState == WindowState.Maximized || _window.WindowState == WindowState.Minimized) && _hasUserMovedWindow)
        {
            _hasUserMovedWindow = false;
            WINDOWPLACEMENT windowPlacement = NativeMethods.GetWindowPlacement(_hwnd);
            RECT rect = _GetAdjustedWindowRect(new RECT
            {
                Bottom = 100,
                Right = 100
            });
            Point point = DpiHelper.DevicePixelsToLogical(new Point((double)(windowPlacement.rcNormalPosition.Left - rect.Left), (double)(windowPlacement.rcNormalPosition.Top - rect.Top)));
            _window.Top = point.Y;
            _window.Left = point.X;
        }
    }

    private RECT _GetAdjustedWindowRect(RECT rcWindow)
    {
        WS dwStyle = (WS)((int)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE));
        WS_EX dwExStyle = (WS_EX)((int)NativeMethods.GetWindowLongPtr(_hwnd, GWL.EXSTYLE));
        return NativeMethods.AdjustWindowRectEx(rcWindow, dwStyle, false, dwExStyle);
    }

    private bool _IsWindowDocked
    {
        get
        {
            if (_window.WindowState != WindowState.Normal)
            {
                return false;
            }
            RECT rect = _GetAdjustedWindowRect(new RECT
            {
                Bottom = 100,
                Right = 100
            });
            Point point = new Point(_window.Left, _window.Top);
            point -= (Vector)DpiHelper.DevicePixelsToLogical(new Point((double)rect.Left, (double)rect.Top));
            return _window.RestoreBounds.Location != point;
        }
    }

    private IntPtr _WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        foreach (KeyValuePair<WM, MessageHandler> keyValuePair in _messageTable)
        {
            if (keyValuePair.Key == (WM)msg)
            {
                return keyValuePair.Value((WM)msg, wParam, lParam, out handled);
            }
        }
        return IntPtr.Zero;
    }

    private IntPtr _HandleSetTextOrIcon(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        bool flag = _ModifyStyle(WS.VISIBLE, WS.OVERLAPPED);
        IntPtr result = NativeMethods.DefWindowProc(_hwnd, uMsg, wParam, lParam);
        if (flag)
        {
            _ModifyStyle(WS.OVERLAPPED, WS.VISIBLE);
        }
        handled = true;
        return result;
    }

    private IntPtr _HandleNCActivate(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        IntPtr result = NativeMethods.DefWindowProc(_hwnd, WM.NCACTIVATE, wParam, new IntPtr(-1));
        handled = true;
        return result;
    }

    private IntPtr _HandleNCCalcSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        handled = true;
        return new IntPtr(768);
    }

    private IntPtr _HandleNCHitTest(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        IntPtr intPtr = IntPtr.Zero;
        handled = false;
        if (Utility.IsOSVistaOrNewer && _chromeInfo.GlassFrameThickness != default(Thickness) && _isGlassEnabled)
        {
            handled = NativeMethods.DwmDefWindowProc(_hwnd, uMsg, wParam, lParam, out intPtr);
        }
        if (IntPtr.Zero == intPtr)
        {
            Point point = new Point((double)Utility.GET_X_LPARAM(lParam), (double)Utility.GET_Y_LPARAM(lParam));
            Rect deviceRectangle = _GetWindowRect();
            HT ht = _HitTestNca(DpiHelper.DeviceRectToLogical(deviceRectangle), DpiHelper.DevicePixelsToLogical(point));
            if (ht != HT.CLIENT)
            {
                Point point2 = point;
                point2.Offset(-deviceRectangle.X, -deviceRectangle.Y);
                point2 = DpiHelper.DevicePixelsToLogical(point2);
                IInputElement inputElement = _window.InputHitTest(point2);
                if (inputElement != null && WindowChrome.GetIsHitTestVisibleInChrome(inputElement))
                {
                    ht = HT.CLIENT;
                }
            }
            handled = true;
            intPtr = new IntPtr((int)ht);
        }
        return intPtr;
    }

    private IntPtr _HandleNCRButtonUp(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        if (2 == wParam.ToInt32())
        {
            SystemCommands.ShowSystemMenuPhysicalCoordinates(_window, new Point((double)Utility.GET_X_LPARAM(lParam), (double)Utility.GET_Y_LPARAM(lParam)));
        }
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        WindowState? assumeState = null;
        if (wParam.ToInt32() == 2)
        {
            assumeState = new WindowState?(WindowState.Maximized);
        }
        _UpdateSystemMenu(assumeState);
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleWindowPosChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        _UpdateSystemMenu(null);
        if (!_isGlassEnabled)
        {
            WINDOWPOS value = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
            _SetRoundingRegion(new WINDOWPOS?(value));
        }
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleDwmCompositionChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        _UpdateFrameState(false);
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleSettingChange(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        FixupFrameworkIssues();
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleEnterSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        _isUserResizing = true;
        if (_window.WindowState != WindowState.Maximized && !_IsWindowDocked)
        {
            _windowPosAtStartOfUserMove = new Point(_window.Left, _window.Top);
        }
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleExitSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        _isUserResizing = false;
        if (_window.WindowState == WindowState.Maximized)
        {
            _window.Top = _windowPosAtStartOfUserMove.Y;
            _window.Left = _windowPosAtStartOfUserMove.X;
        }
        handled = false;
        return IntPtr.Zero;
    }

    private IntPtr _HandleMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
    {
        if (_isUserResizing)
        {
            _hasUserMovedWindow = true;
        }
        handled = false;
        return IntPtr.Zero;
    }

    private bool _ModifyStyle(WS removeStyle, WS addStyle)
    {
        WS ws = (WS)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE).ToInt32();
        WS ws2 = (ws & ~removeStyle) | addStyle;
        if (ws == ws2)
        {
            return false;
        }
        NativeMethods.SetWindowLongPtr(_hwnd, GWL.STYLE, new IntPtr((int)ws2));
        return true;
    }

    private WindowState _GetHwndState()
    {
        WINDOWPLACEMENT windowPlacement = NativeMethods.GetWindowPlacement(_hwnd);
        return windowPlacement.showCmd switch
        {
            SW.SHOWMINIMIZED => WindowState.Minimized,
            SW.SHOWMAXIMIZED => WindowState.Maximized,
            _ => WindowState.Normal
        };
    }

    private Rect _GetWindowRect()
    {
        RECT windowRect = NativeMethods.GetWindowRect(_hwnd);
        return new Rect((double)windowRect.Left, (double)windowRect.Top, (double)windowRect.Width, (double)windowRect.Height);
    }

    private void _UpdateSystemMenu(WindowState? assumeState)
    {
        WindowState windowState = assumeState ?? _GetHwndState();
        if (assumeState != null || _lastMenuState != windowState)
        {
            _lastMenuState = windowState;
            bool flag = _ModifyStyle(WS.VISIBLE, WS.OVERLAPPED);
            IntPtr systemMenu = NativeMethods.GetSystemMenu(_hwnd, false);
            if (IntPtr.Zero != systemMenu)
            {
                WS value = (WS)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE).ToInt32();
                bool flag2 = Utility.IsFlagSet((int)value, 131072);
                bool flag3 = Utility.IsFlagSet((int)value, 65536);
                bool flag4 = Utility.IsFlagSet((int)value, 262144);
                switch (windowState)
                {
                    case WindowState.Minimized:
                        NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.ENABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, flag3 ? MF.ENABLED : (MF.GRAYED | MF.DISABLED));
                        break;
                    case WindowState.Maximized:
                        NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.ENABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, flag2 ? MF.ENABLED : (MF.GRAYED | MF.DISABLED));
                        NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, MF.GRAYED | MF.DISABLED);
                        break;
                    default:
                        NativeMethods.EnableMenuItem(systemMenu, SC.RESTORE, MF.GRAYED | MF.DISABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.MOVE, MF.ENABLED);
                        NativeMethods.EnableMenuItem(systemMenu, SC.SIZE, flag4 ? MF.ENABLED : (MF.GRAYED | MF.DISABLED));
                        NativeMethods.EnableMenuItem(systemMenu, SC.MINIMIZE, flag2 ? MF.ENABLED : (MF.GRAYED | MF.DISABLED));
                        NativeMethods.EnableMenuItem(systemMenu, SC.MAXIMIZE, flag3 ? MF.ENABLED : (MF.GRAYED | MF.DISABLED));
                        break;
                }
            }
            if (flag)
            {
                _ModifyStyle(WS.OVERLAPPED, WS.VISIBLE);
            }
        }
    }

    private void _UpdateFrameState(bool force)
    {
        if (IntPtr.Zero == _hwnd)
        {
            return;
        }
        bool flag = NativeMethods.DwmIsCompositionEnabled();
        if (force || flag != _isGlassEnabled)
        {
            _isGlassEnabled = (flag && _chromeInfo.GlassFrameThickness != default(Thickness));
            if (!_isGlassEnabled)
            {
                _SetRoundingRegion(null);
            }
            else
            {
                _ClearRoundingRegion();
                _ExtendGlassFrame();
                _FixupWindows7Issues();
            }
            NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOSIZE | SWP.NOZORDER);
        }
    }

    private void _ClearRoundingRegion()
    {
        NativeMethods.SetWindowRgn(_hwnd, IntPtr.Zero, NativeMethods.IsWindowVisible(_hwnd));
    }

    private void _SetRoundingRegion(WINDOWPOS? wp)
    {
        WINDOWPLACEMENT windowPlacement = NativeMethods.GetWindowPlacement(_hwnd);
        if (windowPlacement.showCmd == SW.SHOWMAXIMIZED)
        {
            int num;
            int num2;
            if (wp != null)
            {
                num = wp.Value.x;
                num2 = wp.Value.y;
            }
            else
            {
                Rect rect = _GetWindowRect();
                num = (int)rect.Left;
                num2 = (int)rect.Top;
            }
            IntPtr hMonitor = NativeMethods.MonitorFromWindow(_hwnd, 2u);
            MONITORINFO monitorInfo = NativeMethods.GetMonitorInfo(hMonitor);
            RECT rcWork = monitorInfo.rcWork;
            rcWork.Offset(-num, -num2);
            IntPtr hRgn = IntPtr.Zero;
            try
            {
                hRgn = NativeMethods.CreateRectRgnIndirect(rcWork);
                NativeMethods.SetWindowRgn(_hwnd, hRgn, NativeMethods.IsWindowVisible(_hwnd));
                hRgn = IntPtr.Zero;
                return;
            }
            finally
            {
                Utility.SafeDeleteObject(ref hRgn);
            }
        }
        Size size;
        if (wp != null && !Utility.IsFlagSet(wp.Value.flags, 1))
        {
            size = new Size((double)wp.Value.cx, (double)wp.Value.cy);
        }
        else
        {
            if (wp != null && _lastRoundingState == _window.WindowState)
            {
                return;
            }
            size = _GetWindowRect().Size;
        }
        _lastRoundingState = _window.WindowState;
        IntPtr intPtr = IntPtr.Zero;
        try
        {
            double num3 = Math.Min(size.Width, size.Height);
            double num4 = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.CornerRadius.TopLeft, 0.0)).X;
            num4 = Math.Min(num4, num3 / 2.0);
            if (WindowChromeWorker._IsUniform(_chromeInfo.CornerRadius))
            {
                intPtr = WindowChromeWorker._CreateRoundRectRgn(new Rect(size), num4);
            }
            else
            {
                intPtr = WindowChromeWorker._CreateRoundRectRgn(new Rect(0.0, 0.0, size.Width / 2.0 + num4, size.Height / 2.0 + num4), num4);
                double num5 = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.CornerRadius.TopRight, 0.0)).X;
                num5 = Math.Min(num5, num3 / 2.0);
                Rect region = new Rect(0.0, 0.0, size.Width / 2.0 + num5, size.Height / 2.0 + num5);
                region.Offset(size.Width / 2.0 - num5, 0.0);
                WindowChromeWorker._CreateAndCombineRoundRectRgn(intPtr, region, num5);
                double num6 = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.CornerRadius.BottomLeft, 0.0)).X;
                num6 = Math.Min(num6, num3 / 2.0);
                Rect region2 = new Rect(0.0, 0.0, size.Width / 2.0 + num6, size.Height / 2.0 + num6);
                region2.Offset(0.0, size.Height / 2.0 - num6);
                WindowChromeWorker._CreateAndCombineRoundRectRgn(intPtr, region2, num6);
                double num7 = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.CornerRadius.BottomRight, 0.0)).X;
                num7 = Math.Min(num7, num3 / 2.0);
                Rect region3 = new Rect(0.0, 0.0, size.Width / 2.0 + num7, size.Height / 2.0 + num7);
                region3.Offset(size.Width / 2.0 - num7, size.Height / 2.0 - num7);
                WindowChromeWorker._CreateAndCombineRoundRectRgn(intPtr, region3, num7);
            }
            NativeMethods.SetWindowRgn(_hwnd, intPtr, NativeMethods.IsWindowVisible(_hwnd));
            intPtr = IntPtr.Zero;
        }
        finally
        {
            Utility.SafeDeleteObject(ref intPtr);
        }
    }

    private static IntPtr _CreateRoundRectRgn(Rect region, double radius)
    {
        if (DoubleUtilities.AreClose(0.0, radius))
        {
            return NativeMethods.CreateRectRgn((int)Math.Floor(region.Left), (int)Math.Floor(region.Top), (int)Math.Ceiling(region.Right), (int)Math.Ceiling(region.Bottom));
        }
        return NativeMethods.CreateRoundRectRgn((int)Math.Floor(region.Left), (int)Math.Floor(region.Top), (int)Math.Ceiling(region.Right) + 1, (int)Math.Ceiling(region.Bottom) + 1, (int)Math.Ceiling(radius), (int)Math.Ceiling(radius));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HRGNs")]
    private static void _CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
    {
        IntPtr hrgnSrc = IntPtr.Zero;
        try
        {
            hrgnSrc = WindowChromeWorker._CreateRoundRectRgn(region, radius);
            if (NativeMethods.CombineRgn(hrgnSource, hrgnSource, hrgnSrc, RGN.OR) == CombineRgnResult.ERROR)
            {
                throw new InvalidOperationException("Unable to combine two HRGNs.");
            }
        }
        catch
        {
            Utility.SafeDeleteObject(ref hrgnSrc);
            throw;
        }
    }

    private static bool _IsUniform(CornerRadius cornerRadius)
    {
        return DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.BottomRight) && DoubleUtilities.AreClose(cornerRadius.TopLeft, cornerRadius.TopRight) && DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.TopRight);
    }

    private void _ExtendGlassFrame()
    {
        if (!Utility.IsOSVistaOrNewer)
        {
            return;
        }
        if (IntPtr.Zero == _hwnd)
        {
            return;
        }
        if (!NativeMethods.DwmIsCompositionEnabled())
        {
            _hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
            return;
        }
        _hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
        Point point = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.GlassFrameThickness.Left, _chromeInfo.GlassFrameThickness.Top));
        Point point2 = DpiHelper.LogicalPixelsToDevice(new Point(_chromeInfo.GlassFrameThickness.Right, _chromeInfo.GlassFrameThickness.Bottom));
        MARGINS margins = new MARGINS
        {
            cxLeftWidth = (int)Math.Ceiling(point.X),
            cxRightWidth = (int)Math.Ceiling(point2.X),
            cyTopHeight = (int)Math.Ceiling(point.Y),
            cyBottomHeight = (int)Math.Ceiling(point2.Y)
        };
        NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins);
    }

    private HT _HitTestNca(Rect windowPosition, Point mousePosition)
    {
        int num = 1;
        int num2 = 1;
        bool flag = false;
        if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + _chromeInfo.ResizeBorderThickness.Top + _chromeInfo.CaptionHeight)
        {
            flag = (mousePosition.Y < windowPosition.Top + _chromeInfo.ResizeBorderThickness.Top);
            num = 0;
        }
        else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (double)((int)_chromeInfo.ResizeBorderThickness.Bottom))
        {
            num = 2;
        }
        if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (double)((int)_chromeInfo.ResizeBorderThickness.Left))
        {
            num2 = 0;
        }
        else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - _chromeInfo.ResizeBorderThickness.Right)
        {
            num2 = 2;
        }
        if (num == 0 && num2 != 1 && !flag)
        {
            num = 1;
        }
        HT ht = WindowChromeWorker._HitTestBorders[num, num2];
        if (ht == HT.TOP && !flag)
        {
            ht = HT.CAPTION;
        }
        return ht;
    }

    private void _RestoreStandardChromeState(bool isClosing)
    {
        VerifyAccess();
        _UnhookCustomChrome();
        if (!isClosing)
        {
            _RestoreFrameworkIssueFixups();
            _RestoreGlassFrame();
            _RestoreHrgn();
            _window.InvalidateMeasure();
        }
    }

    private void _UnhookCustomChrome()
    {
        if (_isHooked)
        {
            _hwndSource.RemoveHook(new HwndSourceHook(_WndProc));
            _isHooked = false;
        }
    }

    private void _RestoreFrameworkIssueFixups()
    {
        if (Utility.IsPresentationFrameworkVersionLessThan4)
        {
            FrameworkElement frameworkElement = (FrameworkElement)VisualTreeHelper.GetChild(_window, 0);
            frameworkElement.Margin = default(Thickness);
            _window.StateChanged -= _FixupRestoreBounds;
            _isFixedUp = false;
        }
    }

    private void _RestoreGlassFrame()
    {
        if (!Utility.IsOSVistaOrNewer || _hwnd == IntPtr.Zero)
        {
            return;
        }
        _hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
        if (NativeMethods.DwmIsCompositionEnabled())
        {
            MARGINS margins = default(MARGINS);
            NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins);
        }
    }

    private void _RestoreHrgn()
    {
        _ClearRoundingRegion();
        NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOSIZE | SWP.NOZORDER);
    }
}
