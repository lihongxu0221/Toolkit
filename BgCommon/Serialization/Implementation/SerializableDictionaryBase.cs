namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 可序列化字典基类，实现泛型非有序键值对集合，
/// 当集合发生任何更改时会触发事件通知。
/// 继承自SerializableChangedEventBase，支持序列化、克隆和字典操作。
/// </summary>
[Serializable]
public abstract class SerializableDictionaryBase : SerializableChangedEventBase, IDictionaryEvents, ICloneable, IDictionary, ICollection, IEnumerable, IDeserializationCallback
{
    // 状态标志常量定义
    private const long Sf0 = 1L;

    /// <summary>
    /// 索引器属性的状态标志
    /// </summary>
    public const long SfItem = 1L;

    /// <summary>
    /// Count属性的状态标志
    /// </summary>
    public const long SfCount = 2L;

    /// <summary>
    /// 派生类中应使用的下一个状态标志起始值
    /// </summary>
    protected new const long SfNextSf = 4L;

    private Hashtable hash; // 实际存储数据的哈希表

    /// <summary>
    /// 获取包含所有元素的哈希表（延迟初始化）
    /// </summary>
    protected Hashtable InnerHashtable
    {
        get
        {
            if (hash == null)
            {
                hash = new Hashtable();
            }
            return hash;
        }
    }

    /// <summary>
    /// 获取当前实例的IDictionary接口
    /// </summary>
    protected IDictionary Dictionary => this;

    /// <summary>
    /// 获取字典中元素的数量
    /// </summary>
    public int Count => InnerHashtable.Count;

    /// <summary>
    /// 获取字典中所有值的集合（设计时不可见）
    /// </summary>
    [Browsable(false)]
    public ICollection Values => InnerHashtable.Values;

    /// <summary>
    /// 获取字典中所有键的集合（设计时不可见）
    /// </summary>
    [Browsable(false)]
    public ICollection Keys => InnerHashtable.Keys;

    // IDictionary显式接口实现
    bool IDictionary.IsReadOnly => InnerHashtable.IsReadOnly;

    object? IDictionary.this[object key]
    {
        get => InnerHashtable[key];
        set
        {
            if (InnerHashtable.ContainsKey(key))
            {
                // 替换现有项流程
                object? oldValue = InnerHashtable[key];
                OnReplacingItem(key, oldValue, value);  // 触发替换前事件
                InnerHashtable[key] = value;
                OnReplacedItem(key, oldValue, value);   // 触发替换后事件
            }
            else
            {
                // 新增项流程
                OnInsertingItem(key, value);            // 触发插入前事件
                InnerHashtable[key] = value;
                OnInsertedItem(key, value);             // 触发插入后事件
            }
        }
    }

    bool IDictionary.IsFixedSize => InnerHashtable.IsFixedSize;
    bool ICollection.IsSynchronized => InnerHashtable.IsSynchronized;
    object ICollection.SyncRoot => InnerHashtable.SyncRoot;

    // 事件定义区域
    /// <summary>
    /// 在清空字典前触发的事件
    /// </summary>
    public event EventHandler? Clearing;

    /// <summary>
    /// 在清空字典后触发的事件
    /// </summary>
    public event EventHandler? Cleared;

    /// <summary>
    /// 在插入项前触发的事件
    /// </summary>
    public event DictionaryInsertEventHandler? InsertingItem;

    /// <summary>
    /// 在插入项后触发的事件
    /// </summary>
    public event DictionaryInsertEventHandler? InsertedItem;

    /// <summary>
    /// 在移除项前触发的事件
    /// </summary>
    public event DictionaryRemoveEventHandler? RemovingItem;

    /// <summary>
    /// 在移除项后触发的事件
    /// </summary>
    public event DictionaryRemoveEventHandler? RemovedItem;

    /// <summary>
    /// 在替换项后触发的事件（注意：实际应为替换前事件）
    /// </summary>
    public event DictionaryReplaceEventHandler? ReplacedItem;

    /// <summary>
    /// 在替换项前触发的事件（注意：实际应为替换后事件，可能存在命名错误）
    /// </summary>
    public event DictionaryReplaceEventHandler? ReplacingItem;

    /// <summary>
    /// 反序列化回调方法
    /// </summary>
    public void OnDeserialization(object? obj)
    {
        InnerHashtable.OnDeserialization(obj);
    }

    // 构造函数区域
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public SerializableDictionaryBase() { }

