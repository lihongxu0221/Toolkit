namespace BgCommon.Prism.Wpf.Authority.Data;

/// <summary>
/// SQLite 权限管理数据库上下文类.
/// 继承自 EF Core 的 DbContext，用于管理权限系统相关实体与 SQLite 数据库的交互，包含实体映射、关系配置及数据植入.
/// </summary>
public class AuthorityDbContextSQLite : DbContext
{
    /// <summary>
    /// Gets or sets 用户信息数据集，对应数据库中的 UserInfo 表.
    /// </summary>
    public DbSet<UserInfo> Users { get; set; }

    /// <summary>
    /// Gets or sets 角色数据集，对应数据库中的 Role 表.
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Gets or sets 权限数据集，对应数据库中的 Permission 表.
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Gets or sets 模块信息数据集，对应数据库中的 ModuleInfo 表.
    /// </summary>
    public DbSet<ModuleInfo> Modules { get; set; }

    /// <summary>
    /// Gets or sets 用户-角色关联数据集，对应数据库中的 UserRole 表（多对多关系连接表）.
    /// </summary>
    public DbSet<UserRole> UserRoles { get; set; }

    /// <summary>
    /// Gets or sets 角色-权限关联数据集，对应数据库中的 RolePermission 表（多对多关系连接表）.
    /// </summary>
    public DbSet<RolePermission> RolePermissions { get; set; }

    /// <summary>
    /// Gets or sets 操作日志数据集，对应数据库中的 OperationLog 表.
    /// </summary>
    public DbSet<OperationLog> OperationLogs { get; set; }

    /// <summary>
    /// Gets or sets 登录信息数据集，对应数据库中的 LoginInfo 表.
    /// </summary>
    public DbSet<LoginInfo> LoginInfos { get; set; }

    /// <summary>
    /// Gets or sets 系统参数数据集，对应数据库中的 SystemParameter 表.
    /// </summary>
    public DbSet<SystemParameter> SystemParameters { get; set; }

    /// <summary>
    /// Gets or sets 参数权限数据集，对应数据库中的 ParameterPermission 表.
    /// </summary>
    public DbSet<ParameterPermission> ParameterPermissions { get; set; }

    /// <summary>
    /// Gets or sets 参数约束数据集，对应数据库中的 ParameterConstraint 表.
    /// </summary>
    public DbSet<ParameterConstraint> ParameterConstraints { get; set; }

    /// <summary>
    /// Gets or sets 用户访问权限数据集，对应数据库中的 UserAccessRights 表.
    /// </summary>
    public DbSet<UserAccessRights> UserAccessRights { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorityDbContextSQLite"/> class.
    /// </summary>
    /// <param name="options">Sqlite 仓储上下文配置. </param>
    public AuthorityDbContextSQLite(DbContextOptions<AuthorityDbContextSQLite> options)
        : base(options)
    {
        // 创建迁移: 在“包管理器控制台”中，将默认项目设为 AuthorityManagement.Core，然后运行：
        // Add-Migration InitialDatabaseSetup
        // ```    EF Core 会检查你的 `AuthorityDbContext`，生成创建数据库表的代码，并放入 `Core` 项目的 `Migrations` 文件夹。
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     // 定义 SQLite 数据库文件的路径和名称
    //     _ = optionsBuilder.UseSqlite("Data Source=Authority.db");
    // }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 用户信息表
        // 1. 定义主键
        _ = modelBuilder.Entity<UserInfo>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        _ = modelBuilder.Entity<UserInfo>().HasIndex(u => u.UserName).IsUnique();

        // 角色信息表
        // 1. 定义主键
        _ = modelBuilder.Entity<Role>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        _ = modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();

        // 权限表
        // 1. 定义主键
        _ = modelBuilder.Entity<Permission>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // 2. 定义与 ModuleInfo 的关系 (一个 Permission 属于一个 ModuleInfo)
            _ = entity.HasIndex(p => p.Code).IsUnique();

            // 在这里明确定义关系和外键
            _ = entity.HasOne(p => p.Module) // 一个权限有一个模块
                      .WithMany(m => m.Permissions) // 一个模块有多个权限
                      .HasForeignKey(p => p.ModuleId); // 连接它们的外键是 'moduleId' 属性
        });

        // 模块信息表
        // 1. 定义主键
        _ = modelBuilder.Entity<ModuleInfo>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();

            _ = modelBuilder.Entity<ModuleInfo>().HasIndex(m => m.Code).IsUnique();

