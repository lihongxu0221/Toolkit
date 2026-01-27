namespace BgCommon.Prism.Wpf.Modules.Logging.Views;

/// <summary>
/// LoggerDialogWindow.xaml 的交互逻辑.
/// </summary>
public partial class LoggerDialogWindow : HandyControl.Controls.Window, IDialogWindow
{
    public IDialogResult Result { get; set; }

    public LoggerDialogWindow()
    {
        InitializeComponent();
    }
}