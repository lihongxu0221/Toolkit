using BgCommon.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using RoslynPad.Build;
using RoslynPad.Roslyn;
using RoslynPad.Themes;
using RoslynPad.UI;
using System.Reflection;

namespace BgCommon.Script.ViewModels;

public partial class ScriptMainViewModel : NavigableDialogViewModel
{
    private static readonly Version CurrentVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();

    private readonly IFileDialogService fileDialogService;
    private readonly ITelemetryProvider telemetryProvider;
    private readonly DocumentFileWatcher documentFileWatcher;
    private readonly string editorConfigPath;
    private readonly VsCodeThemeReader themeManager;

    public const double MinimumFontSize = 8;
    public const double MaximumFontSize = 72;

    private double editorFontSize;
    private bool isWithinSearchResults;
    private bool isInitialized;
    private ObservableCollection<OpenDocumentViewModel> openDocuments = new ObservableCollection<OpenDocumentViewModel>();
    private OpenDocumentViewModel? currentOpenDocument;
    private DocumentViewModel documentRoot;
    private DocumentWatcher? documentWatcher;
    private Theme? theme;
    private bool? isSystemDarkTheme;

    public IApplicationSettingsValues Settings { get; }

    public DocumentViewModel DocumentRoot
    {
        get => documentRoot;
        private set => SetProperty(ref documentRoot, value);
    }

    [ObservableProperty]
    private RoslynHost? roslynHost = null;

    [ObservableProperty]
    private string? searchText = string.Empty;

    public string WindowTitle
    {
        get
        {
            var currentVersion = CurrentVersion switch
            {
                { Minor: <= 0, Build: <= 0 } => CurrentVersion.Major.ToString(CultureInfo.InvariantCulture),
                { Build: <= 0 } => $"{CurrentVersion.Major}.{CurrentVersion.Minor}",
                _ => CurrentVersion.ToString()
            };

            return "ScriptWindow " + currentVersion;
        }
    }

    public bool IsInitialized
    {
        get => isInitialized;
        set
        {
            _ = SetProperty(ref isInitialized, value);
            OnPropertyChanged(nameof(HasNoOpenDocuments));
        }
    }

    public ObservableCollection<OpenDocumentViewModel> OpenDocuments
    {
        get => openDocuments;
        private set => SetProperty(ref openDocuments, value);
    }

    public OpenDocumentViewModel? CurrentOpenDocument
    {
        get => currentOpenDocument;
        set
        {
            if (value == null)
            {
                return;
            }

            _ = SetProperty(ref currentOpenDocument, value);
            OnPropertyChanged(nameof(ActiveContent));
        }
    }

    public object? ActiveContent
    {
        get => CurrentOpenDocument;
        set
        {
            if (value is not OpenDocumentViewModel viewModel)
            {
                return;
            }

            CurrentOpenDocument = viewModel;
        }
    }

    public Exception? LastError
    {
        get
        {
            Exception? exception = telemetryProvider.LastError;
            var aggregateException = exception as AggregateException;
            return aggregateException?.Flatten() ?? exception;
        }
    }

    public bool HasError => LastError != null;

    public bool SendTelemetry
    {
        get
        {
            return Settings.SendErrors;
        }

        set
        {
            Settings.SendErrors = value;
            OnPropertyChanged(nameof(SendTelemetry));
        }
    }

    public bool HasNoOpenDocuments => IsInitialized && OpenDocuments.Count == 0;

    public double EditorFontSize
    {
        get => editorFontSize;
        set
        {
            if (!IsValidFontSize(value))
            {
                return;
            }

            if (SetProperty(ref editorFontSize, value))
            {
                Settings.EditorFontSize = value;
                EditorFontSizeChanged?.Invoke(value);
            }
        }
    }

    public bool IsWithinSearchResults
    {
        get => isWithinSearchResults;
        private set
        {
            _ = SetProperty(ref isWithinSearchResults, value);
            OnPropertyChanged(nameof(CanClearSearch));
        }
    }

    public bool CanClearSearch => IsWithinSearchResults || !string.IsNullOrEmpty(SearchText);

    public bool SearchFileContents
    {
        get => Settings.SearchFileContents;
        set
        {
            Settings.SearchFileContents = value;
            if (!value)
            {
                SearchUsingRegex = false;
            }

            OnPropertyChanged();
        }
    }

    public bool SearchUsingRegex
    {
        get => Settings.SearchUsingRegex;
        set
        {
            Settings.SearchUsingRegex = value;
            if (value)
            {
                SearchFileContents = true;
            }

            OnPropertyChanged();
        }
    }