    /// <summary>
    /// 拷贝构造函数（深拷贝）
    /// </summary>
    /// <param name="source">要复制的源字典</param>
    public SerializableDictionaryBase(SerializableDictionaryBase source)
    {
        // 遍历源字典的键值对进行深拷贝
        IEnumerator keyEnumerator = source.Dictionary.Keys.GetEnumerator();
        IEnumerator valueEnumerator = source.Dictionary.Values.GetEnumerator();
        while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
        {
            object key = keyEnumerator.Current;
            object value = valueEnumerator.Current;

            // 对键和值进行克隆（如果支持ICloneable）
            if (key is ICloneable cloneableKey) key = cloneableKey.Clone();
            if (value is ICloneable cloneableValue) value = cloneableValue.Clone();

            Dictionary.Add(key, value);
        }
    }

    /// <summary>
    /// 反序列化构造函数
    /// </summary>
    /// <param name="info">序列化信息对象</param>
    /// <param name="context">流上下文</param>
    protected SerializableDictionaryBase(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    // 克隆方法实现
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// 创建对象的深拷贝（需在派生类中实现具体逻辑）
    /// </summary>
    /// <returns>克隆后的新对象实例</returns>
    protected abstract object Clone();

    // 事件触发方法区域
    /// <summary>
    /// 触发Clearing事件
    /// </summary>
    protected virtual void OnClearing()
    {
        Clearing?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 触发Cleared事件并通知状态变更
    /// </summary>
    protected virtual void OnCleared()
    {
        Cleared?.Invoke(this, EventArgs.Empty);
        OnChanged(3L);  // 假设3L表示Count和Items状态变化
    }

    /// <summary>
    /// 触发InsertingItem事件
    /// </summary>
    protected virtual void OnInsertingItem(object key, object? value)
    {
        InsertingItem?.Invoke(this, new DictionaryInsertEventArgs(key, value));
    }

    /// <summary>
    /// 触发InsertedItem事件并通知状态变更
    /// </summary>
    protected virtual void OnInsertedItem(object key, object? value)
    {
        InsertedItem?.Invoke(this, new DictionaryInsertEventArgs(key, value));
        OnChanged(3L);
    }

    /// <summary>
    /// 触发RemovingItem事件
    /// </summary>
    protected virtual void OnRemovingItem(object key, object? value)
    {
        RemovingItem?.Invoke(this, new DictionaryRemoveEventArgs(key, value));
    }

    /// <summary>
    /// 触发RemovedItem事件并通知状态变更
    /// </summary>
    protected virtual void OnRemovedItem(object key, object? value)
    {
        RemovedItem?.Invoke(this, new DictionaryRemoveEventArgs(key, value));
        OnChanged(3L);
    }

    /// <summary>
    /// 触发ReplacingItem事件
    /// </summary>
    protected virtual void OnReplacingItem(object key, object? oldValue, object? newValue)
    {
        ReplacingItem?.Invoke(this, new DictionaryReplaceEventArgs(key, oldValue, newValue));
    }

    /// <summary>
    /// 触发ReplacedItem事件并通知状态变更
    /// </summary>
    protected virtual void OnReplacedItem(object key, object? oldValue, object? newValue)
    {
        ReplacedItem?.Invoke(this, new DictionaryReplaceEventArgs(key, oldValue, newValue));
        OnChanged(1L);  // 假设1L表示Item属性变化
    }

    // 字典操作方法
    /// <summary>
    /// 将字典内容复制到数组中
    /// </summary>
    /// <param name="array">目标数组</param>
    /// <param name="index">起始索引</param>
    public void CopyTo(Array array, int index)
    {
        InnerHashtable.CopyTo(array, index);
    }

    /// <summary>
    /// 移除指定键的项（带事件通知）
    /// </summary>
    public void Remove(object key)
    {
        if (InnerHashtable.ContainsKey(key))
        {
            object? value = Dictionary[key];
            OnRemovingItem(key, value);  // 触发移除前事件
            InnerHashtable.Remove(key);
            OnRemovedItem(key, value);   // 触发移除后事件
        }
    }

    /// <summary>
    /// 检查是否包含指定键
    /// </summary>
    public bool Contains(object key) => InnerHashtable.Contains(key);

    /// <summary>
    /// 清空字典（带事件通知）
    /// </summary>
    public void Clear()
    {
        if (InnerHashtable.Count > 0)
        {
            OnClearing(); // 触发清空前事件
            InnerHashtable.Clear();
            OnCleared();// 触发清空后事件
        }
    }

    // 枚举器实现
    IDictionaryEnumerator IDictionary.GetEnumerator() => InnerHashtable.GetEnumerator();

    /// <summary>
    /// 显式接口实现的Add方法（实际通过索引器实现）
    /// </summary>
    void IDictionary.Add(object key, object? value) => Dictionary[key] = value;

    IEnumerator IEnumerable.GetEnumerator() => InnerHashtable.GetEnumerator();
}