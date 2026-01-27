namespace BgCommon.DbService.DbProviders;

/// <summary>
/// 数据保存上下文.
/// </summary>
public interface IAuthorityDbContext
{
    /// <summary>
    /// 保存已经发生的数据变更.
    /// </summary>
    /// <returns>返回成功影响的数据行📚.</returns>
    public int SaveChanges();

    /// <summary>
    /// 保存已经发生的数据变更(异步).
    /// </summary>
    /// <param name="cancellationToken">取消令牌.</param>
    /// <returns>返回成功影响的数据行📚.</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public IRepository<T> Set<T>() where T : class;
}