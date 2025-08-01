namespace BgCommon.Serialization;

/// <summary>
/// 该类作为事件参数传递给IDictionaryEvents接口的RemovingItem（正在移除项）
/// 和RemoveItem（已移除项）事件处理程序
/// </summary>
public class DictionaryRemoveEventArgs : DictionaryInsertEventArgs
{
    /// <summary>
    /// 构造DictionaryRemoveEventArgs类的新实例
    /// </summary>
    /// <param name="key">被移除项的键</param>
    /// <param name="value">被移除项的值</param>
    public DictionaryRemoveEventArgs(object key, object? value)
        : base(key, value)
    {
    }
}