using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Modules.Module.Views;
using BgCommon.Prism.Wpf.Authority.Modules.Role.Views;
using BgCommon.Prism.Wpf.Authority.Modules.User.Views;
using Microsoft.EntityFrameworkCore;
using ModuleInfo = BgCommon.Prism.Wpf.Authority.Entities.ModuleInfo;

namespace BgCommon.Prism.Wpf.Authority.Data;

public class AuthorityDbContextSqlite : DbContext
{
    public DbSet<UserInfo> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<ModuleInfo> Modules { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<OperationLog> OperationLogs { get; set; }

    public AuthorityDbContextSqlite(DbContextOptions<AuthorityDbContextSqlite> options)
        : base(options)
    {
        // 创建迁移: 在“包管理器控制台”中，将默认项目设为 AuthorityManagement.Core，然后运行：
        // Add-Migration InitialDatabaseSetup
        // ```    EF Core 会检查你的 `AuthorityDbContext`，生成创建数据库表的代码，并放入 `Core` 项目的 `Migrations` 文件夹。
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 定义 SQLite 数据库文件的路径和名称
        _ = optionsBuilder.UseSqlite("Data Source=Authority.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region 关系配置

        // ... 原有的唯一索引配置保持不变 ...
        _ = modelBuilder.Entity<UserInfo>().HasIndex(u => u.UserName).IsUnique();
        _ = modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        _ = modelBuilder.Entity<Permission>().HasIndex(p => p.Code).IsUnique();

        // --- 配置 UserRole (用户-角色) 多对多关系的连接表 ---
        // 1. 配置 UserRole
        _ = modelBuilder.Entity<UserRole>().HasKey(ur => ur.Id);

        // 2. 添加唯一索引来防止重复分配
        _ = modelBuilder.Entity<UserRole>()
                        .HasIndex(ur => new { ur.UserId, ur.RoleId })
                        .IsUnique();

        // 3. 定义与 User 的关系 (一个 User 有多个 UserRole)
        _ = modelBuilder.Entity<UserRole>()
                        .HasOne(ur => ur.User)
                        .WithMany(ur => ur.UserRoles)
                        .HasForeignKey(ur => ur.UserId);

        // 4. 定义与 Role 的关系 (一个 Role 有多个 UserRole)
        _ = modelBuilder.Entity<UserRole>()
                        .HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId);

        // --- 配置 RolePermission 连接表 ---
        // 1. 定义主键
        _ = modelBuilder.Entity<RolePermission>().HasKey(rp => rp.Id); // 使用 Id 作为主键

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
        #endregion

        #region 数据植入 (Seeding)

        // 1. 植入角色 (Roles)
        _ = modelBuilder.Entity<Role>().HasData(
            new Role { Id = 0, Authority = 99, Name = "超级管理员", Description = "拥有系统所有权限", Enabled = false },
            new Role { Id = 1, Authority = 98, Name = "管理员", Description = "拥有系统所有权限", Enabled = true },
            new Role { Id = 2, Authority = 2, Name = "工程师", Description = "拥有基本的查看和操作权限", Enabled = true },
            new Role { Id = 3, Authority = 1, Name = "操作员", Description = "拥有基本的查看和操作权限", Enabled = true }
        );

        // 2. 植入用户 (Users)
        // 重要提示：在生产环境中，密码绝不能以明文存储！
        // 必须使用安全的哈希算法（如 Argon2, BCrypt）进行加密。此处为演示方便使用明文。
        _ = modelBuilder.Entity<UserInfo>().HasData(
            new UserInfo { Id = 0, UserName = "超级管理员", Password = "666888", CreatedAt = DateTime.Now, IsActive = false, IsOnline = false },
            new UserInfo { Id = 1, UserName = "管理员", Password = "8888*", CreatedAt = DateTime.Now, IsActive = true, IsOnline = false },
            new UserInfo { Id = 2, UserName = "工程师", Password = "8888", CreatedAt = DateTime.Now, IsActive = true, IsOnline = false },
            new UserInfo { Id = 3, UserName = "操作员", Password = "123", CreatedAt = DateTime.Now, IsActive = true, IsOnline = false }
        );

        // 3. 植入用户与角色的关系 (UserRole)
        _ = modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1, AssignedAt = DateTime.Now },
            new UserRole { UserId = 2, RoleId = 2, AssignedAt = DateTime.Now },
            new UserRole { UserId = 3, RoleId = 3, AssignedAt = DateTime.Now }
        );

