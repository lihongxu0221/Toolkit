namespace BgCommon.Serialization;

/// <summary>
/// This attribute is applied to databindings fields in order to
/// indicate that the databinding fields should not be serialized if the
/// ExcludeDataBindings serialization option is set.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SerializationDataBindingAttribute : Attribute
{
}