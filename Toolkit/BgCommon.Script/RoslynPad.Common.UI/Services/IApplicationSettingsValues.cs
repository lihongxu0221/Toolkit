using BgCommon.Script;
using RoslynPad.Themes;
using System.ComponentModel;

namespace RoslynPad.UI;

/// <summary>
/// 应用程序设置数值接口，提供配置项的读取与写入支持.
/// </summary>
public interface IApplicationSettingsValues : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets a value indicating whether 是否发送错误报告.
    /// </summary>
    bool SendErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否启用括号自动补全.
    /// </summary>
    bool EnableBraceCompletion { get; set; }

    /// <summary>
    /// Gets or sets 最新版本号.
    /// </summary>
    string? LatestVersion { get; set; }

    /// <summary>
    /// Gets or sets 窗口位置与大小边界.
    /// </summary>
    string? WindowBounds { get; set; }

    /// <summary>
    /// Gets or sets 停靠布局配置.
    /// </summary>
    string? DockLayout { get; set; }

    /// <summary>
    /// Gets or sets 窗口显示状态.
    /// </summary>
    string? WindowState { get; set; }

    /// <summary>
    /// Gets or sets 编辑器字体大小.
    /// </summary>
    double EditorFontSize { get; set; }

    /// <summary>
    /// Gets or sets 编辑器字体系列.
    /// </summary>
    string EditorFontFamily { get; set; }

    /// <summary>
    /// Gets or sets 输出窗口字体大小.
    /// </summary>
    double OutputFontSize { get; set; }

    /// <summary>
    /// Gets or sets 文档保存路径.
    /// </summary>
    string? DocumentPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否搜索文件内容.
    /// </summary>
    bool SearchFileContents { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否使用正则表达式搜索.
    /// </summary>
    bool SearchUsingRegex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否在编译时执行优化.
    /// </summary>
    bool OptimizeCompilation { get; set; }

    /// <summary>
    /// Gets or sets 实时模式下的触发延迟（毫秒）.
    /// </summary>
    int LiveModeDelayMs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否在输入时执行搜索.
    /// </summary>
    bool SearchWhileTyping { get; set; }

    /// <summary>
    /// Gets or sets 默认执行平台名称.
    /// </summary>
    string DefaultPlatformName { get; set; }

    /// <summary>
    /// Gets or sets 窗口界面字体大小.
    /// </summary>
    double? WindowFontSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 是否在输入注释时自动格式化文档.
    /// </summary>
    bool FormatDocumentOnComment { get; set; }

    /// <summary>
    /// Gets 当前有效的文档路径.
    /// </summary>
    string EffectiveDocumentPath { get; }

    /// <summary>
    /// Gets 自定义主题文件的路径.
    /// </summary>
    string? CustomThemePath { get; }

    /// <summary>
    /// Gets 自定义主题的类型.
    /// </summary>
    ThemeType? CustomThemeType { get; }

    /// <summary>
    /// Gets 当前选中的内置主题.
    /// </summary>
    BuiltInTheme BuiltInTheme { get; }
}