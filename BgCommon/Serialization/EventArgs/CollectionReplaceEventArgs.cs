namespace BgCommon.Serialization;

/// <summary>
/// 替换项（ReplacingItem）和已替换项（ReplacedItem）事件的参数。
/// </summary>
public class CollectionReplaceEventArgs : EventArgs
{
    /// <summary>
    /// 被替换项的索引。
    /// </summary>
    public readonly int Index;

    /// <summary>
    /// 给定索引处的旧值。
    /// </summary>
    public readonly object? OldValue;

    /// <summary>
    /// 给定索引处的新值。
    /// </summary>
    public readonly object? NewValue;

    /// <summary>
    /// 构造 CollectionReplaceEventArgs 类的新实例。
    /// </summary>
    /// <param name="index">被替换项的索引。</param>
    /// <param name="oldValue">给定索引处的旧值。</param>
    /// <param name="newValue">给定索引处的新值。</param>
    public CollectionReplaceEventArgs(int index, object? oldValue, object? newValue)
    {
        Index = index;
        OldValue = oldValue;
        NewValue = newValue;
    }
}