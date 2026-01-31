using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalon.Windows.Controls;
using RoslynPad.UI;

namespace BgCommon.Script.Views;

/// <summary>
/// Interaction logic for SaveDocumentDialog.xaml.
/// </summary>
[Export(typeof(ISaveDocumentDialog))]
internal partial class SaveDocumentDialog : UserControl, ISaveDocumentDialog, INotifyPropertyChanged
{
    private bool showDoNotSave;
    private bool allowNameEdit;
    private SaveResult result;
    private string documentName = string.Empty;
    private string filePath = string.Empty;
    private InlineModalDialog? dialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveDocumentDialog"/> class.
    /// </summary>
    public SaveDocumentDialog()
    {
        this.DataContext = this;
        this.InitializeComponent();
        this.DocumentName = null;
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (AllowNameEdit)
        {
            DocumentTextBox.Focus();
        }
        else
        {
            SaveButton.Focus();
        }
        SetSaveButtonStatus();
    }

    private void DocumentName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in e.Changes)
        {
            if (c.AddedLength == 0)
            {
                continue;
            }

            textBox.Select(c.Offset, c.AddedLength);
            var filteredText = invalidChars.Aggregate(
                textBox.SelectedText,
                (current, invalidChar) => current.Replace(invalidChar.ToString(), string.Empty));
            if (textBox.SelectedText != filteredText)
            {
                textBox.SelectedText = filteredText;
            }

            textBox.Select(c.Offset + c.AddedLength, 0);
        }
    }

    /// <summary>
    /// Gets or sets the name of the document.
    /// </summary>
    /// <remarks>Setting this property updates the status of the save button to reflect whether there are unsaved changes.</remarks>
    public string DocumentName
    {
        get
        {
            return this.documentName;
        }

        set
        {
            _ = SetProperty(ref this.documentName, value);
            SetSaveButtonStatus();
        }
    }

    private void SetSaveButtonStatus()
    {
        SaveButton.IsEnabled = !AllowNameEdit || !string.IsNullOrWhiteSpace(DocumentName);
    }

    public SaveResult Result
    {
        get => result; private set => SetProperty(ref result, value);
    }

    public bool AllowNameEdit
    {
        get => allowNameEdit;
        set => SetProperty(ref allowNameEdit, value);
    }

    public bool ShowDoNotSave
    {
        get => showDoNotSave;
        set => SetProperty(ref showDoNotSave, value);
    }

    public string FilePath
    {
        get => filePath;
        private set => SetProperty(ref filePath, value);
    }

    public Func<string, string>? FilePathFactory { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        return false;
    }

    public Task ShowAsync()
    {
        dialog = new InlineModalDialog
        {
            Owner = Application.Current.MainWindow,
            Content = this,
        };

        dialog.Show();
        return Task.CompletedTask;
    }

    public void Close()
    {
        dialog?.Close();
    }

    private void PerformSave()
    {
        if (AllowNameEdit && !string.IsNullOrEmpty(DocumentName))
        {
            FilePath = FilePathFactory?.Invoke(DocumentName) ?? throw new InvalidOperationException();
            if (File.Exists(FilePath))
            {
                SaveButton.Visibility = Visibility.Collapsed;
                OverwriteButton.Visibility = Visibility.Visible;
                DocumentTextBox.IsEnabled = false;
                _ = Dispatcher.InvokeAsync(OverwriteButton.Focus);
            }
            else
            {
                Result = SaveResult.Save;
                Close();
            }
        }
        else
        {
            Result = SaveResult.Save;
            Close();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Overwrite_Click(object? sender, RoutedEventArgs e)
    {
        Result = SaveResult.Save;
        Close();
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        PerformSave();
    }

    private void DontSave_Click(object? sender, RoutedEventArgs e)
    {
        Result = SaveResult.DoNotSave;
        Close();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void DocumentText_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && SaveButton.IsEnabled)
        {
            PerformSave();
        }
    }
}