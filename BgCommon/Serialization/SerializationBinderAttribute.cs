namespace BgCommon.Serialization;

/// <summary>
/// 此特性应用于程序集，用于选择非默认的绑定方案，
/// 例如将已加载对象升级到最新可用程序集版本的标准方案。
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class SerializationBinderAttribute : Attribute
{
    /// <summary>
    /// 所选的自定义绑定器，或者为 null。
    /// </summary>
    public readonly bool UseLatestVersionBinder;

    public readonly string CustomBinder = string.Empty;

    /// <summary>
    /// 创建 SerializationBinderAttribute 类的新实例。
    /// 表示将使用一个把类型升级到最新程序集版本的绑定器。
    /// </summary>
    public SerializationBinderAttribute()
    {
        UseLatestVersionBinder = true;
    }

    /// <summary>
    /// 创建 SerializationBinderAttribute 类的新实例。
    /// </summary>
    /// <param name="binder">此特性的自定义绑定器的完全限定类型名称。如果为 null，则应用默认的 .NET 绑定规则（通常表示需要并行版本控制）。</param>      
    public SerializationBinderAttribute(string binder)
    {
        UseLatestVersionBinder = false;
        CustomBinder = binder;
    }

    /// <summary>
    /// 创建 SerializationBinderAttribute 类的新实例。
    /// </summary>
    /// <param name="binder">此特性的自定义绑定器的完全限定类型名称。如果为 null，则应用默认的 .NET 绑定规则（通常表示需要并行版本控制）。</param> 
    /// <param name="useLatest">决定是否升级到最新的程序集版本。</param>
    public SerializationBinderAttribute(string binder, bool useLatest)
    {
        UseLatestVersionBinder = useLatest;
        CustomBinder = binder;
    }
}