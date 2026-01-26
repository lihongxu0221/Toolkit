namespace BgCommon.Prism.Wpf.Controls.Windows;

/// <summary>
/// LoginHostWindow.xaml 的交互逻辑
/// </summary>
public partial class LoginHostWindow : HandyControl.Controls.Window, IDialogWindow
{
    public IDialogResult Result { get; set; }

    public LoginHostWindow()
    {
        InitializeComponent();
    }
}