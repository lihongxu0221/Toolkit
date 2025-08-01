namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 实现 <see cref="T:CE.Core.Contour.IChangedEvent"/> 接口的组件基类。
/// 派生类对象在状态发生变更时将触发Changed事件。
/// </summary>
[Serializable]
public class SerializableChangedEventBase : SerializableObjectBase, IChangedEvent, IHasChanged
{
    /// <summary>
    /// 供派生类使用的下一个状态标志位值
    /// </summary>
    protected const long SfNextSf = 1L;

    [NonSerialized]
    private bool mHasChanged;

    [NonSerialized]
    private ChangedEventImpl _changedEvent = new ChangedEventImpl();

    /// <summary>
    /// Gets or sets a value indicating whether 获取或设置对象是否发生变更的标识
    /// </summary>
    public bool HasChanged
    {
        get { return mHasChanged; }
        set { mHasChanged = value; }
    }

    /// <summary>
    /// Gets changed事件挂起计数（非零表示事件触发被挂起）
    /// </summary>
    /// <remarks> 调用SuspendChangedEvent递增，调用ResumeAndRaiseChangedEvent递减 </remarks>
    [Browsable(false)]
    public int ChangedEventSuspended => _changedEvent.ChangedEventSuspended;

    /// <summary>
    /// Gets 获取本对象支持的全量状态标志集合
    /// </summary>
    /// <remarks>
    /// 使用示例：<br/>
    /// if (changedObject.StateFlags["Color"] &amp; eventArgs.StateFlags) { ... }
    /// </remarks>
    [Browsable(false)]
    public StateFlagsCollection StateFlags => _changedEvent.StateFlags;

    /// <summary>
    /// 对象状态变更事件
    /// </summary>
    public event ChangedEventHandler? Changed
    {
        add { _changedEvent.Changed += value; }
        remove { _changedEvent.Changed -= value; }
    }

    /// <summary>
    /// 挂起Changed事件触发（支持嵌套调用）
    /// </summary>
    public void SuspendChangedEvent()
    {
        _changedEvent.SuspendChangedEvent();
    }

    /// <summary>
    /// 恢复Changed事件触发，并在挂起期间存在变更时触发事件
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// 当挂起计数为零时调用将引发异常
    /// </exception>
    public void ResumeAndRaiseChangedEvent()
    {
        _changedEvent.ResumeAndRaiseChangedEvent(this);
    }

    /// <summary>
    /// 触发状态变更事件（基础实现）
    /// </summary>
    /// <param name="stateFlags">变更状态标志位掩码</param>
    protected void OnChanged(long stateFlags)
    {
        OnChanged(new ChangedEventArgs(stateFlags));
    }

    /// <summary>
    /// 触发状态变更事件（扩展实现）
    /// </summary>
    /// <param name="eventArgs">包含变更详情的事件参数</param>
    protected virtual void OnChanged(ChangedEventArgs eventArgs)
    {
        mHasChanged = true;
        _changedEvent.OnChanged(this, eventArgs);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableChangedEventBase"/> class.
    /// 反序列化构造函数
    /// </summary>
    /// <param name="info">序列化数据容器</param>
    /// <param name="context">序列化上下文信息</param>
    protected SerializableChangedEventBase(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableChangedEventBase"/> class.
    /// 默认构造函数
    /// </summary>
    protected SerializableChangedEventBase()
    {
    }
}