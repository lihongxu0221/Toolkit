using AvalonDock;
using AvalonDock.Controls;
using AvalonDock.Layout.Serialization;
using BgCommon.Script.ViewModels;
using RoslynPad;
using RoslynPad.Themes;

namespace BgCommon.Script.Views;

/// <summary>
/// ScriptMainView.xaml 的交互逻辑.
/// </summary>
public partial class ScriptMainView : UserControl
{
    private ScriptMainViewModel viewModel;
    private ThemeDictionary? _themeDictionary;
    private bool isClosing;
    private bool isClosed;

    public ScriptMainView(ScriptMainViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.DataContextChanged += ScriptMainView_DataContextChanged;
        this.Loaded += this.ScriptMainView_Loaded;
        this.Unloaded += this.ScriptMainView_Unloaded;
        this.InitializeComponent();
    }

    private void OnViewModelThemeChanged(object? sender, EventArgs e)
    {
        Application app = Application.Current;
        if (_themeDictionary is not null)
        {
            _ = app.Resources.MergedDictionaries.Remove(_themeDictionary);
        }

        if (viewModel != null)
        {
            _themeDictionary = new ThemeDictionary(viewModel.Theme);
            app.Resources.MergedDictionaries.Add(_themeDictionary);

            SetDockTheme(viewModel.Theme);
            this.UseImmersiveDarkMode(viewModel.ThemeType);
        }
    }

    private void SetDockTheme(Theme theme)
    {
        if (DockingManager is null || viewModel == null)
        {
            return;
        }

        DockingManager.Theme = viewModel.ThemeType == ThemeType.Dark ?
            new AvalonDock.Themes.Vs2013DarkTheme() :
            new AvalonDock.Themes.Vs2013LightTheme();
        DockingManager.Resources.MergedDictionaries.Add(new DockThemeDictionary(theme));

        DockingManager.DocumentPaneControlStyle = new Style(
            typeof(LayoutDocumentPaneControl),
            DockingManager.DocumentPaneControlStyle)
        {
            Setters =
             {
                 new Setter(ItemsControl.ItemContainerStyleProperty, DockingManager.TryFindResource("DocumentPaneControlTabStyle")),
             },
        };
    }

