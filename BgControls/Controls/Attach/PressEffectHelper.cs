namespace BgControls.Controls;

///// <summary>
///// 提供按压缩放动画效果的辅助类.
///// </summary>
//public static class PressEffectHelper
//{
//    /// <summary>
//    /// 是否启用按压缩放效果的附加属性.
//    /// </summary>
//    public static readonly DependencyProperty IsEnabledProperty =
//        DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PressEffectHelper), new PropertyMetadata(false, OnIsEnabledChanged));

//    /// <summary>
//    /// 获取是否启用按压缩放效果.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <returns>是否启用.</returns>
//    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

//    /// <summary>
//    /// 设置是否启用按压缩放效果.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <param name="value">是否启用.</param>
//    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

//    /// <summary>
//    /// 按压时缩放比例的附加属性.
//    /// </summary>
//    public static readonly DependencyProperty ScaleProperty =
//        DependencyProperty.RegisterAttached("Scale", typeof(double), typeof(PressEffectHelper), new PropertyMetadata(0.95));

//    /// <summary>
//    /// 获取按压时的缩放比例.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <returns>缩放比例.</returns>
//    public static double GetScale(DependencyObject obj) => (double)obj.GetValue(ScaleProperty);

//    /// <summary>
//    /// 设置按压时的缩放比例.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <param name="value">缩放比例.</param>
//    public static void SetScale(DependencyObject obj, double value) => obj.SetValue(ScaleProperty, value);

//    /// <summary>
//    /// 私有附加属性,用于存储我们为按压效果专门创建的ScaleTransform实例.
//    /// 这可以防止我们干扰控件上可能存在的其他变换.
//    /// </summary>
//    private static readonly DependencyProperty PressEffectScaleTransformProperty =
//        DependencyProperty.RegisterAttached("PressEffectScaleTransform", typeof(ScaleTransform), typeof(PressEffectHelper), new PropertyMetadata(null));

//    /// <summary>
//    /// 获取按压效果的ScaleTransform实例.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <returns>ScaleTransform实例.</returns>
//    private static ScaleTransform? GetPressEffectScaleTransform(DependencyObject obj) => (ScaleTransform?)obj.GetValue(PressEffectScaleTransformProperty);

//    /// <summary>
//    /// 设置按压效果的ScaleTransform实例.
//    /// </summary>
//    /// <param name="obj">目标依赖对象.</param>
//    /// <param name="value">ScaleTransform实例.</param>
//    private static void SetPressEffectScaleTransform(DependencyObject obj, ScaleTransform? value) => obj.SetValue(PressEffectScaleTransformProperty, value);

//    /// <summary>
//    /// 当IsEnabled属性变化时的回调.
//    /// </summary>
//    /// <param name="d">目标依赖对象.</param>
//    /// <param name="e">属性变更参数.</param>
//    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        if (d is not UIElement element)
//        {
//            return;
//        }

//        if ((bool)e.NewValue)
//        {
//            // 挂载事件.
//            element.PreviewMouseDown += OnPreviewMouseDown;
//            element.PreviewMouseUp += OnPreviewMouseUp;
//            element.MouseLeave += OnMouseLeave;
//            element.LostMouseCapture += OnLostMouseCapture;

//            // 确保变换已设置.
//            EnsureScaleTransform(element);
//        }
//        else
//        {
//            // 卸载事件.
//            element.PreviewMouseDown -= OnPreviewMouseDown;
//            element.PreviewMouseUp -= OnPreviewMouseUp;
//            element.MouseLeave -= OnMouseLeave;
//            element.LostMouseCapture -= OnLostMouseCapture;

//            // 如果需要在禁用时恢复动画,可以调用一次.
//            AnimateScale(element, 1.0);
//        }
//    }

//    /// <summary>
//    /// 确保元素上存在一个我们可以控制的ScaleTransform.
//    /// </summary>
//    /// <param name="element">目标UI元素.</param>
//    private static void EnsureScaleTransform(UIElement element)
//    {
//        // 如果我们已经创建并附加了变换,则无需任何操作.
//        if (GetPressEffectScaleTransform(element) != null)
//        {
//            return;
//        }

//        ScaleTransform effectTransform = new ScaleTransform(1.0, 1.0);

//        // 将我们创建的变换存储在私有附加属性中,以便将来查找.
//        SetPressEffectScaleTransform(element, effectTransform);

//        // 获取元素当前的RenderTransform.
//        Transform originalTransform = element.RenderTransform;

