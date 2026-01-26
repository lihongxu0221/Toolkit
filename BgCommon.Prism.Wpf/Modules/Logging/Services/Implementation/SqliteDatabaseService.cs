using BgCommon.Prism.Wpf.Modules.Logging.Models;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace BgCommon.Prism.Wpf.Modules.Logging.Services.Implementation;

/// <summary>
/// 提供基于SQLite的日志数据库服务，实现日志的初始化、写入、查询和清理等功能.
/// </summary>
internal class SqliteDatabaseService : IDatabaseService
{
    private readonly string connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDatabaseService"/> class.
    /// 构造函数，从NLog配置或AppSettings中获取数据库连接字符串，并确保数据库目录存在.
    /// </summary>
    public SqliteDatabaseService()
    {
        // 从NLog配置（或AppSettings）获取连接字符串
        // 假设你在nlog.config中有 <variable name="dbPath" ... />
        string dbPath = LogManager.Configuration.Variables["dbPath"].Render(new LogEventInfo());
        string? directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        this.connectionString = $"Data Source={dbPath}"; // 例如："Data Source=logs/HistoricalLogs.sqlite"
    }

    /// <inheritdoc/>
    public bool InitializeDatabase()
    {
        bool result = false;
        try
        {
            using (var connection = new SqliteConnection(this.connectionString))
            {
                connection.Open();
                DbCommand command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Logs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Timestamp TEXT NOT NULL,
                        Level TEXT,
                        Source TEXT,
                        Message TEXT,
                        Exception TEXT
                    );
                    CREATE INDEX IF NOT EXISTS IDX_Logs_Timestamp ON Logs (Timestamp);
                    CREATE INDEX IF NOT EXISTS IDX_Logs_Source ON Logs (Source);
                ";
                result = command.ExecuteNonQuery() > 0;
            }

            BgLoggerSource.DataBase.Trace("Database initialized successfully at: {0}", this.connectionString.Replace("Data Source=", string.Empty));
        }
        catch (Exception ex)
        {
            BgLoggerSource.DataBase.Error(ex, "Error initializing database.");

            // 可以考虑重新抛出异常或优雅地处理
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> LogAsync(LogEntry entry)
    {
        using var connection = new SqliteConnection(this.connectionString);
        await connection.OpenAsync();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
                    INSERT INTO Logs (Timestamp, Level, Source, Message, Exception)
                    VALUES ($timestamp, $level, $source, $message, $exception);
                ";
        _ = command.Parameters.AddWithValue("$timestamp", entry.Timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff")); // ISO 8601
        _ = command.Parameters.AddWithValue("$level", entry.Level);
        _ = command.Parameters.AddWithValue("$source", entry.Source);
        _ = command.Parameters.AddWithValue("$message", entry.Message);
        _ = command.Parameters.AddWithValue("$exception", (object)entry.ExceptionInfo ?? DBNull.Value);
        int result = await command.ExecuteNonQueryAsync();
        return result > 0;
    }

    /// <inheritdoc/>
    public async Task<List<LogEntry>> GetLogsAsync(string sourceName, DateTime startDate, DateTime endDate, int limit = 1000)
    {
        var logs = new List<LogEntry>();
        using (var connection = new SqliteConnection(this.connectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();

            // 确保endDate包含当天的全部时间
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);

            if (string.IsNullOrEmpty(sourceName) || sourceName.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                command.CommandText =
                @"
                     SELECT Id, Timestamp, Level, Source, Message, Exception
                     FROM Logs
                     WHERE Timestamp >= @startDate
                       AND Timestamp <= @endDate
                     ORDER BY Timestamp DESC
                     LIMIT @limit;
                 ";
                _ = command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                _ = command.Parameters.AddWithValue("@endDate", endOfDay.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                _ = command.Parameters.AddWithValue("@limit", limit);
            }
            else
            {
                command.CommandText =
                @"
                    SELECT Id, Timestamp, Level, Source, Message, Exception
                    FROM Logs
                    WHERE (@sourceName IS NULL OR Source = @sourceName)
                      AND Timestamp >= @startDate
                      AND Timestamp <= @endDate
                    ORDER BY Timestamp DESC
                    LIMIT @limit;
                ";
                _ = command.Parameters.AddWithValue("@sourceName", sourceName);
                _ = command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                _ = command.Parameters.AddWithValue("@endDate", endOfDay.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                _ = command.Parameters.AddWithValue("@limit", limit);
            }

            using SqliteDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                logs.Add(new LogEntry
                {
                    Id = reader.GetInt64(0),
                    Timestamp = DateTime.Parse(reader.GetString(1)),
                    Level = reader.GetString(2),
                    Source = reader.GetString(3),
                    Message = reader.GetString(4),
                    ExceptionInfo = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                });
            }
        }

        return logs;
    }

    /// <inheritdoc/>
    public async Task<bool> CleanupOldLogsAsync(IEnumerable<LogSourceSetting>? settings, bool clearAll)
    {
        bool result = true;
        BgLoggerSource.DataBase.Trace("Starting log cleanup process...");
        if (settings != null)
        {
            using var connection = new SqliteConnection(this.connectionString);
            await connection.OpenAsync();
            foreach (LogSourceSetting setting in settings)
            {
                SqliteCommand command = connection.CreateCommand();
                try
                {
                    if (clearAll)
                    {
                        command.CommandText = @"DELETE FROM Logs WHERE Source = $source;VACUUM;";

                        // 清除所有日志条目，不考虑存储天数
                        _ = command.Parameters.AddWithValue("$source", setting.Name);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            BgLoggerSource.DataBase.Trace($"Cleaned up {rowsAffected} old log entries for source '{setting.Name}'.");
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if (setting.StorageDays > 0)
                        {
                            // 计算日志保留的截止日期
                            DateTime cutoffDate = DateTime.UtcNow.AddDays(-setting.StorageDays);
                            command.CommandText = @" DELETE FROM Logs WHERE Source = $source AND Timestamp < $cutoffDate; VACUUM;";
                            _ = command.Parameters.AddWithValue("$source", setting.Name);
                            _ = command.Parameters.AddWithValue("$cutoffDate", cutoffDate.ToString("yyyy/MM/dd HH:mm:ss.fff"));

                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
                                BgLoggerSource.DataBase.Trace($"Cleaned up {rowsAffected} old log entries for source '{setting.Name}' older than {cutoffDate:yyyy-MM-dd}.");
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            // 如果没有设置存储天数，则不进行清理
                            result = false;
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    BgLoggerSource.DataBase.Error(ex, $"Error cleaning up logs for source '{setting.Name}'.");
                }
            }
        }

        BgLoggerSource.DataBase.Trace("Log cleanup process finished.");
        return await Task.FromResult(result);
    }

    /// <summary>
    /// 异步获取历史日志文件大小.
    /// </summary>
    /// <returns>返回 历史日志文件大小.</returns>
    public async Task<long> GetHistoryFileSize()
    {
        long fileSize = 0L;
        string dbPath = LogManager.Configuration.Variables["dbPath"].Render(new LogEventInfo());
        if (File.Exists(dbPath))
        {
            fileSize = new FileInfo(dbPath).Length;
        }
        else
        {
            BgLoggerSource.DataBase.Warn("Log database file does not exist: {0}", dbPath);
        }

        return await Task.FromResult(fileSize);
    }
}