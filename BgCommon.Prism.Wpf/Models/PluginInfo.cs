namespace BgCommon.Prism.Wpf.Models;

/// <summary>
/// 封装了从外部程序集中发现的一个插件的详细信息.
/// </summary>
public partial class PluginInfo : ObservableObject
{
    private readonly Type pluginType;

    /// <summary>
    /// Gets 插件的类型 (System.Type).
    /// </summary>
    public Type PluginType => pluginType;

    /// <summary>
    /// Gets 类型的名称 (例如 "MyPlugin").
    /// </summary>
    public string Name => PluginType?.Name ?? string.Empty;

    /// <summary>
    /// Gets 类型的完全限定名 (例如 "MyNamespace.MyPlugin").
    /// </summary>
    public string FullName => PluginType?.FullName ?? string.Empty;

    /// <summary>
    /// Gets 程序集的名称 (不带扩展名, 例如 "MyPluginLibrary").
    /// </summary>
    public string AssemblyName => PluginType?.Assembly?.GetName()?.Name ?? string.Empty;

    /// <summary>
    /// Gets 类型的 AssemblyQualifiedName (例如 "MyNamespace.MyPlugin, MyPluginLibrary, ...").
    /// </summary>
    public string AssemblyQualifiedName => $"{FullName},{AssemblyName}";

    /// <summary>
    /// 指示用户是否已选择启用此插件.
    /// </summary>
    [ObservableProperty]
    private bool isEnabled;

    public PluginInfo(Type pluginType)
    {
        this.pluginType = pluginType;
    }
}