    public bool UseSystemTheme { get; private set; }

    public ThemeType ThemeType { get; private set; } = ThemeType.Dark;

    public Theme Theme
    {
        get => theme.NotNull();
        private set
        {
            theme = value;
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler<EventArgs>? ThemeChanged;

    public event Action<double>? EditorFontSizeChanged;

    public ScriptMainViewModel(
        IContainerExtension container,
        IFileDialogService fileDialogService,
        ITelemetryProvider telemetryProvider,
        IApplicationSettings settings,
        DocumentFileWatcher documentFileWatcher,
        Theme? theme = null,
        bool? isSystemDarkTheme = null)
        : base(container)
    {
        this.fileDialogService = fileDialogService;
        this.telemetryProvider = telemetryProvider;
        this.documentFileWatcher = documentFileWatcher;
        themeManager = new VsCodeThemeReader();

        settings.LoadDefault();
        editorConfigPath = Path.Combine(settings.GetDefaultDocumentPath(), ".editorconfig");
        this.Settings = settings.Values;

        this.telemetryProvider.Initialize(CurrentVersion.ToString(), settings);
        this.telemetryProvider.LastErrorChanged += () =>
        {
            OnPropertyChanged(nameof(LastError));
            OnPropertyChanged(nameof(HasError));
        };

        editorFontSize = Settings.EditorFontSize;

        documentRoot = CreateDocumentRoot();

        OpenDocuments = new ObservableRangeCollection<OpenDocumentViewModel>();
        OpenDocuments.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(HasNoOpenDocuments));
        this.theme = theme;
        this.isSystemDarkTheme = isSystemDarkTheme;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            await InitializeInternalAsync().ConfigureAwait(true);
            UIService.RunOnUIThread(() =>
            {
                IsInitialized = true;
            });
        }
        catch (Exception e)
        {
            telemetryProvider.ReportError(e);
        }
    }

    public void InitializeTheme()
    {
        UseSystemTheme = string.IsNullOrEmpty(Settings.CustomThemePath) && Settings.BuiltInTheme == BuiltInTheme.System;

        (string? path, ThemeType type) theme = string.IsNullOrEmpty(Settings.CustomThemePath) ?
            GetBuiltinThemePath(Settings.BuiltInTheme) :
            (path: Settings.CustomThemePath, type: Settings.CustomThemeType.GetValueOrDefault());
        LoadTheme(theme.path, theme.type);

        if (UseSystemTheme)
        {
            ListenToSystemThemeChanges(() =>
            {
                (string? path, ThemeType type) buitInTheme = GetBuiltinThemePath(BuiltInTheme.System);
                LoadTheme(buitInTheme.path, buitInTheme.type);
            });
        }

        (string? path, ThemeType type) GetBuiltinThemePath(BuiltInTheme builtInTheme)
        {
            if (builtInTheme == BuiltInTheme.System)
            {
                var isSystemDarkTheme = IsSystemDarkTheme();
                if (this.isSystemDarkTheme == isSystemDarkTheme)
                {
                    return default;
                }

                builtInTheme = isSystemDarkTheme ? BuiltInTheme.Dark : BuiltInTheme.Light;
                this.isSystemDarkTheme = isSystemDarkTheme;
            }

            (string file, ThemeType type) theme = builtInTheme switch
            {
                BuiltInTheme.Light => ("light_modern.json", ThemeType.Light),
                BuiltInTheme.Dark => ("dark_modern.json", ThemeType.Dark),
                _ => throw new ArgumentOutOfRangeException(nameof(builtInTheme)),
            };

            return (GetOsSpecificThemePath(theme.file), theme.type);
        }

        static string GetOsSpecificThemePath(string path) =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? Path.Combine(AppContext.BaseDirectory, "..", "Resources", "Themes", path)
            : Path.Combine(AppContext.BaseDirectory, "Themes", path);

        void LoadTheme(string? themeFile, ThemeType type)
        {
            if (string.IsNullOrEmpty(themeFile))
            {
                return;
            }

            ThemeType = type;
            Theme = themeManager.ReadThemeAsync(themeFile, type).GetAwaiter().GetResult();
        }
    }

    protected ImmutableArray<Assembly> CompositionAssemblies =>
    [
        // typeof(ScriptMainViewModel).Assembly,
        // Assembly.Load(new AssemblyName("RoslynPad.Roslyn.Windows")),
        // Assembly.Load(new AssemblyName("RoslynPad.Editor.Windows"))
    ];

    protected void ListenToSystemThemeChanges(Action onChange)
    {
        SystemEvents.UserPreferenceChanging += (_, _) => onChange();
    }