//        if (originalTransform == null || originalTransform == Transform.Identity)
//        {
//            // 如果没有变换,直接使用我们的变换.
//            element.RenderTransform = effectTransform;
//        }
//        else if (originalTransform is TransformGroup group)
//        {
//            // 如果已经是变换组,将我们的变换添加到子项中.
//            group.Children.Add(effectTransform);
//        }
//        else
//        {
//            // 如果是单个其他变换,创建一个新的变换组.
//            var newGroup = new TransformGroup();
//            newGroup.Children.Add(originalTransform); // 先添加旧的.
//            newGroup.Children.Add(effectTransform);   // 再添加我们的.
//            element.RenderTransform = newGroup;
//        }

//        // 确保变换中心是控件中心.
//        element.RenderTransformOrigin = new Point(0.5, 0.5);
//    }

//    /// <summary>
//    /// 鼠标按下事件处理,触发缩放动画.
//    /// </summary>
//    /// <param name="sender">事件源.</param>
//    /// <param name="e">鼠标事件参数.</param>
//    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
//    {
//        if (sender is UIElement element)
//        {
//            // 如果控件不是ButtonBase，我们才需要自己管理鼠标捕获，
//            // 以便在鼠标移出后仍能正确恢复。
//            // ButtonBase会自己处理捕获，我们不应干预，否则会破坏Click事件。
//            if (element is not ButtonBase)
//            {
//                _ = element.CaptureMouse();
//            }

//            AnimateScale(element, GetScale(element));
//        }
//    }

//    /// <summary>
//    /// 鼠标弹起事件处理,恢复缩放动画.
//    /// </summary>
//    /// <param name="sender">事件源.</param>
//    /// <param name="e">鼠标事件参数.</param>
//    private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
//    {
//        // 确保只操作我们自己捕获的非按钮控件
//        if (sender is UIElement element && element.IsMouseCaptured)
//        {
//            if (element is not ButtonBase && element.IsMouseCaptured)
//            {
//                element.ReleaseMouseCapture(); // 这会自动触发 OnLostMouseCapture
//            }
//            else
//            {
//                // 如果是ButtonBase，直接恢复缩放动画。
//                // ButtonBase会自动处理鼠标捕获，我们不需要干预。
//                AnimateScale(element, 1.0);
//            }
//        }
//    }

//    /// <summary>
//    /// 鼠标离开事件处理,恢复缩放动画.
//    /// </summary>
//    /// <param name="sender">事件源.</param>
//    /// <param name="e">鼠标事件参数.</param>
//    private static void OnMouseLeave(object sender, MouseEventArgs e)
//    {
//        if (sender is UIElement element && element.IsMouseCaptured)
//        {
//            if (element is not ButtonBase && element.IsMouseCaptured)
//            {
//                element.ReleaseMouseCapture(); // 这会自动触发 OnLostMouseCapture
//            }
//            else
//            {
//                // 如果是ButtonBase，直接恢复缩放动画。
//                // ButtonBase会自动处理鼠标捕获，我们不需要干预。
//                AnimateScale(element, 1.0);
//            }
//        }
//    }

//    /// <summary>
//    /// 丢失鼠标捕获事件处理,恢复缩放动画.
//    /// </summary>
//    /// <param name="sender">事件源.</param>
//    /// <param name="e">鼠标事件参数.</param>
//    private static void OnLostMouseCapture(object sender, MouseEventArgs e)
//    {
//        if (sender is UIElement element)
//        {
//            AnimateScale(element, 1.0);
//        }
//    }

//    /// <summary>
//    /// 执行缩放动画.
//    /// </summary>
//    /// <param name="element">目标UI元素.</param>
//    /// <param name="toScale">目标缩放比例.</param>
//    private static void AnimateScale(UIElement element, double toScale)
//    {
//        // 从我们的私有附加属性中获取要进行动画处理的ScaleTransform.
//        ScaleTransform? scaleTransform = GetPressEffectScaleTransform(element);

//        // 如果由于某种原因找不到(理论上不应该发生),则退出.
//        if (scaleTransform == null)
//        {
//            // 作为备用方案,可以重新调用EnsureScaleTransform,但正常情况下不需要.
//            EnsureScaleTransform(element);
//            scaleTransform = GetPressEffectScaleTransform(element);
//            if (scaleTransform == null)
//            {
//                return;
//            }
//        }

//        var storyboard = new Storyboard();
//        var duration = new Duration(TimeSpan.FromMilliseconds(100));

//        // 创建动画并直接以我们存储的ScaleTransform对象为目标.
//        // 这样做比使用字符串路径 "RenderTransform.ScaleX" 更健壮.
//        var scaleXAnimation = new DoubleAnimation(toScale, duration) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
//        Storyboard.SetTarget(scaleXAnimation, scaleTransform);
//        Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
//        storyboard.Children.Add(scaleXAnimation);

//        var scaleYAnimation = new DoubleAnimation(toScale, duration) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
//        Storyboard.SetTarget(scaleYAnimation, scaleTransform);
//        Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));
//        storyboard.Children.Add(scaleYAnimation);

