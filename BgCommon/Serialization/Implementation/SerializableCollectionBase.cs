using BgCommon.Serialization.Interop;

namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 一个通用的、可序列化的有序值集合的基类， 该集合会引发风格的更改事件。
/// </summary>  
[Serializable]
public abstract class SerializableCollectionBase : SerializableChangedEventBase, IList, ICollection, IEnumerable, ICloneable, ICollectionEvents
{
    private const long Sf0 = 1L;

    /// <summary>
    /// 项（索引器）属性的状态标志。
    /// </summary>
    public const long SfItem = 1L;

    /// <summary>
    /// Count 属性的状态标志。
    /// </summary>
    public const long SfCount = 2L;

    /// <summary>
    /// 供派生类使用的下一个状态标志。
    /// </summary>
    protected new const long SfNextSf = 4L;

    private ArrayList list;

    /// <summary>
    /// 获取包含元素列表的内部 ArrayList。
    /// </summary>
    protected virtual IList InnerList
    {
        get
        {
            if (list == null)
            {
                list = new ArrayList();
            }
            return list;
        }
    }

    /// <summary>
    /// 获取此集合的 IList 接口。
    /// </summary>
    protected virtual IList List => this;

    /// <summary>
    /// 获取集合中包含的元素数量。
    /// </summary>
    [FilterThreshold(FilterThresholdConstants.Typical)]
    public int Count => InnerList.Count;

    bool IList.IsReadOnly => InnerList.IsReadOnly;

    object? IList.this[int index]
    {
        get
        {
            if (index < 0 || index > InnerList.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return InnerList[index];
        }
        set
        {
            if (index < 0 || index > InnerList.Count)
            {
                throw new IndexOutOfRangeException();
            }
            object? oldValue = InnerList[index];
            OnReplacingItem(index, oldValue, value);
            InnerList[index] = value;
            OnReplacedItem(index, oldValue, value);
        }
    }

    public bool IsFixedSize => false;

    bool ICollection.IsSynchronized => InnerList.IsSynchronized;

    object ICollection.SyncRoot => InnerList.SyncRoot;

    /// <summary>
    /// 在集合被清空之前引发。
    /// </summary>
    public event EventHandler? Clearing;

    /// <summary>
    /// 在集合被清空之后引发。
    /// </summary>
    public event EventHandler? Cleared;

    /// <summary>
    /// 在插入项之前引发。
    /// </summary>
    public event CollectionInsertEventHandler? InsertingItem;

    /// <summary>
    /// 在插入项之后引发。
    /// </summary>
    public event CollectionInsertEventHandler? InsertedItem;

    /// <summary>
    /// 在移除项之前引发。
    /// </summary>
    public event CollectionRemoveEventHandler? RemovingItem;

    /// <summary>
    /// 在移除项之后引发。
    /// </summary>
    public event CollectionRemoveEventHandler? RemovedItem;

    /// <summary>
    /// 在替换项之前引发。
    /// </summary>
    public event CollectionReplaceEventHandler? ReplacingItem;

    /// <summary>
    /// 在替换项之后引发。
    /// </summary>
    public event CollectionReplaceEventHandler? ReplacedItem;

    /// <summary>
    /// 在项被移动到新索引之前引发。
    /// </summary>
    public event CollectionMoveEventHandler? MovingItem;

    /// <summary>
    /// 在项被移动到新索引之后引发。
    /// </summary>
    public event CollectionMoveEventHandler? MovedItem;

    /// <summary>
    /// 构造 CollectionBase 类的新实例。
    /// </summary>
    protected SerializableCollectionBase()
    {
    }

    /// <summary>
    /// 构造 CollectionBase 类的新实例，
    /// 该实例最初包含从提供的实例克隆的项。
    /// </summary>
    /// <param name="other">
    /// 其项将被克隆并添加到新集合中的集合。
    /// 如果一个项不支持 ICloned 接口，则将该项本身添加。
    /// </param>
    protected SerializableCollectionBase(SerializableCollectionBase other)
    {
        for (int i = 0; i < other.Count; i++)
        {
            object? obj = other.InnerList[i];
            if (obj is ICloneable cloneable)
            {
                try
                {
                    obj = cloneable.Clone();
                }
                catch (NotSupportedException)
                {
                    continue;
                }
            }

            ((IList)this).Add(obj);
        }
    }