    protected bool IsSystemDarkTheme()
    {
        // using RegistryKey? personalizeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        // return personalizeKey?.GetValue("AppsUseLightTheme") as int? == 0;
        return true;
    }

    private async Task InitializeInternalAsync()
    {
        RoslynHost = await Task.Run(() => new RoslynHost(
            CompositionAssemblies,
            RoslynHostReferences.NamespaceDefault.With(imports: ["RoslynPad.Runtime"]),
            disabledDiagnostics: ["CS1701", "CS1702", "CS7011", "CS8097"],
            analyzerConfigFiles: [editorConfigPath]))
            .ConfigureAwait(true);

        OpenDocumentFromCommandLine();
        await OpenAutoSavedDocumentsAsync().ConfigureAwait(true);
    }

    private void OpenDocumentFromCommandLine()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length > 1)
        {
            string filePath = args[1];

            if (File.Exists(filePath))
            {
                var document = DocumentViewModel.FromPath(filePath);
                OpenDocument(document);
            }
        }
    }

    private async Task OpenAutoSavedDocumentsAsync()
    {
        IEnumerable<OpenDocumentViewModel> documents = await Task.Run(() => LoadAutoSavedDocuments(DocumentRoot.Path))
            .ConfigureAwait(true);
        OpenDocuments.AddRange(documents);
        if (OpenDocuments.Count == 0)
        {
            CreateNewDocument();
        }
        else
        {
            CurrentOpenDocument = OpenDocuments[0];
        }
    }

    private IEnumerable<OpenDocumentViewModel> LoadAutoSavedDocuments(string root)
    {
        return IOUtilities.EnumerateFilesRecursive(root, $"*{DocumentViewModel.AutoSaveSuffix}.*")
            .Select(DocumentViewModel.FromPath)
            .Where(IsRelevantDocument)
            .Select(GetOpenDocumentViewModel);
    }

    private OpenDocumentViewModel GetOpenDocumentViewModel(DocumentViewModel? documentViewModel = null)
    {
        OpenDocumentViewModel d = this.Container.Resolve<OpenDocumentViewModel>();
        d.MainViewModel = this;
        d.SetDocument(documentViewModel);
        return d;
    }

    [MemberNotNull(nameof(documentWatcher))]
    private DocumentViewModel CreateDocumentRoot()
    {
        try
        {
            documentWatcher?.Dispose();
            var root = DocumentViewModel.CreateRoot(Settings.EffectiveDocumentPath);
            documentWatcher = new DocumentWatcher(documentFileWatcher, root);
            return root;
        }
        catch (Exception _)
        {
            throw;
        }
    }

    private void ClearCurrentOpenDocument()
    {
        if (CurrentOpenDocument == null)
        {
            return;
        }

        CurrentOpenDocument = null;
        OnPropertyChanged(nameof(CurrentOpenDocument));
    }

    public void OpenDocument(DocumentViewModel document)
    {
        if (document.IsFolder)
        {
            return;
        }

        OpenDocumentViewModel? openDocument = OpenDocuments.FirstOrDefault(x =>
            x.Document?.Path != null &&
            string.Equals(x.Document.Path, document.Path, StringComparison.Ordinal));
        if (openDocument == null)
        {
            openDocument = GetOpenDocumentViewModel(document);
            OpenDocuments.Add(openDocument);
        }

        CurrentOpenDocument = openDocument;
    }

    public void CreateNewDocument(SourceCodeKind kind = SourceCodeKind.Regular)
    {
        OpenDocumentViewModel openDocument = GetOpenDocumentViewModel();
        openDocument.SourceCodeKind = kind;
        OpenDocuments.Add(openDocument);
        CurrentOpenDocument = openDocument;
    }

    public async Task AutoSaveOpenDocumentsAsync()
    {
        foreach (OpenDocumentViewModel document in OpenDocuments)
        {
            await document.AutoSaveAsync().ConfigureAwait(false);
        }
    }

    public async Task CloseAllDocumentsAsync()
    {
        var openDocs = new ObservableCollection<OpenDocumentViewModel>(OpenDocuments);
        foreach (OpenDocumentViewModel document in openDocs)
        {
            await OnCloseDocumentAsync(document).ConfigureAwait(false);
        }
    }

    public async Task OnExitAsync()
    {
        await AutoSaveOpenDocumentsAsync().ConfigureAwait(false);
        IOUtilities.PerformIO(() => Directory.Delete(Path.Combine(Path.GetTempPath(), "roslynpad", "build"), recursive: true));
    }

    public DocumentViewModel AddDocument(string documentName)
    {
        return DocumentRoot.CreateNew(documentName);
    }

    private class DocumentWatcher : IDisposable
    {
        private static readonly char[] PathSeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

        private readonly DocumentViewModel documentRoot;
        private readonly IDisposable subscription;

        public DocumentWatcher(DocumentFileWatcher watcher, DocumentViewModel documentRoot)
        {
            this.documentRoot = documentRoot;
            watcher.Path = documentRoot.Path;
            subscription = watcher.Subscribe(OnDocumentFileChanged);
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        private void OnDocumentFileChanged(DocumentFileChanged data)
        {
            var pathParts = data.Path.Substring(documentRoot.Path.Length)
                .Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);

            DocumentViewModel? current = documentRoot;

            for (var index = 0; index < pathParts.Length; index++)
            {
                if (!current.IsChildrenInitialized)
                {
                    break;
                }

                var part = pathParts[index];
                var isLast = index == pathParts.Length - 1;

                DocumentViewModel parent = current;
                current = current.InternalChildren[part];
                if (current is null)
                {
                    if (data.Type != DocumentFileChangeType.Deleted)
                    {
                        var currentPath = isLast && data.Type == DocumentFileChangeType.Renamed
                            ? data.NewPath
                            : Path.Combine(documentRoot.Path, Path.Combine(pathParts.Take(index + 1).ToArray()));

                        var newDocument = DocumentViewModel.FromPath(currentPath!);
                        if (!newDocument.IsAutoSave &&
                            IsRelevantDocument(newDocument))
                        {
                            parent.AddChild(newDocument);
                        }
                    }

                    break;
                }

                if (isLast)
                {
                    switch (data.Type)
                    {
                        case DocumentFileChangeType.Renamed:
                            if (data.NewPath != null)
                            {
                                current.ChangePath(data.NewPath);
                                _ = parent.InternalChildren.Remove(current);
                                if (IsRelevantDocument(current))
                                {
                                    parent.AddChild(current);
                                }
                            }

                            break;
                        case DocumentFileChangeType.Deleted:
                            _ = parent.InternalChildren.Remove(current);
                            break;
                    }
                }
            }
        }
    }

    private static bool IsRelevantDocument(DocumentViewModel document)
    {
        return document.IsFolder || DocumentViewModel.RelevantFileExtensions.Contains(Path.GetExtension(document.Name));
    }

    protected override void OnDispose()
    {
        documentFileWatcher?.Dispose();
        documentWatcher?.Dispose();
        CurrentOpenDocument?.Dispose();
        base.OnDispose();
    }

    [RelayCommand]
    private void OnNewDocument(SourceCodeKind kind)
    {
        this.CreateNewDocument(kind);
    }

    [RelayCommand]
    private void OnOpenFile()
    {
        if (!IsInitialized)
        {
            return;
        }

        OpenFileOptions options = new()
        {
            Title = "C# Files",
            Multiselect = false,

            // InitialDirectory = Settings.EffectiveDocumentPath,
            Filters = new List<FileFilter>
            {
                new FileFilter("*.cs","C# Files"),
                new FileFilter("*.*","All Files" ),
            },
        };

        List<AssemblyItem> files = fileDialogService.ShowOpenFileDialog(options);
        if (files == null || files.Count == 0)
        {
            return;
        }

        var filePath = IOUtilities.NormalizeFilePath(files[0].FilePath);
        var document = DocumentViewModel.FromPath(filePath);
        if (!document.IsAutoSave)
        {
            var autoSavePath = document.GetAutoSavePath();
            if (File.Exists(autoSavePath))
            {
                document = DocumentViewModel.FromPath(autoSavePath);
            }
        }

        OpenDocument(document);
    }

    [RelayCommand]
    private async Task OnCloseCurrentDocumentAsync()
    {
        if (CurrentOpenDocument == null)
        {
            return;
        }

        await OnCloseDocumentAsync(CurrentOpenDocument).ConfigureAwait(false);
        if (!OpenDocuments.Any())
        {
            ClearCurrentOpenDocument();
        }
    }

    [RelayCommand]
    public async Task OnCloseDocumentAsync(OpenDocumentViewModel? document)
    {
        if (document == null)
        {
            return;
        }

        SaveResult result = await document.SaveAsync(promptSave: true).ConfigureAwait(true);
        if (result == SaveResult.Cancel)
        {
            return;
        }

        if (document.DocumentId != null)
        {
            RoslynHost?.CloseDocument(document.DocumentId);
        }

        _ = OpenDocuments.Remove(document);
        document.Close();
    }

    [RelayCommand]
    private void OnClearError() => telemetryProvider.ClearLastError();

    [RelayCommand]
    private void OnReportProblem()
    {
        _ = Task.Run(() => Process.Start(
            new ProcessStartInfo
            {
                FileName = "https://github.com/aelij/RoslynPad/issues",
                UseShellExecute = true,
            }));
    }

    [RelayCommand]
    private void OnEditUserDocumentPath()
    {
        OpenFolderOptions options = new()
        {
            Title = "C# Files",
            Multiselect = false,
            InitialDirectory = Settings.EffectiveDocumentPath,
        };

        List<string> folders = fileDialogService.ShowOpenFolderDialog(options);
        if (folders == null || folders.Count == 0)
        {
            return;
        }

        string documentPath = folders[0];
        if (!DocumentRoot.Path.Equals(documentPath, StringComparison.OrdinalIgnoreCase))
        {
            Settings.DocumentPath = documentPath;

            DocumentRoot = CreateDocumentRoot();
        }
    }

    [RelayCommand]
    private void OnToggleOptimization()
    {
        Settings.OptimizeCompilation = !Settings.OptimizeCompilation;
    }

    [RelayCommand]
    private void OnClearRestoreCache()
    {
        IOUtilities.PerformIO(() =>
        {
            Directory.Delete(Path.Combine(Path.GetTempPath(), "roslynpad", "restore"), recursive: true);
        });
    }

    [RelayCommand]
    private async Task OnSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            OnClearSearch();
            return;
        }

        if (!SearchFileContents)
        {
            IsWithinSearchResults = true;

            foreach (DocumentViewModel document in GetAllDocumentsForSearch(DocumentRoot))
            {
                document.IsSearchMatch = SearchDocumentName(document);
            }

            return;
        }

        Regex? regex = null;
        if (SearchUsingRegex)
        {
            regex = CreateSearchRegex();

            if (regex == null)
            {
                return;
            }
        }

        IsWithinSearchResults = true;

        foreach (DocumentViewModel document in GetAllDocumentsForSearch(DocumentRoot))
        {
            if (SearchDocumentName(document))
            {
                document.IsSearchMatch = true;
            }
            else
            {
                await SearchInFileAsync(document, regex).ConfigureAwait(false);
            }
        }

        bool SearchDocumentName(DocumentViewModel document)
        {
            return document.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        Regex? CreateSearchRegex()
        {
            try
            {
                var regex = new Regex(SearchText, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                // ClearError(nameof(SearchText), "Regex");
                return regex;
            }
            catch (ArgumentException)
            {
                // SetError(nameof(SearchText), "Regex", "Invalid regular expression");
                return null;
            }
        }

        async Task SearchInFileAsync(DocumentViewModel document, Regex? regex)
        {
            if (regex != null)
            {
                var documentText = await IOUtilities.ReadAllTextAsync(document.Path).ConfigureAwait(false);
                try
                {
                    document.IsSearchMatch = regex.IsMatch(documentText);
                }
                catch (RegexMatchTimeoutException)
                {
                    document.IsSearchMatch = false;
                }
            }
            else
            {
                await Task.Run(() =>
                {
                    IEnumerable<string> lines = IOUtilities.ReadLines(document.Path);
                    document.IsSearchMatch = lines.Any(line =>
                        line.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }).ConfigureAwait(false);
            }
        }
    }

    [RelayCommand]
    private void OnClearSearch()
    {
        SearchText = null;
        IsWithinSearchResults = false;
        ClearErrors(nameof(SearchText));

        foreach (var document in GetAllDocumentsForSearch(DocumentRoot))
        {
            document.IsSearchMatch = true;
        }
    }

    partial void OnSearchTextChanged(string? oldValue, string? newValue)
    {
        if (Settings.SearchWhileTyping)
        {
            SearchCommand.Execute(null);
        }

        OnPropertyChanged(nameof(CanClearSearch));
    }

    public static bool IsValidFontSize(double value) => value >= MinimumFontSize && value <= MaximumFontSize;

    private static IEnumerable<DocumentViewModel> GetAllDocumentsForSearch(DocumentViewModel root)
    {
        var children = root.Children;
        if (children is null)
        {
            yield break;
        }

        foreach (var document in children)
        {
            if (document.IsFolder)
            {
                foreach (var childDocument in GetAllDocumentsForSearch(document))
                {
                    yield return childDocument;
                }

                document.IsSearchMatch = document.Children?.Any(c => c.IsSearchMatch) == true;
            }
            else
            {
                yield return document;
            }
        }
    }
}