namespace BgCommon.Prism.Wpf.Authority.Data;

/// <summary>
/// 数据库文件名称及路径配置类.
/// 统一管理数据库相关的文件名称、存储目录、完整路径及连接字符串，确保配置一致性.
/// </summary>
public class DbFileNames
{
    /// <summary>
    /// 数据库文件名称.
    /// 指定SQLite数据库的文件名，包含后缀.db.
    /// </summary>
    public static readonly string DbName = "Authority.db";

    /// <summary>
    /// 数据库存储目录路径.
    /// 拼接应用程序当前运行目录与DataDir文件夹，作为数据库文件的存储路径.
    /// </summary>
    public static readonly string DbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataDir");

    /// <summary>
    /// 数据库文件完整路径.
    /// 拼接数据库存储目录与数据库文件名，得到数据库文件的绝对路径.
    /// </summary>
    public static readonly string DbPath = Path.Combine(DbDirectory, DbName);

    /// <summary>
    /// 数据库连接字符串.
    /// 基于SQLite的连接字符串格式，使用数据库完整路径作为数据源.
    /// </summary>
    public static readonly string ConnectionString = $"Data Source={DbPath}";
}