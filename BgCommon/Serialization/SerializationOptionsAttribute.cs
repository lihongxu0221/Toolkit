namespace BgCommon.Serialization;

/// <summary>
/// 此特性应用于类型的选定字段，以表明基于SerializationOptionsConstants 枚举中的位标志，这些字段的序列化是可选的。
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SerializationOptionsAttribute : Attribute
{
    /// <summary>
    /// 该字段的序列化选项位。
    /// </summary>
    public readonly SerializationOptionsConstants OptionBit;

    /// <summary>
    /// 创建 SerializationOptions 类的新实例。
    /// </summary>
    /// <param name="optionBit">SerializationOptionsConstants 枚举中的一个且仅一个成员（不包括 All 和 Minimum），表示该字段的选项位。</param>
    public SerializationOptionsAttribute(SerializationOptionsConstants optionBit)
    {
        if (optionBit != SerializationOptionsConstants.Results &&
            optionBit != SerializationOptionsConstants.InputImages &&
            optionBit != SerializationOptionsConstants.OutputImages &&
            optionBit != SerializationOptionsConstants.ExcludeDataBindings)
        {
            throw new ArgumentException("SerializationOptions 特性必须使用 SerializationOptionsConstants 枚举中的一个且仅一个成员（不包括 All 和 Minimum）进行初始化。");
        }

        OptionBit = optionBit;
    }
}