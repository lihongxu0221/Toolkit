namespace BgCommon.Serialization.Implementation;

/// <summary>
/// 该类对对象进行序列化和反序列化的方式，
/// 使得即使未来版本中对象的字段被添加或删除，
/// 在加载该对象时也不会引发错误。
/// </summary>
public class SerializationSurrogate : ISerializationSurrogate
{
    /// <summary>
    /// 使用序列化对象所需的数据填充提供的 SerializationInfo
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="info">需要填充数据的 SerializationInfo</param>
    /// <param name="context">此序列化的目标上下文</param>
    public static void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        GetObjectData(obj, info, context, null);
    }

    /// <summary>
    /// 使用序列化对象所需的数据填充提供的 SerializationInfo
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="info">需要填充数据的 SerializationInfo</param>
    /// <param name="context">此序列化的目标上下文</param>
    /// <param name="nonCE3DBase">
    /// 对象的基类，该类具有自己的非序列化实现。GetObjectData() 不会序列化此基类的私有成员。
    /// </param>
    public static void GetObjectData(object obj, SerializationInfo info, StreamingContext context, Type? nonCE3DBase)
    {
        SerializationOptionsContext? optionsContext = context.Context as SerializationOptionsContext;
        Type type = obj.GetType();
        while (type != typeof(object) && type != nonCE3DBase)
        {
            FieldInfo[] fields = type!.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.IsNotSerialized || optionsContext != null && !optionsContext.ShouldSerialize(fieldInfo) || fieldInfo.FieldType.IsSubclassOf(typeof(MulticastDelegate)))
                {
                    continue;
                }
                object? value = fieldInfo.GetValue(obj);
                if (value != null)
                {
                    Type type2 = value.GetType();
                    if (type2.IsClass && !type2.IsSerializable)
                    {
                        continue;
                    }
                }
                info.AddValue(fieldInfo.Name, value);
            }
            type = type.BaseType!;
            while (type != typeof(object) && type != nonCE3DBase && !type.IsSerializable)
            {
                type = type.BaseType!;
            }
        }
    }

    /// <summary>
    /// 使用 SerializationInfo 中的信息填充对象
    /// </summary>
    /// <param name="obj">要填充的对象</param>
    /// <param name="info">用于填充对象的信息</param>
    /// <param name="context">对象反序列化的源上下文</param>
    /// <returns>填充后的反序列化对象</returns>
    public static object? SetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        return SetObjectData(obj, info, context, null);
    }

    /// <summary>
    /// 使用 SerializationInfo 中的信息填充对象
    /// </summary>
    /// <param name="obj">要填充的对象</param>
    /// <param name="info">用于填充对象的信息</param>
    /// <param name="context">对象反序列化的源上下文</param>
    /// <param name="nonClassBase">对象的基类，该类具有自己的非序列化实现。 SetObjectData() 不会反序列化此基类的私有成员。
    /// </param>
    /// <returns>填充后的反序列化对象</returns>
    public static object? SetObjectData(object obj, SerializationInfo info, StreamingContext context, Type? nonClassBase)
    {
        SerializationOptionsContext? optionsContext = context.Context as SerializationOptionsContext;
        Type type = obj.GetType();
        while (type != typeof(object) && type != nonClassBase)
        {
            FieldInfo[] fields = type!.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.IsNotSerialized || fieldInfo.FieldType.IsSubclassOf(typeof(MulticastDelegate)))
                {
                    continue;
                }
                if ((optionsContext == null || optionsContext.ShouldSerialize(fieldInfo)) && HasMember(info, fieldInfo.Name))
                {
                    try
                    {
                        object? value = info.GetValue(fieldInfo.Name, fieldInfo.FieldType);
                        fieldInfo.SetValue(obj, value);
                        continue;
                    }
                    catch (Exception)
                    {
                    }
                }

                object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), inherit: false);
                if (customAttributes.Length > 0)
                {
                    DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)customAttributes[0];
                    fieldInfo.SetValue(obj, defaultValueAttribute.Value);
                }
            }

            type = type.BaseType!;
            while (type != typeof(object) && type != nonClassBase && !type.IsSerializable)
            {
                type = type.BaseType!;
            }
        }
        return null;
    }

    void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        GetObjectData(obj, info, context);
    }

    object? ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        return SetObjectData(obj, info, context);
    }

    /// <summary>
    /// 如果序列化的对象数据包含指定成员名称则返回true
    /// </summary>
    /// <param name="info">包含序列化对象数据的对象</param>
    /// <param name="memberName">要检查的成员名称</param>
    /// <returns></returns>
    internal static bool HasMember(SerializationInfo info, string memberName)
    {
        bool result = false;
        SerializationInfoEnumerator enumerator = info.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Name.Equals(memberName))
            {
                result = true;
                break;
            }
        }
        return result;
    }
}
