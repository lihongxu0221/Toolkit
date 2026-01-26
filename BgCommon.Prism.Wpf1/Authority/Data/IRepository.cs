namespace BgCommon.Prism.Wpf.Authority.Data;

public interface IRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(object id);

    Task<List<T>> ListAllAsync();

    IQueryable<T> AsQueryable();

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

    Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
}