namespace BgControls.Controls;

public static class PasswordHelper
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordHelper), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, OnAttachPropertyChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));

    public static string GetPassword(DependencyObject obj)
    {
        return (string)obj.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject obj, string value)
    {
        obj.SetValue(PasswordProperty, value);
    }

    public static void SetAttach(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachProperty, value);
    }

    public static bool GetAttach(DependencyObject dp)
    {
        return (bool)dp.GetValue(AttachProperty);
    }

    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
        dp.SetValue(IsUpdatingProperty, value);
    }

    private static bool GetIsUpdating(DependencyObject dp)
    {
        return (bool)dp.GetValue(IsUpdatingProperty);
    }

    /// <summary>
    /// 可能有兼容性问题，需要测试.
    /// </summary>
    /// <param name="passwordBox">密码框</param>
    private static void MoveCursorToEnd(PasswordBox passwordBox)
    {
        // 使用反射调用 PasswordBox 的 Select 方法
        Type type = passwordBox.GetType();
        MethodInfo? method = type.GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
        _ = method?.Invoke(passwordBox, new object[] { passwordBox.Password.Length, 0 });
    }

    private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox)
        {
            // 防止循环更新
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if (!GetIsUpdating(passwordBox))
            {
                Debug.WriteLine($"Password updated: {e.NewValue}");
                passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;
                MoveCursorToEnd(passwordBox); // 将光标移动到末尾
            }

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void OnAttachPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox)
        {

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
            else
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            }
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
