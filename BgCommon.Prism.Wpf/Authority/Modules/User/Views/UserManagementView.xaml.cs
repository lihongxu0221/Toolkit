using BgCommon.Prism.Wpf.Authority.Modules.User.ViewModels;
using BgCommon.Prism.Wpf.DependencyInjection;

namespace BgCommon.Prism.Wpf.Authority.Modules.User.Views;

/// <summary>
/// UserManagementView.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Navigation, typeof(UserManagementViewModel))]
public partial class UserManagementView : UserControl, IRegistration
{
    public UserManagementView()
    {
        InitializeComponent();
    }
}