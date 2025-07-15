using BgCommon.Prism.Wpf.Common.ViewModels;
using BgCommon.Prism.Wpf.DependencyInjection;

namespace BgCommon.Prism.Wpf.Common.Views;

/// <summary>
/// ErrorView.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Navigation, typeof(ErrorViewModel))]
public partial class ErrorView : UserControl
{
    public ErrorView()
    {
        InitializeComponent();
    }
}