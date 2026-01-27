namespace BgCommon.Prism.Wpf.MVVM;

/// <summary>
/// 定义编辑操作的结果。
/// </summary>
public enum EditResult
{
    /// <summary>
    /// 更改已保存。
    /// </summary>
    Saved,

    /// <summary>
    /// 操作已取消。
    /// </summary>
    Cancelled
}

/// <summary>
/// 当一个实体被创建、更新或删除后发布的事件。
/// </summary>
/// <typeparam name="TEntity">实体的类型。</typeparam>
public class EntityChangedEvent<TEntity> : PubSubEvent<EntityChangedEventArgs<TEntity>>
    where TEntity : class
{
}

/// <summary>
/// 包含实体变更信息的事件参数。
/// </summary>
/// <typeparam name="TEntity">实体的类型。</typeparam>
public class EntityChangedEventArgs<TEntity>
    where TEntity : class
{
    public bool IsCreateNew { get; }

    public TEntity Entity { get; }

    public EditResult Result { get; }

    public EntityChangedEventArgs(EditResult result, TEntity entity, bool isCreateNew)
    {
        Result = result;
        Entity = entity;
        IsCreateNew = isCreateNew;
    }
}
