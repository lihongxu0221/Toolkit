namespace BgControls.Controls;

public class BackgroundSwitchElement
{
    public static readonly DependencyProperty MouseHoverBackgroundProperty = DependencyProperty.RegisterAttached(
        "MouseHoverBackground", typeof(Brush), typeof(BackgroundSwitchElement), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetMouseHoverBackground(DependencyObject element, Brush value) => element.SetValue(MouseHoverBackgroundProperty, value);

    public static Brush GetMouseHoverBackground(DependencyObject element) => (Brush) element.GetValue(MouseHoverBackgroundProperty);

    public static readonly DependencyProperty MouseDownBackgroundProperty = DependencyProperty.RegisterAttached(
        "MouseDownBackground", typeof(Brush), typeof(BackgroundSwitchElement), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetMouseDownBackground(DependencyObject element, Brush value) => element.SetValue(MouseDownBackgroundProperty, value);

    public static Brush GetMouseDownBackground(DependencyObject element) => (Brush) element.GetValue(MouseDownBackgroundProperty);

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
        "Background", typeof(Brush), typeof(BackgroundSwitchElement), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetBackground(DependencyObject element, Brush value) => element.SetValue(BackgroundProperty, value);

    public static Brush GetBackground(DependencyObject element) => (Brush)element.GetValue(BackgroundProperty);

}