namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 实现 <see cref="T:CE.Core.Contour.IChangedEvent" /> 接口的组件基类。
/// 派生自该类的对象在其部分状态发生改变时会触发 Changed 事件。
/// </summary>
[Serializable]
public class SerializableChangedEventComponentBase : SerializableComponentBase, IChangedEvent, IHasChanged
{
    /// <summary>
    /// 供派生类使用的下一个状态标志值。
    /// </summary>
    protected const long SfNextSf = 1L;

    [NonSerialized]
    private bool mHasChanged;

    [NonSerialized]
    private ChangedEventImpl _ChangedEvent = new ChangedEventImpl();

    public bool HasChanged
    {
        get
        {
            return mHasChanged;
        }
        set
        {
            mHasChanged = value;
        }
    }

    /// <summary>
    /// 若该值不为零，则表示 Changed 事件的触发已被暂停。
    /// 调用 SuspendChangedEvent 方法时该值会增加，调用 ResumeAndRaiseChangedEvent 方法时该值会减少。
    /// </summary>
    [Browsable(false)]
    public int ChangedEventSuspended => _ChangedEvent.ChangedEventSuspended;

    /// <summary>
    /// 返回此对象支持的完整状态标志集。
    /// 可以通过名称来索引这些标志，如下 C# 代码片段所示：
    /// if (changedObject.StateFlags["Color"] & eventArgs.StateFlags) { ... }
    /// </summary>
    [Browsable(false)]
    public StateFlagsCollection StateFlags => _ChangedEvent.StateFlags;

    /// <summary>
    /// 当对象的一个或多个部分状态可能发生改变时，会触发此事件。
    /// </summary>
    public event ChangedEventHandler? Changed
    {
        add { _ChangedEvent.Changed += value; }
        remove { _ChangedEvent.Changed -= value; }
    }

    /// <summary>
    /// 暂时暂停 Changed 事件的触发。此方法可以被多次调用，
    /// 并且每次调用 SuspendChangedEvent 后都必须对应调用一次 ResumeAndRaiseChangedEvent 方法。
    /// </summary>
    public void SuspendChangedEvent()
    {
        _ChangedEvent.SuspendChangedEvent();
    }

    /// <summary>
    /// 在调用 SuspendChangedEvent 方法暂停事件触发后，重新启用 Changed 事件的触发。
    /// 如果 ChangedEventSuspended 计数减少到零，并且在事件暂停期间有任何状态改变，则触发 Changed 事件。
    /// 每次调用 SuspendChangedEvent 后都必须调用此方法一次。
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">如果当前 ChangedEventSuspended 计数为零。</exception>
    public void ResumeAndRaiseChangedEvent()
    {
        _ChangedEvent.ResumeAndRaiseChangedEvent(this);
    }

    /// <summary>
    /// 每当对象的状态可能发生改变时，应在内部调用此方法。
    /// </summary>
    /// <param name="stateFlags">与对象中可能已更改部分相对应的状态标志集。</param>
    protected void OnChanged(long stateFlags)
    {
        OnChanged(new ChangedEventArgs(stateFlags));
    }

    /// <summary>
    /// 当派生对象的状态可能发生改变，并且派生对象通过派生的 ChangedEventArgs 类来表示这种改变时，
    /// 可以在内部调用此方法。
    /// </summary>
    /// <param name="eventArgs">随更改事件一起触发的事件参数。</param>
    protected virtual void OnChanged(ChangedEventArgs eventArgs)
    {
        mHasChanged = true;
        _ChangedEvent.OnChanged(this, eventArgs);
    }

    /// <summary>
    /// 使用序列化数据初始化 SerializableChangedEventComponentBase 类的新实例。
    /// </summary>
    /// <param name="info">
    /// 包含序列化对象数据的对象。
    /// </param>
    /// <param name="context">
    /// 有关 SerializationInfo 的上下文信息。
    /// </param>
    protected SerializableChangedEventComponentBase(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// 初始化 SerializableChangedEventComponentBase 类的新实例。
    /// </summary>
    protected SerializableChangedEventComponentBase()
    {
    }
}
