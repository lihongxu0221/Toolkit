namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 所有支持序列化的组件子类的基类。
/// </summary>
[Serializable]
public class SerializableComponentBase : Component, ISerializable, INotifyPropertyChanged, INotifyPropertyChanging
{
    [field:NonSerialized]
    public event PropertyChangedEventHandler? PropertyChanged;

    [field: NonSerialized]
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// 创建 SerializableComponentBase 类的新实例。
    /// </summary>
    protected SerializableComponentBase()
    {
    }

    /// <summary>
    /// 创建 SerializableComponentBase 类的新实例。
    /// </summary>
    /// <param name="info">
    /// 包含序列化对象数据的对象。
    /// </param>
    /// <param name="context">
    /// 有关 SerializationInfo 的上下文信息。
    /// </param>
    protected SerializableComponentBase(SerializationInfo info, StreamingContext context)
    {
        // 调用 SerializationSurrogate 的 SetObjectData 方法来设置对象数据
        SerializationSurrogate.SetObjectData(this, info, context);
    }

    // 实现 ISerializable 接口的 GetObjectData 方法
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // 将当前对象转换为 IHasChanged 接口类型
        IHasChanged? cogHasChanged = this as IHasChanged;
        // 检查对象是否实现了 IHasChanged 接口，并且上下文状态包含持久化状态
        if (cogHasChanged != null && (context.State & StreamingContextStates.Persistence) != 0)
        {
            // 如果满足条件，将对象的 HasChanged 属性设置为 false
            cogHasChanged.HasChanged = false;
        }

        // 调用自定义的 GetObjectData 方法
        GetObjectData(info, context);
    }

    /// <summary>
    /// 实现 ISerializable 接口的 GetObjectData 方法。
    /// </summary>
    /// <param name="info">包含序列化对象数据的对象。</param>
    /// <param name="context">有关 SerializationInfo 的上下文信息。</param>
    protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // 调用 SerializationSurrogate 的 GetObjectData 方法来获取对象数据
        SerializationSurrogate.GetObjectData(this, info, context);
    }

    /// <summary>
    /// 在序列化期间，设置字段值
    /// </summary>
    /// <param name="context"></param>
    [OnSerializing]
    protected virtual void OnSerializing(StreamingContext context)
    {
        //举例：在序列化期间，设置字段值
    }

    /// <summary>
    /// 序列化完成后，设置字段值
    /// </summary>
    /// <param name="context"></param>
    [OnSerialized]
    protected virtual void OnSerialized(StreamingContext context)
    {
        //举例：在序列化完成后，设置字段值
    }

    /// <summary>
    /// 在反序列化期间，为字段设置默认值 
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnDeserializedAttribute"/> 标记该方法在反序列化期间被调用</remarks>
    [OnDeserializing]
    protected virtual void OnDeserialing(StreamingContext context)
    {
        //举例：在反序列化期间，为字段设置默认值
    }

    /// <summary>
    /// 序列化完成后，设置字段值
    /// </summary>
    /// <param name="context">上下文</param>
    /// <remarks><see cref="OnDeserialized"/> 标记该方法在反序列化之后被调用</remarks>
    [OnDeserialized]
    protected virtual void OnDeserialized(StreamingContext context)
    {
        //举例：序列化完成后，设置字段值

    }

    /// <summary>
    /// 获取存档中记录的当前对象类型所在程序集的版本。
    /// 这对于对象检测自身旧的存档版本并以特殊方式手动反持久化旧存档可能很有用。
    /// 请注意，返回的 Version 对象可以使用其重载的比较运算符（如 &lt;、&gt; 等）与固定版本进行比较。
    /// </summary>
    /// <param name="info">包含存档数据的 SerializationInfo 对象。</param>
    /// <returns>创建给定存档的当前类型所在程序集的版本。</returns>
    /// <remarks>
    /// 注意：在反序列化期间请勿使用此属性，因为此属性的值未定义。
    /// </remarks>
    protected static Version GetArchivedAssemblyVersion(SerializationInfo info)
    {
        // 使用正则表达式匹配程序集名称中的版本信息
        Match match = Regex.Match(info.AssemblyName, "Version=([^,]*),");
        // 检查匹配是否成功且捕获组数量为 2
        if (match.Success && match.Groups.Count == 2)
        {
            // 如果匹配成功，根据匹配结果创建 Version 对象
            return new Version(match.Groups[1].Value);
        }
        // 如果匹配失败，返回一个默认的 Version 对象
        return new Version();
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, "e");
        this.PropertyChanged?.Invoke(this, e);
    }

    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, "e");
        this.PropertyChanging?.Invoke(this, e);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }
        OnPropertyChanging(propertyName);
        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(comparer, "comparer");
        if (comparer.Equals(field, newValue))
        {
            return false;
        }
        OnPropertyChanging(propertyName);
        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }
}