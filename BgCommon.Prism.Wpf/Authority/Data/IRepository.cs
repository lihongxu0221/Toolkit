namespace BgCommon.Prism.Wpf.Authority.Data;

/// <summary>
/// 数据实体仓储接口（仓储模式核心接口）
/// 封装对数据实体的CRUD操作，隔离业务逻辑与数据访问层，提供统一的数据操作入口.
/// </summary>
/// <typeparam name="T">实体类型（必须是引用类型，对应数据库表映射的实体类）.</typeparam>
public interface IRepository<T>
    where T : class
{
    /// <summary>
    /// Gets 获取与此仓储关联的工作单元（Unit of Work）
    /// 用于统一管理多个仓储的事务，确保数据操作的原子性（所有操作要么全部成功，要么全部回滚）.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// 在内存中将实体标记为“已添加”（新增状态）
    /// 仅更新内存中的实体状态，不会立即写入数据库，需配合工作单元提交.
    /// </summary>
    /// <param name="entity">待添加的实体对象（必须是有效的实体实例，包含必要的业务字段）.</param>
    /// <returns>标记为“已添加”状态的实体对象（通常与入参实体为同一实例，便于链式调用）.</returns>
    T Add(T entity);

    /// <summary>
    /// 在内存中将实体标记为“已修改”（更新状态）
    /// 仅更新内存中的实体状态，不会立即写入数据库，需配合工作单元提交；支持实体部分字段修改.
    /// </summary>
    /// <param name="entity">待修改的实体对象（必须包含主键信息，否则无法定位数据库中的目标记录）.</param>
    /// <returns>标记为“已修改”状态的实体对象（通常与入参实体为同一实例，便于链式调用）.</returns>
    T Update(T entity);

    /// <summary>
    /// 在内存中将实体标记为“已删除”（删除状态）
    /// 仅更新内存中的实体状态，不会立即删除数据库记录，需配合工作单元提交；默认采用逻辑删除（若实体支持）.
    /// </summary>
    /// <param name="entity">待删除的实体对象（必须包含主键信息，否则无法定位数据库中的目标记录）.</param>
    /// <returns>标记为“已删除”状态的实体对象（通常与入参实体为同一实例，便于链式调用）.</returns>
    T Delete(T entity);

    /// <summary>
    /// 【同步】通过主键ID获取单个实体
    /// 主键需与实体类中配置的主键字段匹配（支持单主键，复合主键需使用其他重载方法）.
    /// </summary>
    /// <param name="id">实体的主键值（类型需与实体主键字段类型一致，否则会抛出类型转换异常）.</param>
    /// <returns>找到的实体对象；若未找到匹配主键的记录，返回 null.</returns>
    T? GetById(object id);

    /// <summary>
    /// 【同步】获取当前实体对应的所有数据库记录，返回列表
    /// 注意：若表数据量较大（如1000条以上），不建议使用此方法，可能导致内存占用过高，建议使用分页查询.
    /// </summary>
    /// <returns>包含所有实体的 List 集合；若表中无数据，返回空集合（非 null）.</returns>
    List<T> ListAll();

    /// <summary>
    /// 【同步】检查是否存在满足指定条件的实体记录
    /// 适用于快速判断条件匹配结果（如“是否存在已注册的手机号”），性能优于查询完整实体.
    /// </summary>
    /// <param name="predicate">查询条件表达式（Lambda表达式，如：entity => entity.Name == "测试"）.</param>
    /// <returns>true：存在满足条件的记录；false：不存在满足条件的记录.</returns>
    bool Any(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 【同步】获取满足指定条件的第一个实体，若未找到则返回 null
    /// 若存在多条满足条件的记录，仅返回排序后的第一条（默认按主键升序）.
    /// </summary>
    /// <param name="predicate">查询条件表达式（Lambda表达式，如：entity => entity.Id > 10）.</param>
    /// <returns>找到的第一个实体对象；若未找到满足条件的记录，返回 null.</returns>
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 【直接提交】异步添加一个实体并立即保存到数据库
    /// 操作会自动提交事务，无需额外调用工作单元的提交方法；适用于单实体新增场景.
    /// </summary>
    /// <param name="entity">待添加的实体对象（必须是有效的实体实例，包含必要的业务字段，主键若为自增则无需赋值）.</param>
    /// <returns>包含新增实体的 Task 任务（任务完成后返回已保存到数据库的实体，包含自动生成的主键等字段）.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// 【直接提交】异步更新一个实体并立即保存到数据库
    /// 操作会自动提交事务，无需额外调用工作单元的提交方法；支持实体部分字段修改.
    /// </summary>
    /// <param name="entity">待修改的实体对象（必须包含主键信息，否则无法定位数据库中的目标记录）.</param>
    /// <returns>包含更新后实体的 Task 任务（任务完成后返回已更新到数据库的实体对象）.</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// 【直接提交】异步删除一个实体并立即保存到数据库
    /// 操作会自动提交事务，无需额外调用工作单元的提交方法；默认采用逻辑删除（若实体支持）.
    /// </summary>
    /// <param name="entity">待删除的实体对象（必须包含主键信息，否则无法定位数据库中的目标记录）.</param>
    /// <returns>包含删除后实体的 Task 任务（任务完成后返回已标记为删除状态的实体对象）.</returns>
    Task<T> DeleteAsync(T entity);

    /// <summary>
    /// 【异步】通过主键ID获取单个实体
    /// 主键需与实体类中配置的主键字段匹配（支持单主键），异步操作不会阻塞当前线程.
    /// </summary>
    /// <param name="id">实体的主键值（类型需与实体主键字段类型一致，否则会抛出类型转换异常）.</param>
    /// <returns>包含实体对象的 Task 任务；任务完成后，若找到记录则返回实体，否则返回 null.</returns>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// 【异步】获取当前实体对应的所有数据库记录，返回列表
    /// 异步操作不会阻塞当前线程；注意：表数据量较大时不建议使用，建议使用分页查询接口（若有）.
    /// </summary>
    /// <returns>包含所有实体的 Task&lt;List&lt;T&gt;&gt; 任务；任务完成后返回实体列表，无数据则返回空集合（非 null）.</returns>
    Task<List<T>> ListAllAsync();

    /// <summary>
    /// 【异步】检查是否存在满足指定条件的实体记录
    /// 异步操作不会阻塞当前线程，适用于快速判断条件匹配结果，性能优于查询完整实体.
    /// </summary>
    /// <param name="predicate">查询条件表达式（Lambda表达式，如：entity => entity.Status == 1）.</param>
    /// <returns>包含布尔结果的 Task 任务；任务完成后返回 true（存在）或 false（不存在）.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 【异步】获取满足指定条件的第一个实体，若未找到则返回 null
    /// 异步操作不会阻塞当前线程；若存在多条满足条件的记录，仅返回排序后的第一条（默认按主键升序）.
    /// </summary>
    /// <param name="predicate">查询条件表达式（Lambda表达式，如：entity => entity.CreateTime > DateTime.Now.AddDays(-7)）.</param>
    /// <returns>包含实体对象的 Task 任务；任务完成后，若找到记录则返回实体，否则返回 null.</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 用于构建更复杂的查询（如多条件筛选、排序、分页、关联查询等）
    /// 返回的 IQueryable&lt;T&gt; 支持 LINQ 链式查询，最终会转换为 SQL 语句执行（延迟执行）.
    /// </summary>
    /// <returns>IQueryable&lt;T&gt; 接口实例，可用于后续拼接复杂查询条件.</returns>
    IQueryable<T> AsQueryable();
}