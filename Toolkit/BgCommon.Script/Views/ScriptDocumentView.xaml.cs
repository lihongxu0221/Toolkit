using System.Windows.Data;
using System.Windows.Input;
using BgCommon.Script.ViewModels;
using BgLogger;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Build;
using RoslynPad.Editor;
using RoslynPad.UI;

namespace BgCommon.Script.Views;

/// <summary>
/// DocumentView.xaml 的交互逻辑
/// </summary>
public partial class ScriptDocumentView : UserControl, IDisposable
{
    private readonly MarkerMargin errorMargin;
    private OpenDocumentViewModel? viewModel;

    public OpenDocumentViewModel ViewModel => viewModel.NotNull();

    public ScriptDocumentView()
    {
        InitializeComponent();
        errorMargin = new MarkerMargin
        {
            Visibility = Visibility.Collapsed,
            MarkerImage = TryFindResource("ExceptionMarker") as ImageSource,
            Width = 10,
        };

        Editor.TextArea.LeftMargins.Insert(0, errorMargin);
        Editor.PreviewMouseWheel += EditorPreviewMouseWheel;
        Editor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
        Editor.TextArea.SelectionChanged += EditorSelectionChanged;
        DataContextChanged += OnDataContextChanged;
    }

    private void EditorSelectionChanged(object? sender, EventArgs e)
    {
        ViewModel.SelectedText = Editor.SelectedText;
    }

    private void CaretOnPositionChanged(object? sender, EventArgs eventArgs)
    {
        Ln.Text = Editor.TextArea.Caret.Line.ToString(CultureInfo.InvariantCulture);
        Col.Text = Editor.TextArea.Caret.Column.ToString(CultureInfo.InvariantCulture);
    }

    private void EditorPreviewMouseWheel(object? sender, MouseWheelEventArgs args)
    {
        if (viewModel == null)
        {
            return;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            viewModel.MainViewModel.EditorFontSize += args.Delta > 0 ? 1 : -1;
            args.Handled = true;
        }
    }

    private async void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs args)
    {
        try
        {
            if (args.NewValue is OpenDocumentViewModel newVM)
            {
                if (this.viewModel != null)
                {
                    OnHandleOpenDocumentVMEvents(this.viewModel, false);
                    this.viewModel.Dispose();
                    this.viewModel = null;
                }

                this.viewModel = newVM;
                BindingOperations.EnableCollectionSynchronization(viewModel.Results, viewModel.Results);
                OnHandleOpenDocumentVMEvents(viewModel, true);
                Editor.FontSize = viewModel.MainViewModel.EditorFontSize;
                string documentText = await viewModel.LoadTextAsync().ConfigureAwait(true);
                Microsoft.CodeAnalysis.DocumentId documentId = await Editor.InitializeAsync(
                    viewModel.MainViewModel.RoslynHost,
                    new ThemeClassificationColors(viewModel.MainViewModel.Theme),
                    viewModel.WorkingDirectory,
                    documentText,
                    viewModel.SourceCodeKind).ConfigureAwait(true);

                viewModel.Initialize(
                    documentId,
                    OnError,
                    () => new TextSpan(Editor.SelectionStart, Editor.SelectionLength),
                    this);

                Editor.Document.TextChanged += (o, e) => viewModel.OnTextChanged();
            }
        }
        catch (Exception ex)
        {
            LogRun.Error(ex);
        }
    }

    private void OnHandleOpenDocumentVMEvents(OpenDocumentViewModel? viewModel, bool isSubscribe)
    {
        if (viewModel == null)
        {
            return;
        }

        if (isSubscribe)
        {
            viewModel.ReadInput += OnReadInput;
            viewModel.EditorFocus += OnEditorFocused;
            viewModel.EditorChangeLocation += OnEditorChangeLocation;
            viewModel.DocumentUpdated += OnDocumentUpdated;
            viewModel.MainViewModel.EditorFontSizeChanged += OnEditorFontSizeChanged;
            ViewModel.MainViewModel.ThemeChanged += OnThemeChanged;
        }
        else
        {
            viewModel.ReadInput -= OnReadInput;
            viewModel.EditorFocus -= OnEditorFocused;
            viewModel.EditorChangeLocation -= OnEditorChangeLocation;
            viewModel.DocumentUpdated -= OnDocumentUpdated;
            viewModel.MainViewModel.EditorFontSizeChanged -= OnEditorFontSizeChanged;
            ViewModel.MainViewModel.ThemeChanged -= OnThemeChanged;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        Editor.ClassificationHighlightColors = new ThemeClassificationColors(ViewModel.MainViewModel.Theme);
    }

    private void OnReadInput()
    {
        // var textBox = new TextBox();
        // var dialog = new TaskDialog
        // {
        //     Header = "Console Input",
        //     Content = textBox,
        //     Background = Brushes.White,
        // };

        // textBox.Loaded += (o, e) => textBox.Focus();
        // textBox.KeyDown += (o, e) =>
        // {
        //     if (e.Key == Key.Enter)
        //     {
        //         TaskDialog.CancelCommand.Execute(null, dialog);
        //     }
        // };

        // dialog.ShowInline(this);
        // ViewModel.SendInput(textBox.Text);
    }

    private void OnEditorFocused(object? sender, EventArgs e)
    {
        _ = Dispatcher.InvokeAsync(Editor.Focus, DispatcherPriority.Background);
    }

    private void OnEditorChangeLocation((int line, int column) value)
    {
        OnChangePosition(value.line, value.column);
    }

    private void OnDocumentUpdated(object? sender, EventArgs e)
    {
        _ = Dispatcher.InvokeAsync(() =>
        {
            Editor.RefreshHighlighting();
            _ = Editor.RefreshFoldings();
        });
    }

    private void OnEditorFontSizeChanged(double fontSize)
    {
        Editor.FontSize = fontSize;
    }

    private void OnError(ExceptionResultObject? e)
    {
        if (e != null)
        {
            errorMargin.Visibility = Visibility.Visible;
            errorMargin.LineNumber = e.LineNumber;
            errorMargin.Message = "Exception: " + e.Message;
        }
        else
        {
            errorMargin.Visibility = Visibility.Collapsed;
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
    }

    private void Editor_OnLoaded(object? sender, RoutedEventArgs e)
    {
        _ = Dispatcher.InvokeAsync(Editor.Focus, DispatcherPriority.Background);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (viewModel?.MainViewModel is not { } mainViewModel)
            {
                return;
            }

            mainViewModel.EditorFontSizeChanged -= OnEditorFontSizeChanged;
            mainViewModel.ThemeChanged -= OnThemeChanged;
        }
    }

    private void OnChangePosition(int lineNumber, int column)
    {
        Editor.TextArea.Caret.Line = lineNumber;
        Editor.TextArea.Caret.Column = column;
        Editor.ScrollToLine(lineNumber);

        _ = Dispatcher.InvokeAsync(Editor.Focus);
    }

    private void SearchTerm_OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            _ = Editor.Focus();
        }

        // else if (e.Key == Key.Down && ViewModel.NuGet.Packages?.Any() == true)
        // {
        //     if (!ViewModel.NuGet.IsPackagesMenuOpen)
        //     {
        //         ViewModel.NuGet.IsPackagesMenuOpen = true;
        //     }
        //     RootNuGetMenu.Focus();
        // }
    }
}
