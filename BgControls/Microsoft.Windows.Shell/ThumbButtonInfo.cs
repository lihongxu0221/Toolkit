namespace Microsoft.Windows.Shell;

[DefaultEvent("Click")]
public sealed class ThumbButtonInfo : Freezable, ICommandSource
{
    protected override Freezable CreateInstanceCore()
    {
        return new ThumbButtonInfo();
    }

    public Visibility Visibility
    {
        get
        {
            return (Visibility) GetValue(ThumbButtonInfo.VisibilityProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.VisibilityProperty, value);
        }
    }

    public bool DismissWhenClicked
    {
        get
        {
            return (bool) GetValue(ThumbButtonInfo.DismissWhenClickedProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.DismissWhenClickedProperty, value);
        }
    }

    public ImageSource ImageSource
    {
        get
        {
            return (ImageSource) GetValue(ThumbButtonInfo.ImageSourceProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.ImageSourceProperty, value);
        }
    }

    public bool IsBackgroundVisible
    {
        get
        {
            return (bool) GetValue(ThumbButtonInfo.IsBackgroundVisibleProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.IsBackgroundVisibleProperty, value);
        }
    }

    public string Description
    {
        get
        {
            return (string) GetValue(ThumbButtonInfo.DescriptionProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.DescriptionProperty, value);
        }
    }

    private static object _CoerceDescription(DependencyObject d, object value)
    {
        string text = (string) value;
        if (text != null && text.Length >= 260)
        {
            text = text.Substring(0, 259);
        }

        return text;
    }

    private object _CoerceIsEnabledValue(object value)
    {
        bool flag = (bool) value;
        return flag && this._CanExecute;
    }

    public bool IsEnabled
    {
        get
        {
            return (bool) GetValue(ThumbButtonInfo.IsEnabledProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.IsEnabledProperty, value);
        }
    }

    public bool IsInteractive
    {
        get
        {
            return (bool) GetValue(ThumbButtonInfo.IsInteractiveProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.IsInteractiveProperty, value);
        }
    }

    private void _OnCommandChanged(DependencyPropertyChangedEventArgs e)
    {
        ICommand command = (ICommand) e.OldValue;
        ICommand command2 = (ICommand) e.NewValue;
        if (command == command2)
        {
            return;
        }

        if (command != null)
        {
            this._UnhookCommand(command);
        }

        if (command2 != null)
        {
            this._HookCommand(command2);
        }
    }

    private bool _CanExecute
    {
        get
        {
            return (bool) GetValue(ThumbButtonInfo._CanExecuteProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo._CanExecuteProperty, value);
        }
    }

    public event EventHandler? Click;

    internal void InvokeClick()
    {
        if (Click != null)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        this._InvokeCommand();
    }

    private void _InvokeCommand()
    {
        if (Command != null)
        {
            object commandParameter = CommandParameter;
            IInputElement commandTarget = CommandTarget;
            RoutedCommand? routedCommand = Command as RoutedCommand;
            if (routedCommand != null)
            {
                if (routedCommand.CanExecute(commandParameter, commandTarget))
                {
                    routedCommand.Execute(commandParameter, commandTarget);
                    return;
                }
            }
            else if (Command.CanExecute(commandParameter))
            {
                Command.Execute(commandParameter);
            }
        }
    }

    private void _UnhookCommand(ICommand command)
    {
        command.CanExecuteChanged -= this._commandEvent;
        this._commandEvent = null;
        this._UpdateCanExecute();
    }

    private void _HookCommand(ICommand command)
    {
        this._commandEvent = delegate (object sender, EventArgs e)
        {
            this._UpdateCanExecute();
        };
        command.CanExecuteChanged += this._commandEvent;
        this._UpdateCanExecute();
    }

    private void _UpdateCanExecute()
    {
        if (this.Command == null)
        {
            this._CanExecute = true;
            return;
        }

        object commandParameter = this.CommandParameter;
        IInputElement commandTarget = this.CommandTarget;
        RoutedCommand? routedCommand = this.Command as RoutedCommand;
        if (routedCommand != null)
        {
            this._CanExecute = routedCommand.CanExecute(commandParameter, commandTarget);
            return;
        }

        this._CanExecute = this.Command.CanExecute(commandParameter);
    }

    public ICommand Command
    {
        get
        {
            return (ICommand)  GetValue(ThumbButtonInfo.CommandProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.CommandProperty, value);
        }
    }

    public object CommandParameter
    {
        get
        {
            return GetValue(ThumbButtonInfo.CommandParameterProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.CommandParameterProperty, value);
        }
    }

    public IInputElement CommandTarget
    {
        get
        {
            return (IInputElement) GetValue(ThumbButtonInfo.CommandTargetProperty);
        }

        set
        {
            SetValue(ThumbButtonInfo.CommandTargetProperty, value);
        }
    }

    private EventHandler? _commandEvent;

    public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ThumbButtonInfo), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty DismissWhenClickedProperty = DependencyProperty.Register("DismissWhenClicked", typeof(bool), typeof(ThumbButtonInfo), new PropertyMetadata(false));

    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ThumbButtonInfo), new PropertyMetadata(null));

    public static readonly DependencyProperty IsBackgroundVisibleProperty = DependencyProperty.Register("IsBackgroundVisible", typeof(bool), typeof(ThumbButtonInfo), new PropertyMetadata(true));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ThumbButtonInfo), new PropertyMetadata(string.Empty, null, new CoerceValueCallback(ThumbButtonInfo._CoerceDescription)));

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ThumbButtonInfo), new PropertyMetadata(true, null, (DependencyObject d, object e) => ((ThumbButtonInfo) d)._CoerceIsEnabledValue(e)));

    public static readonly DependencyProperty IsInteractiveProperty = DependencyProperty.Register("IsInteractive", typeof(bool), typeof(ThumbButtonInfo), new PropertyMetadata(true));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ThumbButtonInfo), new PropertyMetadata(null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThumbButtonInfo) d)._OnCommandChanged(e);
    }));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ThumbButtonInfo), new PropertyMetadata(null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThumbButtonInfo) d)._UpdateCanExecute();
    }));

    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ThumbButtonInfo), new PropertyMetadata(null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThumbButtonInfo) d)._UpdateCanExecute();
    }));

    private static readonly DependencyProperty _CanExecuteProperty = DependencyProperty.Register("_CanExecute", typeof(bool), typeof(ThumbButtonInfo), new PropertyMetadata(true, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(ThumbButtonInfo.IsEnabledProperty);
    }));
}