    private void ScriptMainView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ScriptMainViewModel mainViewModel)
        {
            viewModel = mainViewModel;
            viewModel.ThemeChanged += OnViewModelThemeChanged;
            viewModel.InitializeTheme();
            DocumentsPane.ToggleAutoHide();
            SetDockTheme(viewModel.Theme);
        }
    }

    private async void ScriptMainView_Loaded(object? sender, RoutedEventArgs e)
    {
        this.Loaded -= ScriptMainView_Loaded;
        if (this.viewModel != null)
        {
            await viewModel.InitializeAsync().ConfigureAwait(false);

            _ = this.Dispatcher.BeginInvoke(
                () =>
            {
                LoadWindowLayout();
                LoadDockLayout();
                this.UseImmersiveDarkMode(viewModel.ThemeType);
            }, DispatcherPriority.Loaded);
        }
    }

    private void ScriptMainView_Unloaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= ScriptMainView_Loaded;
        this.Unloaded -= ScriptMainView_Unloaded;
    }

    // protected override async void OnClosing(CancelEventArgs e)
    // {
    //     base.OnClosing(e);
    //
    //     if (!_isClosing)
    //     {
    //         SaveDockLayout();
    //         SaveWindowLayout();
    //
    //         _isClosing = true;
    //         IsEnabled = false;
    //         e.Cancel = true;
    //
    //         try
    //         {
    //             await Task.Run(_viewModel.OnExitAsync).ConfigureAwait(true);
    //         }
    //         catch
    //         {
    //             // ignored
    //         }
    //
    //         _isClosed = true;
    //         var closeTask = Dispatcher.InvokeAsync(Close);
    //     }
    //     else
    //     {
    //         e.Cancel = !_isClosed;
    //     }
    // }

    private void LoadWindowLayout()
    {
        // var boundsString = _viewModel.Settings.WindowBounds;
        // if (!string.IsNullOrEmpty(boundsString))
        // {
        //     try
        //     {
        //         var bounds = Rect.Parse(boundsString);
        //         if (bounds != default)
        //         {
        //             Left = bounds.Left;
        //             Top = bounds.Top;
        //             Width = bounds.Width;
        //             Height = bounds.Height;
        //         }
        //     }
        //     catch (FormatException)
        //     {
        //     }
        // }

        // if (Enum.TryParse(_viewModel.Settings.WindowState, out WindowState state) &&
        //     state != WindowState.Minimized)
        // {
        //     WindowState = state;
        // }

        // if (_viewModel.Settings.WindowFontSize.HasValue)
        // {
        //     FontSize = _viewModel.Settings.WindowFontSize.Value;
        // }

        // Width = Math.Clamp(Width, 0, SystemParameters.VirtualScreenWidth);
        // Height = Math.Clamp(Height, 0, SystemParameters.VirtualScreenHeight);
        // Left = Math.Clamp(Left, SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenWidth - Width);
        // Top = Math.Clamp(Top, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenHeight - Height);
    }

    private void SaveWindowLayout()
    {
        // _viewModel.Settings.WindowBounds = RestoreBounds.ToString(CultureInfo.InvariantCulture);
        // _viewModel.Settings.WindowState = this.WindowState.ToString();
    }

    private void LoadDockLayout()
    {
        var layout = viewModel.Settings.DockLayout;
        if (string.IsNullOrEmpty(layout))
        {
            return;
        }

        var serializer = new XmlLayoutSerializer(DockingManager);
        var reader = new StringReader(layout);
        try
        {
            serializer.Deserialize(reader);
        }
        catch
        {
            // ignored
        }
    }

    private void SaveDockLayout()
    {
        var serializer = new XmlLayoutSerializer(DockingManager);
        var document = new XDocument();
        using (System.Xml.XmlWriter writer = document.CreateWriter())
        {
            serializer.Serialize(writer);
        }

        document.Root?.Element("FloatingWindows")?.Remove();
        viewModel.Settings.DockLayout = document.ToString();
    }

    // protected override void OnClosed(EventArgs e)
    // {
    //     base.OnClosed(e);

    //     Application.Current.Shutdown();
    // }

    private async void DockingManager_OnDocumentClosing(object? sender, DocumentClosingEventArgs e)
    {
        e.Cancel = true;
        var document = (OpenDocumentViewModel)e.Document.Content;
        await viewModel.OnCloseDocumentAsync(document).ConfigureAwait(false);
    }

    private void ViewErrorDetails_OnClick(object? sender, RoutedEventArgs e)
    {
        if (viewModel.LastError == null)
        {
            return;
        }

        // var taskDialog = new TaskDialog
        // {
        //     Header = "Unhandled Exception",
        //     Content = viewModel.LastError.ToString(),
        //     Buttons =
        //     {
        //         TaskDialogButtonData.FromStandardButtons(TaskDialogButtons.Close).First()
        //     }
        // };
        //
        // taskDialog.SetResourceReference(BackgroundProperty, SystemColors.WindowBrushKey);
        // taskDialog.ShowInline(this);
    }

    private void ViewUpdateClick(object? sender, RoutedEventArgs e)
    {
        _ = Task.Run(() => Process.Start(new ProcessStartInfo("https://roslynpad.net/") { UseShellExecute = true }));
    }

    private void ILViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetShowIL();
    }

    private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
    {
        SetShowIL();
    }

    private void SetShowIL()
    {
        // if (_viewModel.CurrentOpenDocument is not { } currentDocument)
        // {
        //     return;
        // }

        // currentDocument.ShowIL = ILViewer.IsVisible;
    }
}