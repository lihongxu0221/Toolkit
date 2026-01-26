using BgControls.Controls.Datas;
using BgControls.Tools;
using BgControls.Tools.Interop;

namespace BgControls.Controls;

[TemplatePart(Name = ElementBanner, Type = typeof(UIElement))]
[TemplatePart(Name = ElementLeftArea, Type = typeof(UIElement))]
[TemplatePart(Name = ElementRightArea, Type = typeof(UIElement))]
[TemplatePart(Name = ElementNonClientArea, Type = typeof(UIElement))]
public class BGWindow : Window
{
    private const string ResourceToken = "WindowDefaultStyle";
    private const string ElementNonClientArea = "PART_NonClientArea";
    private const string ElementLeftArea = "PART_LeftArea";
    private const string ElementRightArea = "PART_RightArea";
    private const string ElementBanner = "PART_BannerArea";

    public static readonly DependencyProperty NonClientAreaContentProperty = DependencyProperty.Register(nameof(NonClientAreaContent), typeof(UIElement), typeof(BGWindow), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty BannerAreaProperty = DependencyProperty.Register(nameof(BannerArea), typeof(UIElement), typeof(BGWindow), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty LeftAreaProperty = DependencyProperty.Register(nameof(LeftArea), typeof(UIElement), typeof(BGWindow), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty RightAreaProperty = DependencyProperty.Register(nameof(RightArea), typeof(UIElement), typeof(BGWindow), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty CloseButtonHoverBackgroundProperty = DependencyProperty.Register(nameof(CloseButtonHoverBackground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty CloseButtonHoverForegroundProperty = DependencyProperty.Register(nameof(CloseButtonHoverForeground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(nameof(CloseButtonBackground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(Brushes.Transparent));
    public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(nameof(CloseButtonForeground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(Brushes.White));
    public static readonly DependencyProperty OtherButtonBackgroundProperty = DependencyProperty.Register(nameof(OtherButtonBackground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(Brushes.Transparent));
    public static readonly DependencyProperty OtherButtonForegroundProperty = DependencyProperty.Register(nameof(OtherButtonForeground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(Brushes.White));
    public static readonly DependencyProperty OtherButtonHoverBackgroundProperty = DependencyProperty.Register(nameof(OtherButtonHoverBackground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty OtherButtonHoverForegroundProperty = DependencyProperty.Register(nameof(OtherButtonHoverForeground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty NonClientAreaBackgroundProperty = DependencyProperty.Register(nameof(NonClientAreaBackground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty NonClientAreaForegroundProperty = DependencyProperty.Register(nameof(NonClientAreaForeground), typeof(Brush), typeof(BGWindow), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty NonClientAreaHeightProperty = DependencyProperty.Register(nameof(NonClientAreaHeight), typeof(double), typeof(BGWindow), new PropertyMetadata(22.0));
    public static readonly DependencyProperty ShowNonClientAreaProperty = DependencyProperty.Register(nameof(ShowNonClientArea), typeof(bool), typeof(BGWindow), new PropertyMetadata(ValueBoxes.TrueBox, OnShowNonClientAreaChanged));
    public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(BGWindow), new PropertyMetadata(ValueBoxes.TrueBox));
    public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(nameof(IsFullScreen), typeof(bool), typeof(BGWindow), new PropertyMetadata(ValueBoxes.FalseBox, OnIsFullScreenChanged));
    public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(BGWindow), new PropertyMetadata(ValueBoxes.TrueBox));
    public static readonly DependencyProperty ShowLeftAreaProperty = DependencyProperty.Register(nameof(ShowLeftArea), typeof(bool), typeof(BGWindow), new PropertyMetadata(true, OnIsShowLeftAreaChanged));
    public static readonly DependencyProperty ShowRightAreaProperty = DependencyProperty.Register(nameof(ShowRightArea), typeof(bool), typeof(BGWindow), new PropertyMetadata(true, OnIsShowRightAreaChanged));

    private bool _isFullScreen;
    private Thickness _actualBorderThickness;
    private readonly Thickness _commonPadding;
    private double _tempNonClientAreaHeight;
    private WindowState _tempWindowState;
    private WindowStyle _tempWindowStyle;
    private ResizeMode _tempResizeMode;
    private UIElement? _nonClientArea;
    private bool _showNonClientArea = true;
    private UIElement? _bannerArea;
    private UIElement? _leftArea;
    private UIElement? _rightArea;
    private bool _showLeftArea = true;
    private bool _showRightArea = true;

    public UIElement NonClientAreaContent
    {
        get => (UIElement)GetValue(NonClientAreaContentProperty);
        set => SetValue(NonClientAreaContentProperty, value);
    }

    public UIElement BannerArea
    {
        get => (UIElement)GetValue(BannerAreaProperty);
        set => SetValue(BannerAreaProperty, value);
    }

    public UIElement LeftArea
    {
        get => (UIElement)GetValue(LeftAreaProperty);
        set => SetValue(LeftAreaProperty, value);
    }

    public UIElement RightArea
    {
        get => (UIElement)GetValue(RightAreaProperty);
        set => SetValue(RightAreaProperty, value);
    }

    public bool ShowLeftArea
    {
        get => (bool)GetValue(ShowLeftAreaProperty);
        set => SetValue(ShowLeftAreaProperty, value);
    }

    public bool ShowRightArea
    {
        get => (bool)GetValue(ShowRightAreaProperty);
        set => SetValue(ShowRightAreaProperty, value);
    }

    public Brush CloseButtonHoverBackground
    {
        get => (Brush)GetValue(CloseButtonHoverBackgroundProperty);
        set => SetValue(CloseButtonHoverBackgroundProperty, value);
    }

    public Brush CloseButtonHoverForeground
    {
        get => (Brush)GetValue(CloseButtonHoverForegroundProperty);
        set => SetValue(CloseButtonHoverForegroundProperty, value);
    }

    public Brush CloseButtonBackground
    {
        get => (Brush)GetValue(CloseButtonBackgroundProperty);
        set => SetValue(CloseButtonBackgroundProperty, value);
    }

    public Brush CloseButtonForeground
    {
        get => (Brush)GetValue(CloseButtonForegroundProperty);
        set => SetValue(CloseButtonForegroundProperty, value);
    }

    public Brush OtherButtonBackground
    {
        get => (Brush)GetValue(OtherButtonBackgroundProperty);
        set => SetValue(OtherButtonBackgroundProperty, value);
    }

    public Brush OtherButtonForeground
    {
        get => (Brush)GetValue(OtherButtonForegroundProperty);
        set => SetValue(OtherButtonForegroundProperty, value);
    }

    public Brush OtherButtonHoverBackground
    {
        get => (Brush)GetValue(OtherButtonHoverBackgroundProperty);
        set => SetValue(OtherButtonHoverBackgroundProperty, value);
    }

    public Brush OtherButtonHoverForeground
    {
        get => (Brush)GetValue(OtherButtonHoverForegroundProperty);
        set => SetValue(OtherButtonHoverForegroundProperty, value);
    }

    public Brush NonClientAreaBackground
    {
        get => (Brush)GetValue(NonClientAreaBackgroundProperty);
        set => SetValue(NonClientAreaBackgroundProperty, value);
    }

    public Brush NonClientAreaForeground
    {
        get => (Brush)GetValue(NonClientAreaForegroundProperty);
        set => SetValue(NonClientAreaForegroundProperty, value);
    }

    public double NonClientAreaHeight
    {
        get => (double)GetValue(NonClientAreaHeightProperty);
        set => SetValue(NonClientAreaHeightProperty, value);
    }

    public bool ShowNonClientArea
    {
        get => (bool)GetValue(ShowNonClientAreaProperty);
        set => SetValue(ShowNonClientAreaProperty, ValueBoxes.BooleanBox(value));
    }

    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, ValueBoxes.BooleanBox(value));
    }

    public bool IsFullScreen
    {
        get => (bool)GetValue(IsFullScreenProperty);
        set => SetValue(IsFullScreenProperty, ValueBoxes.BooleanBox(value));
    }

    public bool ShowIcon
    {
        get => (bool)GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    static BGWindow()
    {
        StyleProperty.OverrideMetadata(typeof(BGWindow), new FrameworkPropertyMetadata(ResourceHelper.GetResourceInternal<Style>(ResourceToken)));
    }

    public BGWindow()
    {
        var chrome = new WindowChrome
        {
            UseAeroCaptionButtons = false,
            GlassFrameThickness = new Thickness(0, 0, 0, 1),
            CornerRadius = new CornerRadius(0)
        };

        _ = BindingOperations.SetBinding(chrome, WindowChrome.CaptionHeightProperty, new Binding(NonClientAreaHeightProperty.Name) { Source = this });
        WindowChrome.SetWindowChrome(this, chrome);
        _commonPadding = Padding;

        Loaded += (s, e) => OnLoaded(e);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _nonClientArea = GetTemplateChild(ElementNonClientArea) as UIElement;
        _bannerArea = GetTemplateChild(ElementBanner) as UIElement;
        _leftArea = GetTemplateChild(ElementLeftArea) as UIElement;
        _rightArea = GetTemplateChild(ElementRightArea) as UIElement;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        this.GetHwndSource()?.AddHook(HwndSourceHook);
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Maximized)
        {
            BorderThickness = new Thickness(0);
            _tempNonClientAreaHeight = NonClientAreaHeight;
            NonClientAreaHeight += 8;
        }
        else
        {
            BorderThickness = _actualBorderThickness;
            NonClientAreaHeight = _tempNonClientAreaHeight;
        }
    }

    protected void OnLoaded(RoutedEventArgs args)
    {
        _actualBorderThickness = BorderThickness;
        _tempNonClientAreaHeight = NonClientAreaHeight;

        if (WindowState == WindowState.Maximized)
        {
            BorderThickness = new Thickness(0);
            _tempNonClientAreaHeight += 8;
        }

        _ = CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (s, e) => WindowState = WindowState.Minimized));
        _ = CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (s, e) => WindowState = WindowState.Maximized));
        _ = CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (s, e) => WindowState = WindowState.Normal));
        _ = CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (s, e) => Close()));
        _ = CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

        _tempWindowState = WindowState;
        _tempWindowStyle = WindowStyle;
        _tempResizeMode = ResizeMode;

        SwitchIsFullScreen(_isFullScreen);
        SwitchShowNonClientArea(_showNonClientArea);
        SwitchShowArea(_leftArea, _showLeftArea);
        SwitchShowArea(_rightArea, _showRightArea);

        if (WindowState == WindowState.Maximized)
        {
            _tempNonClientAreaHeight -= 8;
        }

        if (SizeToContent != SizeToContent.WidthAndHeight)
        {
            return;
        }

        SizeToContent = SizeToContent.Height;
        _ = Dispatcher.BeginInvoke(new Action(() =>
        {
            SizeToContent = SizeToContent.WidthAndHeight;
        }));
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (SizeToContent == SizeToContent.WidthAndHeight)
        {
            InvalidateMeasure();
        }
    }

    private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        var mmi = (InteropValues.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(InteropValues.MINMAXINFO));
        var monitor = InteropMethods.MonitorFromWindow(hwnd, InteropValues.MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero && mmi != null)
        {
            InteropValues.APPBARDATA appBarData = default;
            var autoHide = InteropMethods.SHAppBarMessage(4, ref appBarData) != 0;
            if (autoHide)
            {
                var monitorInfo = default(InteropValues.MONITORINFO);
                monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(InteropValues.MONITORINFO));
                InteropMethods.GetMonitorInfo(monitor, ref monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top - 1);
            }
        }

        Marshal.StructureToPtr(mmi, lParam, true);
    }

    private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        switch (msg)
        {
            case InteropValues.WM_WINDOWPOSCHANGED:
                Padding = WindowState == WindowState.Maximized ? WindowHelper.WindowMaximizedPadding : _commonPadding;
                break;
            case InteropValues.WM_GETMINMAXINFO:
                WmGetMinMaxInfo(hwnd, lparam);
                Padding = WindowState == WindowState.Maximized ? WindowHelper.WindowMaximizedPadding : _commonPadding;
                break;
            case InteropValues.WM_NCHITTEST:
                // for fixing #886
                // https://developercommunity.visualstudio.com/t/overflow-exception-in-windowchrome/167357
                try
                {
                    _ = lparam.ToInt32();
                }
                catch (OverflowException)
                {
                    handled = true;
                }

                break;
        }

        return IntPtr.Zero;
    }

    private static void OnShowNonClientAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        BGWindow ctl = (BGWindow)d;
        ctl.SwitchShowNonClientArea((bool)e.NewValue);
    }

    private void SwitchShowNonClientArea(bool showNonClientArea)
    {
        if (_nonClientArea == null)
        {
            _showNonClientArea = showNonClientArea;
            return;
        }

        if (showNonClientArea)
        {
            if (IsFullScreen)
            {
                _nonClientArea.Show(false);
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight = 0;
            }
            else
            {
                _nonClientArea.Show(true);
                NonClientAreaHeight = _tempNonClientAreaHeight;
            }
        }
        else
        {
            _nonClientArea.Show(false);
            _tempNonClientAreaHeight = NonClientAreaHeight;
            NonClientAreaHeight = 0;
        }
    }

    private static void OnIsFullScreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BGWindow)d;
        ctl.SwitchIsFullScreen((bool)e.NewValue);
    }

    private void SwitchIsFullScreen(bool isFullScreen)
    {
        if (_nonClientArea == null)
        {
            _isFullScreen = isFullScreen;
            return;
        }

        if (isFullScreen)
        {
            _nonClientArea.Show(false);
            _tempNonClientAreaHeight = NonClientAreaHeight;
            NonClientAreaHeight = 0;

            _tempWindowState = WindowState;
            _tempWindowStyle = WindowStyle;
            _tempResizeMode = ResizeMode;
            WindowStyle = WindowStyle.None;
            //下面三行不能改变，就是故意的
            WindowState = WindowState.Maximized;
            WindowState = WindowState.Minimized;
            WindowState = WindowState.Maximized;
        }
        else
        {
            if (ShowNonClientArea)
            {
                _nonClientArea.Show(true);
                NonClientAreaHeight = _tempNonClientAreaHeight;
            }
            else
            {
                _nonClientArea.Show(false);
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight = 0;
            }

            WindowState = _tempWindowState;
            WindowStyle = _tempWindowStyle;
            ResizeMode = _tempResizeMode;
        }
    }

    private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
    {
        var point = WindowState == WindowState.Maximized
            ? new Point(0, NonClientAreaHeight)
            : new Point(Left, Top + NonClientAreaHeight);
        SystemCommands.ShowSystemMenu(this, point);
    }

    private static void OnIsShowLeftAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        BGWindow window = (BGWindow)d;
        window._showLeftArea = (bool)e.NewValue;
        window.SwitchShowArea(window._leftArea, (bool)e.NewValue);
    }

    private static void OnIsShowRightAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        BGWindow window = (BGWindow)d;
        window._showRightArea = (bool)e.NewValue;
        window.SwitchShowArea(window._rightArea, (bool)e.NewValue);
    }

    private void SwitchShowArea(UIElement? _nonClientArea, bool isShowArea)
    {
        if (_nonClientArea == null)
        {
            return;
        }

        if (isShowArea)
        {
            _nonClientArea.Visibility = Visibility.Visible;

        }
        else
        {
            _nonClientArea.Visibility = Visibility.Collapsed;
        }
    }
}
