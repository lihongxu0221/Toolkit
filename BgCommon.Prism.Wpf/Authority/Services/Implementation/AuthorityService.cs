using BgCommon;
using BgCommon.Prism.Wpf.Authority.Data;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using Microsoft.EntityFrameworkCore;

namespace BgCommon.Prism.Wpf.Authority.Services;

/// <summary>
/// 权限校验服务实例.
/// </summary>
internal class AuthorityService : ObservableObject, IAuthorityService
{
    private readonly IContainerExtension container;
    private readonly IDialogService dialogService;
    private readonly AuthorityDbContextSqlite _context;

    /// <summary>
    /// Gets or sets a value indicating whether 是否处于调试模式.
    /// </summary>
    public bool IsDebugMode { get; set; }

    public AuthorityService(IContainerExtension container)
    {
        this.container = container;
        this.dialogService = container.Resolve<IDialogService>();
        this._context = container.Resolve<AuthorityDbContextSqlite>();
    }

    private bool TryFindUser(string username, out UserInfo? user)
    {
        user = null;
        if (_context.Users == null || _context.Users.Count() == 0)
        {
            return false;
        }

        user = _context.Users.FirstOrDefault(u => u.UserName == username);
        return user != null;
    }

    private bool TryFindUser(long userId, out UserInfo? user)
    {
        user = null;
        if (_context.Users == null || _context.Users.Count() == 0)
        {
            return false;
        }

        user = _context.Users.FirstOrDefault(u => u.Id == userId);
        return user != null;
    }

    private IQueryable<Entities.ModuleInfo> GetAuthorizedModulesQuery(long userId)
    {
        IQueryable<int> roleIds = _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId);

        IQueryable<int> permissionIds = _context.RolePermissions
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .Select(rp => rp.PermissionId);

        IQueryable<long> moduleIds = _context.Permissions
                    .Where(p => p.ModuleId.HasValue && permissionIds.Contains(p.Id))
                    .Select(p => p.ModuleId.Value)
                    .Distinct();

