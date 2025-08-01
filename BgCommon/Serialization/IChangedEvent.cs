namespace BgCommon.Serialization;

/// <summary>
/// 该接口包含 Changed 事件及其挂起/恢复控制方法
/// </summary>
public interface IChangedEvent
{
    /// <summary>
    /// 指示 Changed 事件是否处于挂起状态
    /// </summary>
    /// <value>
    /// 非零值表示 Changed 事件触发已被挂起。调用 SuspendChangedEvent 时递增，
    /// 调用 ResumeAndRaiseChangedEvent 时递减
    /// </value>
    [Browsable(false)]
    int ChangedEventSuspended { get; }

    /// <summary>
    /// 获取本对象支持的全量状态标志集合。可通过标志名称索引访问，
    /// 如以下 C# 代码示例：
    /// if (changedObject.StateFlags["Color"] &amp; eventArgs.StateFlags) { ... }
    /// </summary>
    StateFlagsCollection StateFlags { get; }

    /// <summary>
    /// 当对象状态发生变更时触发此事件
    /// </summary>
    event ChangedEventHandler? Changed;

    /// <summary>
    /// 临时挂起 Changed 事件触发。可多次调用，每次调用需对应一次 
    /// ResumeAndRaiseChangedEvent 调用
    /// </summary>
    void SuspendChangedEvent();

    /// <summary>
    /// 恢复被挂起的 Changed 事件触发。当挂起计数归零且挂起期间存在变更时，
    /// 将触发 Changed 事件。必须与 SuspendChangedEvent 调用次数匹配
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">当当前挂起计数为零时抛出 </exception>
    /// <event cref="E:CE.Core.Contour.IChangedEvent">当挂起计数归零且挂起期间存在被抑制的变更事件时触发 </event>
    void ResumeAndRaiseChangedEvent();
}