            // 【核心代码】
            // 1. HasMany(m => m.ChildModules)
            //    - 声明一个模块【有多个】子模块
            // 2. WithOne(m => m.ParentModule)
            //    - 声明每一个子模块【有一个】父模块
            // 3. HasForeignKey(m => m.ParentId)
            //    - 明确告诉EF Core，连接这两者的外键是 `ParentId` 属性
            // 4. OnDelete(DeleteBehavior.Restrict) (可选，但推荐)
            //    - 设置级联删除行为。Restrict 表示“如果一个模块有子模块，则不允许删除该模块”，
            //      这可以防止意外删除整个模块树，增强数据完整性。
            _ = entity.HasMany(m => m.ChildModules)
                      .WithOne(m => m.ParentModule)
                      .HasForeignKey(m => m.ParentId)
                      .OnDelete(DeleteBehavior.Restrict); // 或 .SetNull 如果您希望删除父模块时，子模块的ParentId变为 null
        });

        // --- 配置 UserRole (用户-角色) 多对多关系的连接表 ---
        // 1. 配置 UserRole
        _ = modelBuilder.Entity<UserRole>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // 2. 添加唯一索引来防止重复分配
        _ = modelBuilder.Entity<UserRole>()
                        .HasIndex(ur => new { ur.UserId, ur.RoleId })
                        .IsUnique();

        // 3. 定义与 User 的关系 (一个 User 有多个 UserRole)
        _ = modelBuilder.Entity<UserRole>()
                        .HasOne(ur => ur.User)
                        .WithMany(ur => ur.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .OnDelete(DeleteBehavior.Cascade); // 明确设置为级联删除

        // 4. 定义与 Role 的关系 (一个 Role 有多个 UserRole)
        _ = modelBuilder.Entity<UserRole>()
                        .HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId);

        // --- 配置 RolePermission 连接表 ---
        // 1. 定义主键
        _ = modelBuilder.Entity<RolePermission>(entity =>
        {
            // 指定 Id 是主键 (虽然 [Key] 已经做了，但这里可以重复)
            _ = entity.HasKey(e => e.Id);

            // 【显式声明】Id 属性的值在添加时由数据库生成
            _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // 2. 添加唯一索引来防止重复分配
        _ = modelBuilder.Entity<RolePermission>()
                        .HasIndex(ur => new { ur.RoleId, ur.PermissionId })
                        .IsUnique();

        // 3. 定义与 Role 的关系 (一个 Role 有多个 RolePermission)
        _ = modelBuilder.Entity<RolePermission>()
                        .HasOne(rp => rp.Role)
                        .WithMany(rp => rp.Permissions)
                        .HasForeignKey(rp => rp.RoleId);

        // 4. 定义与 Permission 的关系 (一个 Permission 有多个 RolePermission)
        _ = modelBuilder.Entity<RolePermission>()
                        .HasOne(rp => rp.Permission)
                        .WithMany(p => p.RolePermissions)
                        .HasForeignKey(rp => rp.PermissionId);

        // LoginInfo 的 UserName 字段添加唯一索引，确保每个用户只保存一条登录信息
        _ = modelBuilder.Entity<LoginInfo>()
            .HasIndex(li => li.UserName)
            .IsUnique();

        // 配置 SystemParameter
        _ = modelBuilder.Entity<SystemParameter>(entity =>
        {
            _ = entity.HasIndex(p => new { p.Code, p.ModuleId }).IsUnique();

            // 配置与 ParameterConstraint 的一对多关系
            _ = entity.HasMany(p => p.Constraints)
                      .WithOne(c => c.SystemParameter)
                      .HasForeignKey(c => c.ParameterId)
                      .OnDelete(DeleteBehavior.Cascade); // 如果删除了参数，级联删除其所有约束
        });

        // 配置 ParameterConstraint
        _ = modelBuilder.Entity<ParameterConstraint>(entity =>
        {
            // ParameterId 和 Type 的组合必须是唯一的，防止对一个参数重复设置同一种约束
            _ = entity.HasIndex(c => new { c.ParameterId, c.Type }).IsUnique();
        });

        // 配置 ParameterPermission
        _ = modelBuilder.Entity<ParameterPermission>(entity =>
        {
            // RoleId 和 ParameterId 的组合必须是唯一的，防止重复授权
            _ = entity.HasIndex(pp => new { pp.RoleId, pp.ParameterId }).IsUnique();

            // 配置与 Role 的关系
            _ = entity.HasOne(pp => pp.Role)
                      .WithMany() // 一个角色可以有多个参数权限
                      .HasForeignKey(pp => pp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade); // 如果删除了角色，级联删除其参数权限

            // 配置与 SystemParameter 的关系
            _ = entity.HasOne(pp => pp.SystemParameter)
                      .WithMany() // 一个参数可以被多个角色授权
                      .HasForeignKey(pp => pp.ParameterId)
                      .OnDelete(DeleteBehavior.Cascade); // 如果删除了参数，级联删除其权限设置
        });

        // 配置 用户权限.
        _ = modelBuilder.Entity<UserAccessRights>(entity =>
        {
            _ = entity.HasKey(ua => ua.Id);

            // UserId 和 AccessRight 的组合必须是唯一的，防止重复记录
            _ = entity.HasIndex(ua => new { ua.UserId, ua.RefObjType, ua.RefObjId })
                      .IsUnique();

            // 添加与 UserInfo 的关系配置
            _ = entity.HasOne(ua => ua.User)
                      .WithMany() // UserInfo 中没有导航属性指回 UserAccessRights
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // 明确设置为级联删除
        });
    }
}