        // 4. 植入模块 (Modules)
        _ = modelBuilder.Entity<ModuleInfo>().HasData(
            new ModuleInfo { Id = 1000, Authority = 98, ParentId = -1, Name = "用户管理", TypeFullName = typeof(UserManagementView).AssemblyQualifiedName ?? string.Empty, IsEnabled = true, CreatedBy = "System", ModifiedBy = "System" },
            new ModuleInfo { Id = 1001, Authority = 98, ParentId = -1, Name = "角色管理", TypeFullName = typeof(RolePermissionView).AssemblyQualifiedName ?? string.Empty, IsEnabled = true, CreatedBy = "System", ModifiedBy = "System" },
            new ModuleInfo { Id = 1002, Authority = 98, ParentId = -1, Name = "模块管理", TypeFullName = typeof(ModuleManagementView).AssemblyQualifiedName ?? string.Empty, IsEnabled = true, CreatedBy = "System", ModifiedBy = "System" }
       );

        // 5. 植入权限项 (Permissions)
        // 用户管理相关权限
        _ = modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1001, ModuleId = 1000, Name = "查看用户", Code = "User.View" },   // 用户管理相关权限
            new Permission { Id = 1002, ModuleId = 1000, Name = "新增用户", Code = "User.Create" },
            new Permission { Id = 1003, ModuleId = 1000, Name = "编辑用户", Code = "User.Edit" },
            new Permission { Id = 1004, ModuleId = 1000, Name = "删除用户", Code = "User.Delete" }
        );

        // 角色权限管理相关权限
        _ = modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 2001, ModuleId = 1001, Name = "查看角色", Code = "Role.View" }
        );

        // 模块管理
        _ = modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 3001, ModuleId = 1002, Name = "模块管理", Code = "Module.View" },
            new Permission { Id = 3002, ModuleId = 1002, Name = "新增模块", Code = "Module.Create" },
            new Permission { Id = 3003, ModuleId = 1002, Name = "编辑模块", Code = "Module.Edit" },
            new Permission { Id = 3004, ModuleId = 1002, Name = "删除模块", Code = "Module.Delete" }
        );

        // 6. 植入角色与权限的关系 (RolePermission)
        // --- 管理员的权限 (拥有所有权限) ---
        _ = modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 1001 }, // User.View 
            new RolePermission { RoleId = 1, PermissionId = 1002 }, // User.Create
            new RolePermission { RoleId = 1, PermissionId = 1003 }, // User.Edit
            new RolePermission { RoleId = 1, PermissionId = 1004 }, // User.Delete
            new RolePermission { RoleId = 1, PermissionId = 2001 }, // Role.View
            new RolePermission { RoleId = 1, PermissionId = 3001 }, // Module.View
            new RolePermission { RoleId = 1, PermissionId = 3002 }, // Module.Create
            new RolePermission { RoleId = 1, PermissionId = 3003 }, // Module.Edit
            new RolePermission { RoleId = 1, PermissionId = 3004 } // Module.Delete
        );

        // --- 工程师的权限 (只有查看权限) ---
        _ = modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 2, PermissionId = 1001 }, // User.View
            new RolePermission { RoleId = 2, PermissionId = 2001 } // Role.View
        );

        // --- 操作员的权限 (只有查看权限) ---
        _ = modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 3, PermissionId = 1001 }, // User.View
            new RolePermission { RoleId = 3, PermissionId = 2001 } // Role.View
        );
        #endregion
    }
}
