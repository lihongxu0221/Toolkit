namespace BgCommon.Authority.Views;

/// <summary>
/// LoggerDialogWindow.xaml 的交互逻辑
/// </summary>
public partial class AuthorityDialogWindow : HandyControl.Controls.Window, IDialogWindow
{
    public IDialogResult Result { get; set; }

    public AuthorityDialogWindow()
    {
        InitializeComponent();
    }
}