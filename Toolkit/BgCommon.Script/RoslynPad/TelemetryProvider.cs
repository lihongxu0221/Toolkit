using RoslynPad.UI;

namespace RoslynPad;

internal class TelemetryProvider : TelemetryProviderBase
{
    public override void Initialize(string version, IApplicationSettings settings)
    {
        base.Initialize(version, settings);

        Application.Current.DispatcherUnhandledException += OnUnhandledDispatcherException;
    }

    private void OnUnhandledDispatcherException(object? sender, DispatcherUnhandledExceptionEventArgs args)
    {
        HandleException(args.Exception);
        args.Handled = true;
    }
}