        return _context.Modules.Where(m => moduleIds.Contains(m.Id) && m.IsEnabled)
                               .AsNoTracking();
    }

    /// <inheritdoc/>
    public async Task LogOperationAsync(long? userId, string username, string actionType, string details)
    {
        try
        {
            var log = new OperationLog
            {
                UserId = (int?)userId,
                Username = username,
                ActionType = actionType,
                Details = details,
                Timestamp = DateTime.Now,
                IpAddress = string.Empty,
            };
            _ = _context.OperationLogs.Add(log);
            _ = await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // 如果记录日志本身失败，将其输出到调试窗口，避免因日志功能异常导致主功能失败.
            Debug.WriteLine($"关键操作日志记录失败: {ex.Message}");
            throw;
        }
    }

    #region User Management APIs
    public async Task<AuthorityResult> LoginAsync(string userName, string password)
    {
        try
        {
            UserInfo? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("登陆失败: 用户{0}不存在", userName));
            }

            // 重要提示：在生产环境中，绝不能以明文存储和比较密码！
            // 必须使用安全的哈希算法（如 Argon2, BCrypt）进行加密和验证。
            if (user.Password != password)
            {
                return AuthorityResult.ToFail(Ioc.GetString("登陆失败: 用户名或密码错误"));
            }

            if (!user.IsActive)
            {
                return AuthorityResult.ToFail(Ioc.GetString("登陆失败: 用户已被禁用，请联系管理员"));
            }

            // 登陆成功，加载用户的完整权限信息
            UserInfo? fullUserInfo = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.Permissions)
                            .ThenInclude(rp => rp.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (fullUserInfo == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("登陆失败: 无法加载用户详细信息"));
            }

            user.IsOnline = true;
            _ = _context.Users.Update(user);
            _ = await _context.SaveChangesAsync();

            await LogOperationAsync(user.Id, user.UserName, "User.Login", Ioc.GetString("用户 {0} 登录成功", userName));
            return AuthorityResult.ToSuccess(1, Ioc.GetString("用户 {0} 登录成功", userName), fullUserInfo);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Login Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("登陆失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> LogoutAsync(long userId)
    {
        try
        {
            UserInfo? user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("注销失败: 用户编号 '{0}' 对应的用户不存在", userId));
            }

            user.IsOnline = false;
            _ = _context.Users.Update(user);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            await LogOperationAsync(user.Id, user.UserName, "User.Logout", Ioc.GetString("用户 '{0}' 已注销", user.UserName));
            return AuthorityResult.ToSuccess(1, Ioc.GetString("注销成功"));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Logout Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("注销失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> RegisterAsync(string userName, string password, Role role)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userName))
            {
                return AuthorityResult.ToFail(Ioc.GetString("用户名 '{0}' 已存在", userName));
            }

            // Role? dbRole = await _context.Roles.FindAsync(role.Id);
            // if (dbRole == null)
            // {
            //     return AuthorityResult.ToFail(Ioc.GetString("指定角色不存在"));
            // }

            // var newUser = new UserInfo(userName, password);
            // newUser.UserRoles.Add(new UserRole { RoleId = dbRole.Id, AssignedAt = DateTime.Now });

            // _ = await _context.Users.AddAsync(newUser);
            // _ = await _context.SaveChangesAsync();

            // --- 数据变更 1: 创建用户 ---
            var newUser = new UserInfo(userName, password);
            _ = _context.Users.Add(newUser);
            _ = await _context.SaveChangesAsync(); // 必须先保存以获取 newUser.Id

            // --- 数据变更 2: 分配角色 ---
            var userRole = new UserRole
            {
                UserId = (int)newUser.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.Now
            };

            _ = _context.UserRoles.Add(userRole);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            string logDetails = Ioc.GetString("新用户 '{0}' 已注册并被分配了 '{1}' 角色", userName, role.Name);
            await LogOperationAsync(newUser.Id, newUser.UserName, "User.Create", logDetails);

            return AuthorityResult.ToSuccess(1, Ioc.GetString("用户{0}注册成功", userName), newUser);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Register Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("注册失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> UpdateUserInfoAsync(UserInfo user, UserInfo operatorUser)
    {
        try
        {
            UserInfo? existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("未找到要更新的用户"));
            }

            // 更新用户的标量属性
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "User.Update", Ioc.GetString("用户信息 '{0}' 已更新", user.UserName));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("用户{0}信息更新成功", user.UserName), user);
        }
        catch (DbUpdateException ex) // 更具体的异常处理
        {
            Debug.WriteLine($"[Update User Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("更新失败: {0}", ex.InnerException?.Message ?? ex.Message));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Update User Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("更新失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> DeleteUserAsync(int userId, UserInfo operatorUser)
    {
        try
        {
            UserInfo? user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("删除失败: 用户编码 '{0}' 对应的用户不存在", userId));
            }

            _ = _context.Users.Remove(user);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            // 删除操作由系统执行，操作人可以是调用者，这里简化为 "System"
            string logDetails = Ioc.GetString("用户 '{0}' (ID: {1}) 已被删除", user.UserName, userId);
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "User.Delete", logDetails);

            return AuthorityResult.ToSuccess(1, Ioc.GetString("用户{0}已成功删除", user.UserName));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Delete User Failed] {ex}");
            return AuthorityResult.ToFail(Ioc.GetString("删除失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<UserInfo>>> GetAllUsersAsync()
    {
        try
        {
            List<UserInfo>? users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .ToListAsync();

            return AuthorityResult<List<UserInfo>>.ToSuccess(0, Ioc.GetString("获取用户列表成功"), users);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Get All Users Failed] {ex}");
            return AuthorityResult<List<UserInfo>>.ToFail(Ioc.GetString("获取用户列表失败: {0}", ex.Message));
        }
    }

    #endregion

    #region ModuleInfo Management APIs

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAllModulesAsync(UserInfo user)
    {
        try
        {
            List<Entities.ModuleInfo> modules = await GetAuthorizedModulesQuery(user.Id)
                .ToListAsync();
            return AuthorityResult<List<Entities.ModuleInfo>>.ToSuccess(0, Ioc.GetString("获取模块列表成功"), modules);
        }
        catch (Exception ex)
        {
            return AuthorityResult<List<Entities.ModuleInfo>>.ToFail(Ioc.GetString("获取模块列表失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<Entities.ModuleInfo>>> GetModulesAsync(UserInfo user, long? parentId)
    {
        try
        {
            List<Entities.ModuleInfo> modules = await GetAuthorizedModulesQuery(user.Id)
                .Where(m => m.ParentId == parentId)
                .ToListAsync();
            return AuthorityResult<List<Entities.ModuleInfo>>.ToSuccess(0, Ioc.GetString("获取子模块成功"), modules);
        }
        catch (Exception ex)
        {
            return AuthorityResult<List<Entities.ModuleInfo>>.ToFail(Ioc.GetString("获取子模块失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> VerifyAsync(UserInfo user, long moduleId, string? action)
    {
        try
        {
            var hasPermission = false;
            if (string.IsNullOrWhiteSpace(action))
            {
                hasPermission = await GetAuthorizedModulesQuery(user.Id).AnyAsync(m => m.Id == moduleId);

                return AuthorityResult.ToFail(Ioc.GetString("没有模块 '{0}' 的操作权限", moduleId));
            }
            else
            {
                List<int> roleIds = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                hasPermission = await _context.RolePermissions
                    .AnyAsync(rp => roleIds.Contains(rp.RoleId) &&
                                    rp.Permission != null && rp.Permission.ModuleId == moduleId &&
                                    rp.Permission.Code == action);

                if (!hasPermission)
                {
                    return AuthorityResult.ToFail(Ioc.GetString("没有执行  '{0}' 操作的权限", action));
                }
            }

            return AuthorityResult.ToSuccess(1, Ioc.GetString("权限验证通过"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("权限验证时发生错误: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 获取系统中的所有模块列表（用于管理）.
    /// </summary>
    public async Task<AuthorityResult<List<Entities.ModuleInfo>>> GetAllSystemModulesAsync()
    {
        try
        {
            List<Entities.ModuleInfo>? modules = await _context.Modules
                .AsNoTracking()
                .ToListAsync();
            return AuthorityResult<List<Entities.ModuleInfo>>.ToSuccess(1, Ioc.GetString("获取所有系统模块成功"), modules);
        }
        catch (Exception ex)
        {
            return AuthorityResult<List<Entities.ModuleInfo>>.ToFail(Ioc.GetString("获取模块列表失败: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 添加一个新模块.
    /// </summary>
    public async Task<AuthorityResult> AddModuleAsync(Entities.ModuleInfo moduleToAdd, UserInfo operatorUser)
    {
        try
        {
            // 校验模块名是否已存在
            if (await _context.Modules.AnyAsync(m => m.Name == moduleToAdd.Name))
            {
                return AuthorityResult.ToFail(Ioc.GetString("模块名称 '{0}' 已存在。", moduleToAdd.Name));
            }

            // 校验模块类型全名是否已存在
            if (await _context.Modules.AnyAsync(m => m.TypeFullName == moduleToAdd.TypeFullName))
            {
                return AuthorityResult.ToFail(Ioc.GetString("模块类型 '{0}' 已被注册。", moduleToAdd.TypeFullName));
            }

            // --- 数据变更 ---
            moduleToAdd.CreatedBy = operatorUser.UserName;
            moduleToAdd.ModifiedBy = operatorUser.UserName;
            moduleToAdd.CreatedAt = DateTime.Now;
            moduleToAdd.LastModifiedAt = DateTime.Now;

            _ = _context.Modules.Add(moduleToAdd);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Module.Create", Ioc.GetString("新模块 '{0}' 已被创建。", moduleToAdd.Name));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("新模块添加成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 更新一个已有的模块信息.
    /// </summary>
    public async Task<AuthorityResult> UpdateModuleAsync(Entities.ModuleInfo moduleToUpdate, UserInfo operatorUser)
    {
        try
        {
            Entities.ModuleInfo? existingModule = await _context.Modules.FindAsync(moduleToUpdate.Id);
            if (existingModule == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("指定的模块不存在。"));
            }

            // 校验更新后的名称是否与其他模块冲突
            if (await _context.Modules.AnyAsync(m => m.Id != moduleToUpdate.Id && m.Name == moduleToUpdate.Name))
            {
                return AuthorityResult.ToFail(Ioc.GetString("模块名称 '{0}' 已存在。", moduleToUpdate.Name));
            }

            // 校验更新后的类型全名是否与其他模块冲突
            if (await _context.Modules.AnyAsync(m => m.Id != moduleToUpdate.Id && m.TypeFullName == moduleToUpdate.TypeFullName))
            {
                return AuthorityResult.ToFail(Ioc.GetString("模块类型 '{0}' 已被注册。", moduleToUpdate.TypeFullName));
            }

            // --- 数据变更 ---
            // 更新修改人和修改时间
            moduleToUpdate.ModifiedBy = operatorUser.UserName;
            moduleToUpdate.LastModifiedAt = DateTime.Now;

            // 将更新的值应用到从数据库检索出的实体
            _context.Entry(existingModule).CurrentValues.SetValues(moduleToUpdate);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Module.Update", Ioc.GetString("模块 '{0}' 的信息已被更新。", moduleToUpdate.Name));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("模块信息更新成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 删除一个模块.
    /// </summary>
    public async Task<AuthorityResult> DeleteModuleAsync(long moduleIdToDelete, UserInfo operatorUser)
    {
        try
        {
            Entities.ModuleInfo? moduleToDelete = await _context.Modules.FindAsync(moduleIdToDelete);
            if (moduleToDelete == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("指定的模块不存在。"));
            }

            // 业务规则：如果模块包含子模块，则不允许删除
            if (await _context.Modules.AnyAsync(m => m.ParentId == moduleIdToDelete))
            {
                return AuthorityResult.ToFail(Ioc.GetString("无法删除模块 '{0}'，因为它包含子模块。", moduleToDelete.Name));
            }

            // 业务规则：如果模块已关联任何权限，则不允许删除
            if (await _context.Permissions.AnyAsync(p => p.ModuleId == moduleIdToDelete))
            {
                return AuthorityResult.ToFail(Ioc.GetString("无法删除模块 '{0}'，因为它已关联了权限项。", moduleToDelete.Name));
            }

            // --- 数据变更 ---
            _ = _context.Modules.Remove(moduleToDelete);
            _ = await _context.SaveChangesAsync();

            // --- 日志记录 ---
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Module.Delete", Ioc.GetString("模块 '{0}' (ID: {1}) 已被删除。", moduleToDelete.Name, moduleIdToDelete));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("模块删除成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    #endregion

    #region Role Management APIs

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<Role>>> GetAllRolesAsync()
    {
        try
        {
            List<Role> roles = await _context.Roles
                .AsNoTracking()
                .ToListAsync();
            return AuthorityResult<List<Role>>.ToSuccess(1, Ioc.GetString("获取角色列表成功"), roles);
        }
        catch (Exception ex)
        {
            return AuthorityResult<List<Role>>.ToFail(Ioc.GetString("获取角色列表失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> AddRoleAsync(Role roleToAdd, UserInfo operatorUser)
    {
        try
        {
            if (await _context.Roles.AnyAsync(r => r.Name == roleToAdd.Name))
            {
                return AuthorityResult.ToFail(Ioc.GetString("角色名称 '{0}' 已存在。", roleToAdd.Name));
            }

            _ = _context.Roles.Add(roleToAdd);
            _ = await _context.SaveChangesAsync();
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Role.Create", Ioc.GetString("新角色 '{0}' 已被创建。", roleToAdd.Name));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("新角色添加成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> UpdateRoleAsync(Role roleToUpdate, UserInfo operatorUser)
    {
        try
        {
            Role? existingRole = await _context.Roles.FindAsync(roleToUpdate.Id);
            if (existingRole == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("指定的角色不存在。"));
            }

            if (await _context.Roles.AnyAsync(r => r.Id != roleToUpdate.Id && r.Name == roleToUpdate.Name))
            {
                return AuthorityResult.ToFail(Ioc.GetString("角色名称 '{0}' 已存在。", roleToUpdate.Name));
            }

            _context.Entry(existingRole).CurrentValues.SetValues(roleToUpdate);
            _ = await _context.SaveChangesAsync();
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Role.Update", Ioc.GetString("角色 '{0}' 的信息已被更新。", roleToUpdate.Name));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("角色信息更新成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> DeleteRoleAsync(int roleId, UserInfo operatorUser)
    {
        try
        {
            Role? roleToDelete = await _context.Roles.FindAsync(roleId);
            if (roleToDelete == null)
            {
                return AuthorityResult.ToFail(Ioc.GetString("指定的角色不存在。"));
            }

            if (await _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId))
            {
                return AuthorityResult.ToFail(Ioc.GetString("无法删除角色 '{0}'，因为它已被分配给一个或多个用户。", roleToDelete.Name));
            }

            _ = _context.Roles.Remove(roleToDelete);
            _ = await _context.SaveChangesAsync();
            await LogOperationAsync(operatorUser.Id, operatorUser.UserName, "Role.Delete", Ioc.GetString("角色 '{0}' (ID: {1}) 已被删除。", roleToDelete.Name, roleId));

            return AuthorityResult.ToSuccess(1, Ioc.GetString("角色删除成功。"));
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("操作失败: {0}", ex.Message));
        }
    }

    #endregion
}