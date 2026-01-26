namespace BgCommon.Prism.Wpf.Modules.Logging.Views;

/// <summary>
/// LoggerDialogAutoSizeWindow.xaml 的交互逻辑.
/// </summary>
public partial class LoggerDialogAutoSizeWindow : HandyControl.Controls.Window, IDialogWindow
{
    public IDialogResult Result { get; set; }

    public LoggerDialogAutoSizeWindow()
    {
        InitializeComponent();
    }
}