    /// <summary>
    /// 特殊的序列化构造函数。
    /// </summary>
    /// <param name="info">
    /// 用于反序列化集合的数据。
    /// </param>
    /// 用于反序列化序列化信息的上下文。
    /// <param name="context"></param>
    protected SerializableCollectionBase(SerializationInfo info, StreamingContext context)
    {
        SerializationSurrogate.SetObjectData(this, info, context);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    /// <summary>
    /// 创建对象的深拷贝。必须在派生类中重写此方法。
    /// </summary>
    /// <returns>克隆实例的深拷贝。</returns>
    protected abstract object Clone();

    /// <summary>
    /// 引发 Clearing 事件。
    /// </summary>
    protected virtual void OnClearing()
    {
        if (Clearing != null)
        {
            Clearing(this, new EventArgs());
        }
    }

    /// <summary>
    /// 引发 Cleared 事件。
    /// </summary>
    protected virtual void OnCleared()
    {
        if (Cleared != null)
        {
            Cleared(this, new EventArgs());
        }
        OnChanged(3L);
    }

    /// <summary>
    /// 引发 InsertingItem 事件。
    /// </summary>
    /// <param name="index">项将被插入的索引。</param>
    /// <param name="value">要插入的项的值。</param>
    protected virtual void OnInsertingItem(int index, object? value)
    {
        if (InsertingItem != null)
        {
            InsertingItem(this, new CollectionInsertEventArgs(index, value));
        }
    }

    /// <summary>
    /// 引发 InsertedItem 事件。
    /// </summary>
    /// <param name="index">项已被插入的索引。</param>
    /// <param name="value">插入的项的值。</param>
    protected virtual void OnInsertedItem(int index, object? value)
    {
        InsertedItem?.Invoke(this, new CollectionInsertEventArgs(index, value));
        OnChanged(3L);
    }

    /// <summary>
    /// 引发 RemovingItem 事件。
    /// </summary>
    /// <param name="index">要移除的项的索引。</param>
    /// <param name="value">要移除的项的值。</param>
    protected virtual void OnRemovingItem(int index, object? value)
    {
        RemovingItem?.Invoke(this, new CollectionRemoveEventArgs(index, value));
    }

    /// <summary>
    /// 引发 RemovedItem 事件。
    /// </summary>
    /// <param name="index">已移除的项的索引。</param>
    /// <param name="value">已移除的项的值。</param>
    protected virtual void OnRemovedItem(int index, object? value)
    {
        RemovedItem?.Invoke(this, new CollectionRemoveEventArgs(index, value));
        OnChanged(3L);
    }

    /// <summary>
    /// 引发 ReplacingItem 事件。
    /// </summary>
    /// <param name="index">要被替换的项的索引。</param>
    /// <param name="oldValue">要被替换的项的值。</param>
    /// <param name="newValue">替换旧值的项的值。</param>
    protected virtual void OnReplacingItem(int index, object? oldValue, object? newValue)
    {
        ReplacingItem?.Invoke(this, new CollectionReplaceEventArgs(index, oldValue, newValue));
    }

    /// <summary>
    /// 引发 ReplacedItem 事件。
    /// </summary>
    /// <param name="index">已被替换的项的索引。</param>
    /// <param name="oldValue">已被替换的项的值。</param>
    /// <param name="newValue">替换了旧值的项的值。</param>
    protected virtual void OnReplacedItem(int index, object? oldValue, object? newValue)
    {
        ReplacedItem?.Invoke(this, new CollectionReplaceEventArgs(index, oldValue, newValue));
        OnChanged(1L);
    }

    /// <summary>
    /// 引发 MovingItem 事件。
    /// </summary>
    /// <param name="fromIndex">要被移动的项的索引。</param>
    /// <param name="toIndex">要被移动的项的目标索引。</param>
    /// <param name="value">被移动的项的值。</param>
    protected virtual void OnMovingItem(int fromIndex, int toIndex, object? value)
    {
        MovingItem?.Invoke(this, new CollectionMoveEventArgs(fromIndex, toIndex, value));
    }

    /// <summary>
    /// 引发 MovedItem 事件。
    /// </summary>
    /// <param name="fromIndex">被移动的项的源索引。</param>
    /// <param name="toIndex">被移动的项的新索引。</param>
    /// <param name="value">被移动的项的值。</param>
    protected virtual void OnMovedItem(int fromIndex, int toIndex, object? value)
    {
        MovingItem?.Invoke(this, new CollectionMoveEventArgs(fromIndex, toIndex, value));
        OnChanged(1L);
    }

    /// <summary>
    /// 从集合中移除所有对象。
    /// </summary>
    public void Clear()
    {
        OnClearing();
        InnerList.Clear();
        OnCleared();
    }

    /// <summary>
    /// 将一个项从一个位置移动到另一个位置。
    /// </summary>
    /// <param name="fromIndex">该项的原始索引。</param>
    /// <param name="toIndex">该项的新索引。</param>
    public void Move(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= Count)
        {
            throw new ArgumentException("集合索引越界", "fromIndex");
        }
        if (toIndex < 0 || toIndex >= Count)
        {
            throw new ArgumentException("集合索引越界", "toIndex");
        }
        if (fromIndex != toIndex)
        {
            object? value = InnerList[fromIndex];
            OnMovingItem(fromIndex, toIndex, value);
            InnerList.RemoveAt(fromIndex);
            InnerList.Insert(toIndex, value);
            OnMovedItem(fromIndex, toIndex, value);
        }
    }

    /// <summary>
    /// 移除指定索引处的元素。
    /// </summary>
    /// <param name="index">要移除的元素的从零开始的索引。</param>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= InnerList.Count)
        {
            throw new ArgumentOutOfRangeException();
        }
        object? value = InnerList[index];
        OnRemovingItem(index, value);
        InnerList.RemoveAt(index);
        OnRemovedItem(index, value);
    }

    /// <summary>
    /// 将集合的内容复制到一个数组中。
    /// </summary>
    /// <param name="array">要复制到的数组。</param>
    /// <param name="index">开始复制的索引。</param>
    public void CopyTo(Array array, int index)
    {
        InnerList.CopyTo(array, index);
    }

    void IList.Insert(int index, object? value)
    {
        if (index < 0 || index > InnerList.Count)
        {
            throw new ArgumentOutOfRangeException();
        }
        OnInsertingItem(index, value);
        InnerList.Insert(index, value);
        OnInsertedItem(index, value);
    }

    void IList.Remove(object? value)
    {
        int num = InnerList.IndexOf(value);
        if (num >= 0)
        {
            RemoveAt(num);
        }
    }

    bool IList.Contains(object? value)
    {
        return InnerList.Contains(value);
    }

    int IList.IndexOf(object? value)
    {
        return InnerList.IndexOf(value);
    }

    int IList.Add(object? value)
    {
        OnInsertingItem(InnerList.Count, value);
        int num = InnerList.Add(value);
        OnInsertedItem(num, value);
        return num;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }
}