namespace BgCommon.Prism.Wpf.Authority.Data;

/// <summary>
/// EF数据实体仓储实现类（基于Entity Framework Core）.
/// </summary>
/// <typeparam name="T">数据实体类型.</typeparam>
public class EFRepository<T> : IRepository<T>, IUnitOfWork
    where T : class
{
    private readonly AuthorityDbContextSQLite context;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">仓储Sqlite上下文实例.</param>
    public EFRepository(AuthorityDbContextSQLite dbContext)
    {
        this.context = dbContext;
    }

    /// <summary>
    /// Gets UnitOfWork 属性返回自身，因为仓储和工作单元是同一个类.
    /// </summary>
    public IUnitOfWork UnitOfWork => this;

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public int SaveChanges()
    {
        return context.SaveChanges();
    }

    /// <inheritdoc/>
    public T Add(T entity)
    {
        return context.Set<T>().Add(entity).Entity;
    }

    /// <inheritdoc/>
    public T Update(T entity)
    {
        return context.Set<T>().Update(entity).Entity;
    }

    /// <inheritdoc/>
    public T Delete(T entity)
    {
        return context.Set<T>().Remove(entity).Entity;
    }

    /// <inheritdoc/>
    public async Task<T> AddAsync(T entity)
    {
        var result = await context.Set<T>().AddAsync(entity);
        return result.Entity;
    }

    /// <inheritdoc/>
    public async Task<T> UpdateAsync(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public async Task<T> DeleteAsync(T entity)
    {
        T result = context.Set<T>().Remove(entity).Entity;
        return await Task.FromResult(result);
    }

    /// <inheritdoc/>
    public T? GetById(object id)
    {
        return context.Set<T>().Find(id);
    }

    /// <inheritdoc/>
    public List<T> ListAll()
    {
        return context.Set<T>().ToList();
    }

    /// <inheritdoc/>
    public bool Any(Expression<Func<T, bool>> predicate)
    {
        return context.Set<T>().Any(predicate);
    }

    /// <inheritdoc/>
    public T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return context.Set<T>().FirstOrDefault(predicate);
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(object id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<List<T>> ListAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().AnyAsync(predicate);
    }

    /// <inheritdoc/>
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    /// <inheritdoc/>
    public IQueryable<T> AsQueryable()
    {
        return context.Set<T>();
    }
}