namespace BgControls.Controls;

/// <summary>
/// 标题栏
/// </summary>
[TemplatePart(Name = ElementTitle, Type = typeof(UIElement))]
[TemplatePart(Name = ElementLeftArea, Type = typeof(UIElement))]
[TemplatePart(Name = ElementRightArea, Type = typeof(UIElement))]
public class Banner : ContentControl
{
    private const string ElementTitle = "PART_Title";
    private const string ElementLeftArea = "PART_LeftArea";
    private const string ElementRightArea = "PART_RightArea";

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(UIElement), typeof(Banner), new FrameworkPropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty LeftAreaProperty = DependencyProperty.Register(nameof(LeftArea), typeof(UIElement), typeof(Banner), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty RightAreaProperty = DependencyProperty.Register(nameof(RightArea), typeof(UIElement), typeof(Banner), new PropertyMetadata(default(UIElement)));
    public static readonly DependencyProperty IsShowLeftAreaProperty = DependencyProperty.Register(nameof(IsShowLeftArea), typeof(bool), typeof(Banner), new PropertyMetadata(true, OnIsShowLeftAreaChanged));
    public static readonly DependencyProperty IsShowRightAreaProperty = DependencyProperty.Register(nameof(IsShowRightArea), typeof(bool), typeof(Banner), new PropertyMetadata(true, OnIsShowRightAreaChanged));

    private UIElement? _title;
    private UIElement? _leftArea;
    private UIElement? _rightArea;
    private bool _showLeftArea = true;
    private bool _showRightArea = true;

    public UIElement Title
    {
        get { return (UIElement)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
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

    public bool IsShowLeftArea
    {
        get => (bool)GetValue(IsShowLeftAreaProperty);
        set => SetValue(IsShowLeftAreaProperty, value);
    }

    public bool IsShowRightArea
    {
        get => (bool)GetValue(IsShowLeftAreaProperty);
        set => SetValue(IsShowLeftAreaProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _title = GetTemplateChild(ElementTitle) as UIElement;
        _leftArea = GetTemplateChild(ElementLeftArea) as UIElement;
        _rightArea = GetTemplateChild(ElementRightArea) as UIElement;
        SwitchShowArea(_leftArea, _showLeftArea);
        SwitchShowArea(_rightArea, _showRightArea);
    }

    private static void OnIsShowLeftAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Banner banner = (Banner)d;
        banner._showLeftArea = (bool)e.NewValue;
        banner.SwitchShowArea(banner._leftArea, (bool)e.NewValue);
    }

    private static void OnIsShowRightAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Banner banner = (Banner)d;
        banner._showRightArea = (bool)e.NewValue;
        banner.SwitchShowArea(banner._rightArea, (bool)e.NewValue);
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