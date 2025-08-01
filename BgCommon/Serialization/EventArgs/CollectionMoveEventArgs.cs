namespace BgCommon.Serialization;

/// <summary>
/// 移动项（MovingItem）和已移动项（MovedItem）事件的参数。
/// </summary>
public class CollectionMoveEventArgs : EventArgs
{
    /// <summary>
    /// 项移动前的索引。
    /// </summary>
    public readonly int FromIndex;

    /// <summary>
    /// 项移动后的索引。
    /// </summary>
    public readonly int ToIndex;

    /// <summary>
    /// 获取值。
    /// </summary>
    public readonly object? Value;

    /// <summary>
    /// 构造 CollectionMoveEventArgs 类的新实例。
    /// </summary>
    /// <param name="fromIndex">项移动前的索引。</param>
    /// <param name="toIndex">项移动后的索引。</param>
    /// <param name="value">移动项的值。</param>
    public CollectionMoveEventArgs(int fromIndex, int toIndex, object? value)
    {
        FromIndex = fromIndex;
        ToIndex = toIndex;
        Value = value;
    }
}