namespace BgCommon.Prism.Wpf.Modules.Parameters.Models;

/// <summary>
/// 参数查询请求.
/// </summary>
public sealed class ParameterQueryRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterQueryRequest"/> class.
    /// </summary>
    /// <param name="rootComponent">所属实例.</param>
    /// <param name="rootNodes">参数列表.</param>
    public ParameterQueryRequest(object rootComponent, params PropertyNodeItem[] rootNodes)
    {
        this.RootComponent = rootComponent;
        this.RootNodes = new ObservableCollection<PropertyNodeItem>();
        _ = this.RootNodes.AddRange(rootNodes);
    }

    /// <summary>
    /// Gets 所属VM.
    /// </summary>
    public object RootComponent { get; }

    /// <summary>
    /// Gets 参数列表.
    /// </summary>
    public ObservableCollection<PropertyNodeItem> RootNodes { get; }

    /// <summary>
    /// 清空.
    /// </summary>
    public void Clear()
    {
        this.RootNodes?.Clear();
    }
}

/// <summary>
/// 参数根节点.
/// </summary>
public class PropertyNodeItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyNodeItem"/> class.
    /// </summary>
    /// <param name="propetyName">参数名称.</param>
    /// <param name="propetyValue">参数实例.</param>
    public PropertyNodeItem(string propetyName, object propetyValue)
    {
        this.PropetyName = propetyName;
        this.PropetyValue = propetyValue;
    }

    /// <summary>
    /// Gets 参数名称.
    /// </summary>
    public string PropetyName { get; }

    /// <summary>
    /// Gets 参数实例.
    /// </summary>
    public object PropetyValue { get; }
}