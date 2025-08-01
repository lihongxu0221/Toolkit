namespace BgCommon.Serialization.Implementation.Internal;

public abstract class KeyValuePairEventArgs : KeyEventArgs
{
    /// <summary>
    /// 被插入项的值
    /// </summary>
    public readonly object Item;

    protected KeyValuePairEventArgs(object item, object key)
        : base(key)
    {
        Item = item;
    }
}