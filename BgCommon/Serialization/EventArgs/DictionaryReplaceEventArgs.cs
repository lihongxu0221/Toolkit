namespace BgCommon.Serialization;

/// <summary>
/// 该类作为事件参数传递给IDictionaryEvents接口的ReplacingItem（正在替换项）
/// 和ReplacedItem（已替换项）事件处理程序
/// </summary>
public class DictionaryReplaceEventArgs : EventArgs
{
    /// <summary>
    /// 被替换项的键
    /// </summary>
    public readonly object Key;

    /// <summary>
    /// 被替换项的旧值
    /// </summary>
    public readonly object? OldValue;

    /// <summary>
    /// 替换后的新值
    /// </summary>
    public readonly object? NewValue;

    /// <summary>
    /// 构造DictionaryReplaceEventArgs类的新实例
    /// </summary>
    /// <param name="key">被替换项的键</param>
    /// <param name="oldValue">被替换前的旧值</param>
    /// <param name="newValue">替换后的新值</param>
    public DictionaryReplaceEventArgs(object key, object? oldValue, object? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }
}