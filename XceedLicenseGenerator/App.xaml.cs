using System.Configuration;
using System.Data;
using System.Windows;

namespace XceedLicenseGenerator;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
public partial class App : Application
{
    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 产品名 + 主版本 + 次版本
        string XceedKey = XceedKeyGenerator.GenerateKey("WTK", 5, 1, new DateTime(2045, 12, 31), 255);
        Licenser.LicenseKey = XceedKey;
    }
}