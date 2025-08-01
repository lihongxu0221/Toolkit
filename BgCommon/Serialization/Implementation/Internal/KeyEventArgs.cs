namespace BgCommon.Serialization.Implementation.Internal;

public abstract class KeyEventArgs : EventArgs
{
    /// <summary>
    /// 获取或设置与事件相关的键。
    /// </summary>
    public readonly object Key;

    /// <summary>
    /// 获取或设置一个值，指示是否已处理此事件。
    /// </summary>
    public bool Handled { get; set; } = true;

    protected KeyEventArgs(object key)
    {
        Key = key;
    }
}