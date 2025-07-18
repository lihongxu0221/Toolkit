using BgCommon.Prism.Wpf.DependencyInjection;
using ToolkitDemo.ViewModels;

namespace ToolkitDemo.Views;

/// <summary>
/// ModuleHostView.xaml 的交互逻辑
/// </summary>
[Registration(Registration.Navigation, typeof(ModuleHostViewModel))]
public partial class ModuleHostView : UserControl
{
    public ModuleHostView()
    {
        InitializeComponent();
    }
}