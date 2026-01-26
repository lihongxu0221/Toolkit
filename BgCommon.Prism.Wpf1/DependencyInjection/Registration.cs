namespace BgCommon.Prism.Wpf.DependencyInjection;

/// <summary>
/// 注册方式
/// </summary>
public enum Registration
{
    Normal,
    Many,
    Singleton,
    ManySingleton,
    Scope,
    Navigation,
    Dialog,
    DialogWindow,

    [Obsolete("coming later , I'm considering how to implement it.")]
    Instance,
}