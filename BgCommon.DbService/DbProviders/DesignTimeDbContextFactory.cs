namespace BgCommon.DbService.DbProviders;

/// <summary>
/// 这个工厂类专门用于在“设计时” (例如，运行 Add-Migration 命令时)
/// 为 Entity Framework Core 工具提供一个正确配置的 DbContext 实例。
/// 它在应用程序正常运行时【不会】被调用.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthorityDbContextSQLite>
{
    /// <inheritdoc/>
    public AuthorityDbContextSQLite CreateDbContext(string[] args)
    {
        if (!Directory.Exists(DbFileNames.DbDirectory))
        {
            _ = Directory.CreateDirectory(DbFileNames.DbDirectory);
        }

        Debug.WriteLine("===== 设计时工厂日志 =====");
        Debug.WriteLine($"当前时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Debug.WriteLine($"数据库路径: {DbFileNames.DbDirectory}");
        Debug.WriteLine($"数据库连接字符串: {DbFileNames.ConnectionString}");
        Debug.WriteLine(DbFileNames.DbDirectory);

        var optionsBuilder = new DbContextOptionsBuilder<AuthorityDbContextSQLite>();
        _ = optionsBuilder.UseSqlite(DbFileNames.ConnectionString);

        return new AuthorityDbContextSQLite(optionsBuilder.Options);
    }
}