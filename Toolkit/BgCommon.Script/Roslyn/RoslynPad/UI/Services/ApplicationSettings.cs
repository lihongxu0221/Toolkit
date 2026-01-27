using BgCommon.Core;
using RoslynPad.Themes;

namespace RoslynPad.UI;

/// <summary>
/// 程序集设置服务.
/// </summary>
internal partial class ApplicationSettings : IApplicationSettings
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly ITelemetryProvider? telemetryProvider;
    private SerializableValues serializableValues;
    private string? path;

    public ApplicationSettings([Import(AllowDefault = true)] ITelemetryProvider telemetryProvider)
    {
        this.telemetryProvider = telemetryProvider;
        serializableValues = new SerializableValues();
        InitializeValues();
    }

    private void InitializeValues()
    {
        serializableValues.PropertyChanged += (_, _) => SaveSettings();
        serializableValues.Settings = this;
    }

    public void LoadDefault() =>
        LoadFrom(Path.Combine(GetDefaultDocumentPath(), DefaultConfigFileName));

    public void LoadFrom(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        LoadSettings(path);

        this.path = path;
    }

    public IApplicationSettingsValues Values => serializableValues;

    public string GetDefaultDocumentPath()
    {
        string? documentsPath;

        // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        // {
        //     documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // }
        // else
        // {
        //     // Unix or Mac
        //     documentsPath = Environment.GetEnvironmentVariable("HOME");
        // }
        documentsPath = FileNames.ScriptsPath;
        if (string.IsNullOrEmpty(documentsPath))
        {
            documentsPath = "/";
            telemetryProvider?.ReportError(new InvalidOperationException("Unable to locate the user documents folder; Using root"));
        }

        return Path.Combine(documentsPath, DefaultConfigFolderName);
    }

    private void LoadSettings(string path)
    {
        if (!File.Exists(path))
        {
            serializableValues.LoadDefaultSettings();
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            serializableValues = JsonSerializer.Deserialize<SerializableValues>(json, SerializerOptions) ?? new SerializableValues();
            InitializeValues();
        }
        catch (Exception e)
        {
            serializableValues.LoadDefaultSettings();
            telemetryProvider?.ReportError(e);
        }
    }

    private void SaveSettings()
    {
        if (path == null)
        {
            return;
        }

        try
        {
            using FileStream stream = File.Create(path);
            JsonSerializer.Serialize(stream, serializableValues, SerializerOptions);
        }
        catch (Exception e)
        {
            telemetryProvider?.ReportError(e);
        }
    }

    private partial class SerializableValues : ObservableObject, IApplicationSettingsValues
    {
        private const int LiveModeDelayMsDefault = 2000;
        private const int DefaultFontSize = 12;
        private string? _dockLayout;
        private string? _effectiveDocumentPath;

        public void LoadDefaultSettings()
        {
            SendErrors = true;
            FormatDocumentOnComment = true;
            EditorFontSize = DefaultFontSize;
            OutputFontSize = DefaultFontSize;
            LiveModeDelayMs = LiveModeDelayMsDefault;
            EditorFontFamily = GetDefaultPlatformFontFamily();
        }

        private static string GetDefaultPlatformFontFamily()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Cascadia Code,Consolas";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "Menlo";
            }
            else
            {
                return "Monospace";
            }
        }

        [ObservableProperty]
        private bool sendErrors = false;

        [ObservableProperty]
        private bool enableBraceCompletion = true;

        [ObservableProperty]
        private string? latestVersion = string.Empty;

        [ObservableProperty]
        private string? windowBounds = string.Empty;

        [JsonPropertyName("dockLayoutV2")]
        public string? DockLayout
        {
            get => _dockLayout;
            set => SetProperty(ref _dockLayout, value);
        }

        [ObservableProperty]
        private string? windowState = string.Empty;

        [ObservableProperty]
        private double editorFontSize = DefaultFontSize;

        [ObservableProperty]
        private string editorFontFamily = GetDefaultPlatformFontFamily();

        [ObservableProperty]
        private double outputFontSize = DefaultFontSize;

        [ObservableProperty]
        private string? documentPath = string.Empty;

        [ObservableProperty]
        private bool searchFileContents = false;

        [ObservableProperty]
        private bool searchUsingRegex = false;

        [ObservableProperty]
        private bool optimizeCompilation = false;

        [ObservableProperty]
        private int liveModeDelayMs = LiveModeDelayMsDefault;

        [ObservableProperty]
        private bool searchWhileTyping = false;

        [ObservableProperty]
        private string defaultPlatformName = string.Empty;

        [ObservableProperty]
        private double? windowFontSize = null;

        [ObservableProperty]
        private bool formatDocumentOnComment = true;

        [ObservableProperty]
        private string? customThemePath = string.Empty;

        [ObservableProperty]
        private ThemeType? customThemeType;

        [ObservableProperty]
        private BuiltInTheme builtInTheme;

        [JsonIgnore]
        public string EffectiveDocumentPath
        {
            get
            {
                if (_effectiveDocumentPath == null)
                {

                    var userDefinedPath = DocumentPath;
                    _effectiveDocumentPath = !string.IsNullOrEmpty(userDefinedPath) && Directory.Exists(userDefinedPath)
                        ? userDefinedPath!
                        : Settings?.GetDefaultDocumentPath() ?? string.Empty;
                }

                return _effectiveDocumentPath;
            }
        }

        [JsonIgnore]
        public ApplicationSettings? Settings { get; set; }
    }
}
