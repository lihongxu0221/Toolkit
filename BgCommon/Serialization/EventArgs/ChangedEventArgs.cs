namespace BgCommon.Serialization;

/// <summary>
/// Changed 事件参数
/// </summary>
public class ChangedEventArgs : EventArgs
{
    /// <summary>
    /// 获取与Changed事件关联的状态标志
    /// </summary>
    public readonly long StateFlags;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangedEventArgs"/> class.
    /// 构造ChangedEventArgs新实例
    /// </summary>
    /// <param name="stateFlags">
    /// 与Changed事件关联的状态标志值
    /// </param>
    public ChangedEventArgs(long stateFlags)
    {
        StateFlags = stateFlags;
    }

    /// <summary>
    /// 生成包含可能变更成员对应状态标志名称的字符串
    /// </summary>
    /// <param name="sender"> 触发Changed事件的对象实例 </param>
    /// <returns> 使用 | 符号分隔的状态标志名称字符串 </returns>
    public string GetStateFlagNames(object sender)
    {
        return GetStateFlagNames(sender.GetType(), StateFlags);
    }

    /// <summary>
    /// 根据对象类型和状态标志值生成对应的标志名称字符串
    /// </summary>
    /// <param name="senderType">
    /// 触发Changed事件的对象类型
    /// </param>
    /// <param name="stateFlags">
    /// Changed事件的状态标志值
    /// </param>
    /// <returns>
    /// 使用 | 符号分隔的状态标志名称字符串
    /// </returns>
    public static string GetStateFlagNames(Type senderType, long stateFlags)
    {
        StringBuilder stringBuilder = new StringBuilder();
        FieldInfo[] fields = senderType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        bool flag = false;
        foreach (FieldInfo fieldInfo in fields)
        {
            if (fieldInfo.Name.StartsWith("Sf") &&
                fieldInfo.FieldType == typeof(long) &&
                (stateFlags & (long)fieldInfo.GetValue(null)) != 0)
            {
                if (flag)
                {
                    _ = stringBuilder.Append("|");
                }

                _ = stringBuilder.Append(fieldInfo.Name);
                flag = true;
            }
        }

        return stringBuilder.ToString();
    }
}