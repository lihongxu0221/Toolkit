using ToolkitDemo.Views;

namespace ToolkitDemo;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
public partial class App : Application
{
    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        int i = 1;
        if (i == 1)
        {
            var bootstrapper = new Bootstrapper(this);
            bootstrapper.Run();
            return;
        }

        if (i == 2)
        {
            this.MainWindow = new TestDemoView();
            this.MainWindow.Show();
        }
    }
}