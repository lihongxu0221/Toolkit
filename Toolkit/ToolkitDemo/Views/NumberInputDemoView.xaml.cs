using BgCommon.Prism.Wpf.DependencyInjection;
using ToolkitDemo.Models;
using ToolkitDemo.ViewModels;

namespace ToolkitDemo.Views;

/// <summary>
/// NumberInputDemoView.xaml 的交互逻辑.
/// </summary>
[Registration(Registration.Navigation, typeof(NumberInputDemoViewModel))]
public partial class NumberInputDemoView : UserControl, IRegistration, IRegionMemberLifetime
{
    public NumberInputDemoView()
    {
        InitializeComponent();
        this.DataContext = new NumberInputDemoViewModel();
        this.Unloaded += NumberInputDemoView_Unloaded;
    }

    private void NumberInputDemoView_Unloaded(object sender, RoutedEventArgs e)
    {
        // this.DataContext = null;
        Debug.WriteLine("NumberInputDemoView_Unloaded");
    }

    /// <inheritdoc/>
    public bool KeepAlive => false;

    private void NumberInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        Debug.WriteLine($"NumberInput value is changed, New Value: {e.NewValue}");
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        TestDemoView testDemoView = new TestDemoView();
        testDemoView.Show();
    }
}