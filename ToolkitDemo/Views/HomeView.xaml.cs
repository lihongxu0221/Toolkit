using BgCommon.Prism.Wpf.DependencyInjection;
using ToolkitDemo.ViewModels;

namespace ToolkitDemo.Views;

/// <summary>
/// HomeView.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Navigation, typeof(HomeViewModel))]
public partial class HomeView : UserControl, IRegistration
{
    public HomeView()
    {
        InitializeComponent();
    }
}