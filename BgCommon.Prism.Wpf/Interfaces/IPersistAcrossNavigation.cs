namespace BgCommon.Prism.Wpf;

/// <summary>
/// 实现此接口的 ViewModel 将不会在导航离开其视图时被 ModuleHostViewModel 销毁。
/// 这通常用于标记那些在 DI 容器中注册为单例的 ViewModel。
/// </summary>
public interface IPersistAcrossNavigation { }