//        storyboard.Begin();
//    }
//}

public static class PressEffectHelper
{
    #region IsEnabled Attached Property
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(PressEffectHelper),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);
    #endregion

    #region PressedScale Attached Property
    public static readonly DependencyProperty PressedScaleProperty =
        DependencyProperty.RegisterAttached(
            "PressedScale", typeof(double), typeof(PressEffectHelper),
            new PropertyMetadata(0.95));

    public static double GetPressedScale(DependencyObject obj) => (double)obj.GetValue(PressedScaleProperty);
    public static void SetPressedScale(DependencyObject obj, double value) => obj.SetValue(PressedScaleProperty, value);
    #endregion

    #region (Private) PressEffectScaleTransform Attached Property
    private static readonly DependencyProperty PressEffectScaleTransformProperty =
        DependencyProperty.RegisterAttached(
            "PressEffectScaleTransform", typeof(ScaleTransform), typeof(PressEffectHelper));

    private static ScaleTransform? GetPressEffectScaleTransform(DependencyObject obj) => (ScaleTransform?)obj.GetValue(PressEffectScaleTransformProperty);
    private static void SetPressEffectScaleTransform(DependencyObject obj, ScaleTransform? value) => obj.SetValue(PressEffectScaleTransformProperty, value);
    #endregion

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element) return;

        if ((bool)e.NewValue)
        {
            element.PreviewMouseDown += OnPreviewMouseDown;
            element.PreviewMouseUp += OnPreviewMouseUp;
            element.MouseLeave += OnMouseLeave;
            element.LostMouseCapture += OnLostMouseCapture;

            EnsureScaleTransform(element);
        }
        else
        {
            element.PreviewMouseDown -= OnPreviewMouseDown;
            element.PreviewMouseUp -= OnPreviewMouseUp;
            element.MouseLeave -= OnMouseLeave;
            element.LostMouseCapture -= OnLostMouseCapture;

            AnimateScale(element, 1.0);
        }
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is UIElement element)
        {
            // 只为非ButtonBase控件捕获鼠标，以避免破坏Click事件
            if (element is not ButtonBase)
            {
                element.CaptureMouse();
            }
            AnimateScale(element, GetPressedScale(element));
        }
    }

    private static void OnLostMouseCapture(object sender, MouseEventArgs e)
    {
        if (sender is UIElement element)
        {
            AnimateScale(element, 1.0);
        }
    }

    private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is UIElement element && element is not ButtonBase && element.IsMouseCaptured)
        {
            element.ReleaseMouseCapture();
        }
    }

    private static void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is UIElement element && element is not ButtonBase && element.IsMouseCaptured)
        {
            element.ReleaseMouseCapture();
        }
    }

    private static void EnsureScaleTransform(UIElement element)
    {
        if (GetPressEffectScaleTransform(element) != null) return;

        var effectTransform = new ScaleTransform(1.0, 1.0);
        SetPressEffectScaleTransform(element, effectTransform);

        // 必须设置变换中心为中心点
        element.RenderTransformOrigin = new Point(0.5, 0.5);

        var originalTransform = element.RenderTransform;

        if (originalTransform == null || originalTransform == Transform.Identity)
        {
            element.RenderTransform = effectTransform;
        }
        else if (originalTransform is TransformGroup group)
        {
            group.Children.Add(effectTransform);
        }
        else
        {
            var newGroup = new TransformGroup();
            newGroup.Children.Add(originalTransform);
            newGroup.Children.Add(effectTransform);
            element.RenderTransform = newGroup;
        }
    }

    private static void AnimateScale(UIElement element, double toScale)
    {
        var scaleTransform = GetPressEffectScaleTransform(element);
        if (scaleTransform == null)
        {
            // 如果变换丢失，尝试重新确保它存在
            EnsureScaleTransform(element);
            scaleTransform = GetPressEffectScaleTransform(element);
            if (scaleTransform == null) return; // 如果仍然失败，则退出
        }

        var storyboard = new Storyboard();
        var duration = new Duration(TimeSpan.FromMilliseconds(120));
        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        var animX = new DoubleAnimation(toScale, duration) { EasingFunction = easing };
        Storyboard.SetTarget(animX, scaleTransform);
        Storyboard.SetTargetProperty(animX, new PropertyPath(ScaleTransform.ScaleXProperty));
        storyboard.Children.Add(animX);

        var animY = new DoubleAnimation(toScale, duration) { EasingFunction = easing };
        Storyboard.SetTarget(animY, scaleTransform);
        Storyboard.SetTargetProperty(animY, new PropertyPath(ScaleTransform.ScaleYProperty));
        storyboard.Children.Add(animY);

        storyboard.Begin();
    }
}
