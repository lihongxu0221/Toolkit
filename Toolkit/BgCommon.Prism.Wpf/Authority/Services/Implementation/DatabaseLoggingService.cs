namespace BgCommon.Prism.Wpf.Authority.Services;

internal class DatabaseLoggingService : ILoggingService
{
    private readonly AuthorityDbContextSQLite context; // 日志服务直接写DbContext，简化事务

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseLoggingService"/> class.
    /// </summary>
    /// <param name="context">数据仓储上下文.</param>
    public DatabaseLoggingService(AuthorityDbContextSQLite context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task LogOperationAsync(long? operatorId, string operatorUsername, string actionType, string details)
    {
        try
        {
            var log = new OperationLog
            {
                UserId = (int?)operatorId,
                Username = operatorUsername,
                ActionType = actionType,
                Details = details,
                Timestamp = DateTime.Now
            };

            _ = await context.OperationLogs.AddAsync(log);
            _ = await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"关键操作日志记录失败: {ex.Message}");
            throw;
        }
    }
}