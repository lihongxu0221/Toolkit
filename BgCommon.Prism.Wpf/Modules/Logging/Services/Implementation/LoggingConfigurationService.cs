using BgCommon.Prism.Wpf.Modules.Logging.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BgCommon.Prism.Wpf.Modules.Logging.Services.Implementation;

/// <summary>
/// 日志配置服务实现类，用于管理日志源的配置信息。
/// </summary>
internal class LoggingConfigurationService : ILoggingConfigurationService
{
    private readonly string appSettingsPath; // appsettings.json 文件路径

    private readonly IConfiguration configuration;
    private readonly AppSettings appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingConfigurationService"/> class.
    /// 构造函数，初始化日志配置服务。
    /// </summary>
    ///// <param name="appSettings">应用程序设置对象。</param>
    ///// <param name="configuration">注入 IConfiguration 以获取路径（如可能），否则可硬编码或配置。</param>
    public LoggingConfigurationService(AppSettings appSettings, IConfiguration configuration)
    {
        this.appSettings = appSettings;
        this.configuration = configuration;

        // this.appSettings = appSettings;
        // 尝试查找 appsettings.json 文件。
        // 这在应用部署方式不同的情况下可能会有些棘手。
        // 在开发场景下，CurrentDirectory 通常可用。
        appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "bglogger.appsettings.json");
        if (!File.Exists(appSettingsPath))
        {
            // 如果未找到 appsettings.json，则回退或进行错误处理
            // 如果文件未正确复制或工作目录不同，在已发布应用中可能会出现此问题
            // 更健壮的方案可能需要通过配置指定路径
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                appSettingsPath = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "bglogger.appsettings.json");
            }
        }

        // 如果 appsettings.json 中没有日志源设置，则确保使用默认设置
        if (this.appSettings.LogSourceSettings == null || !this.appSettings.LogSourceSettings.Any())
        {
            this.appSettings.LogSourceSettings = new List<LogSourceSetting>();
        }

        // 确保所有枚举值都有对应的设置
        foreach (BgLoggerSource sourceEnum in Enum.GetValues(typeof(BgLoggerSource)))
        {
            if (sourceEnum == BgLoggerSource.UnKnowm)
            {
                continue;
            }

            if (!this.appSettings.LogSourceSettings.Any(s => s.Name.Equals(sourceEnum.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                this.appSettings.LogSourceSettings.Add(new LogSourceSetting { Name = sourceEnum.ToString() });
            }
        }
    }

    /// <summary>
    /// 获取所有日志源的配置信息。
    /// </summary>
    /// <returns>日志源配置信息集合。</returns>
    public IEnumerable<LogSourceSetting> GetLogSourceSettings()
    {
        return appSettings.LogSourceSettings.Select(s => new LogSourceSetting(s)).ToList();
    }

    /// <summary>
    /// 根据日志源名称获取对应的配置信息。
    /// </summary>
    /// <param name="sourceName">日志源名称。</param>
    /// <returns>日志源配置信息。</returns>
    public LogSourceSetting GetSettingForSource(string sourceName)
    {
        LogSourceSetting? setting = appSettings.LogSourceSettings.FirstOrDefault(s => s.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase));
        if (setting == null)
        {
            return new LogSourceSetting { Name = sourceName };
        }

        return new LogSourceSetting(setting);
    }

    /// <summary>
    /// 异步保存日志源配置信息到 appsettings.json 文件，并更新内存中的设置。
    /// </summary>
    /// <param name="settingsToSave">要保存的日志源配置信息集合。</param>
    /// <returns>保存成功返回 true，否则抛出异常。</returns>
    /// <exception cref="FileNotFoundException">如果 appsettings.json 文件未找到，则抛出此异常。</exception>
    /// <exception cref="Exception">保存过程中发生的其他异常。</exception>
    public bool SaveLogSourceSettings(IEnumerable<LogSourceSetting> settingsToSave)
    {
        if (!File.Exists(appSettingsPath))
        {
            BgLoggerSource.General.Error($"appsettings.json not found at {appSettingsPath}. Cannot save settings.");
            throw new FileNotFoundException("appsettings.json not found.", appSettingsPath);
        }

        try
        {
            // 读取 appsettings.json 文件内容
            string json = File.ReadAllText(appSettingsPath);
            var jsonObj = JObject.Parse(json); // 使用 Newtonsoft.Json 解析

            // 确保 AppSettings 节点存在
            var appSettingsSection = jsonObj["AppSettings"] as JObject;
            if (appSettingsSection == null)
            {
                appSettingsSection = new JObject();
                jsonObj["AppSettings"] = appSettingsSection;
            }

            // 更新或创建 LogSourceSettings 节点
            appSettingsSection["LogSourceSettings"] = JArray.FromObject(settingsToSave, JsonSerializer.CreateDefault(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            // 将更新后的内容写回 appsettings.json 文件
            File.WriteAllText(appSettingsPath, jsonObj.ToString(Newtonsoft.Json.Formatting.Indented));

            // 更新内存中的 _appSettings 实例
            appSettings.LogSourceSettings.Clear();
            foreach (LogSourceSetting s in settingsToSave)
            {
                appSettings.LogSourceSettings.Add(new LogSourceSetting(s));
            }

            BgLoggerSource.General.Info("Log source settings saved to appsettings.json and reloaded into memory.");

            // 重要说明：
            // 1. 配置自动重载：Microsoft.Extensions.Configuration 可配置为自动重载 appsettings.json。
            //    如果配置了，IConfiguration 和 IOptions<AppSettings> 会自动更新。
            //    NLog 的 autoReload=true 也会自动检测 NLog 配置的更改。
            // 2. UI 刷新：UI（如 MainWindowViewModel 的 LogSources）需要刷新显示设置。
            //    可通过事件聚合器（EventAggregator）实现通知。
            return true;
        }
        catch (Exception ex)
        {
            BgLoggerSource.General.Error(ex, "Failed to save log source settings.");
            throw; // 重新抛出异常以通知调用方
        }
    }
}