namespace BgControls.Controls;

/// <summary>
/// 一个静态服务类，用于管理一个全局共享的数字小键盘 Popup。
/// 这可以防止同时创建多个小键盘实例，并简化了显示和隐藏的逻辑。
/// </summary>
internal static class NumericInputPopupService
{
    private static Popup? sharedPopup;
    private static NumericKeypad? sharedKeypad;
    private static NumberInput? currentOwner; // 当前正在使用小键盘的 NumberInput 控件
    private static Window? ownerWindow;

    /// <summary>
    /// 确保共享的 Popup 和 Keypad 实例已创建。
    /// </summary>
    private static void EnsureSharedPopup()
    {
        if (sharedPopup != null)
        {
            return;
        }

        sharedKeypad = new NumericKeypad();
        sharedPopup = new Popup
        {
            AllowsTransparency = true,
            Placement = PlacementMode.Bottom,

            // 确保 Popup 在点击外部时不会自动关闭，自行管理关闭逻辑
            StaysOpen = true,
            Child = sharedKeypad,
            Width = 400, // 或根据需要设置
            MinWidth = 400
        };

        // 订阅小键盘的值确认事件，以便将结果传回给当前的 NumberInput 控件
        sharedKeypad.ValueConfirmed += OnSharedKeypadValueConfirmed;

        // 当 Popup 关闭时，我们确保解绑全局事件
        sharedPopup.Closed += OnSharedPopupClosed;
    }

    /// <summary>
    /// 检查当前是否有小键盘 Popup 打开，并且鼠标是否在 Popup 上。
    /// </summary>
    public static bool IsMouseOverPopup()
    {
        return sharedPopup?.IsMouseOver ?? false;
    }

    /// <summary>
    /// 为指定的 NumberInput 控件显示小键盘。
    /// </summary>
    /// <param name="owner">请求显示小键盘的 NumberInput 控件。</param>
    public static void ShowPopup(NumberInput owner)
    {
        if (owner == null)
        {
            return;
        }

        // 如果点击的已经是当前所有者且 Popup 已打开，则什么都不做
        if (currentOwner == owner && sharedPopup?.IsOpen == true)
        {
            return;
        }

        EnsureSharedPopup();

        // 如果之前有其他所有者，先确保旧的 Popup 已关闭
        if (sharedPopup!.IsOpen)
        {
            // IsOpen = false 会触发 Closed 事件，从而调用 DetachFromWindow
            sharedPopup.IsOpen = false;
        }

        currentOwner = owner;
        sharedKeypad!.SetInitialValue(owner.Value, owner.Maximum, owner.Minimum, owner.DecimalPlaces);
        sharedPopup.PlacementTarget = owner;
        sharedPopup.IsOpen = true;

        // 当 Popup 打开时，附加到父窗口以监控外部点击
        AttachToWindow(owner);
    }

    /// <summary>
    /// 当一个控件不再需要小键盘时调用（例如，它失去了焦点）。
    /// </summary>
    public static void HidePopup()
    {
        if (sharedPopup != null)
        {
            sharedPopup.IsOpen = false;
        }
    }

    /// <summary>
    /// 当一个控件不再需要小键盘时调用（例如，它失去了焦点）。
    /// </summary>
    public static void HidePopupIfOpen()
    {
        if (sharedPopup != null && sharedPopup.IsOpen)
        {
            sharedPopup.IsOpen = false;
        }
    }

    /// <summary>
    /// 当一个控件不再需要小键盘时调用（例如，它失去了焦点）。
    /// </summary>
    /// <param name="owner">请求关闭的控件。</param>
    public static void HidePopup(NumberInput owner)
    {
        // 只有当请求关闭的 owner 是当前的所有者时，才关闭 popup
        // 这可以防止一个控件错误地关闭了另一个控件的 popup
        if (currentOwner == owner && sharedPopup != null)
        {
            sharedPopup.IsOpen = false;
        }
    }

    /// <summary>
    /// 当共享小键盘确认一个值时触发。
    /// </summary>
    private static void OnSharedKeypadValueConfirmed(object? sender, double confirmedValue)
    {
        // 将确认的值设置回当前的 "所有者" 控件
        currentOwner?.OnKeypadValueConfirmed(confirmedValue);

        // 关闭小键盘
        HidePopupIfOpen();
    }

    /// <summary>
    /// 当共享 Popup 关闭时触发（例如，用户点击了外部）。
    /// </summary>
    private static void OnSharedPopupClosed(object? sender, EventArgs e)
    {
        // 当 Popup 关闭时（无论任何原因），通知当前所有者，并清除引用
        if (ownerWindow != null)
        {
            ownerWindow.PreviewMouseDown -= OnWindowPreviewMouseDown;
            ownerWindow = null;
        }

        // 清理所有者，因为 Popup 已经关闭    
        // 通知当前的 NumberInput 控件 Popup 已关闭，但不要在这里调用 CommitTextInput 或管理焦点。
        // TextBox_LostFocus 事件会正确处理这些。
        currentOwner?.OnPopupClosed();
        currentOwner = null;
    }

    private static void AttachToWindow(FrameworkElement owner)
    {
        ownerWindow = Window.GetWindow(owner);
        if (ownerWindow != null)
        {
            // 使用 PreviewMouseDown 是因为它在所有控件的标准 Down 事件之前触发
            ownerWindow.PreviewMouseDown += OnWindowPreviewMouseDown;
        }
    }

    /// <summary>
    /// 这是我们自定义的“点击外部关闭”逻辑。
    /// </summary>
    private static void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sharedPopup == null || !sharedPopup.IsOpen)
        {
            return;
        }

        // 如果点击发生在 Popup 内部，则不执行任何操作
        if (sharedPopup?.IsMouseOver == true)
        {
            return;
        }

        // 如果点击发生在当前所有者控件的内部，也不执行任何操作
        if (currentOwner?.IsMouseOver == true)
        {
            // 如果用户点击已获焦点的输入框，我们不希望关闭小键盘
            return;
        }

        // 如果点击发生在上述两者之外，则关闭 Popup
        HidePopupIfOpen();
    }
}