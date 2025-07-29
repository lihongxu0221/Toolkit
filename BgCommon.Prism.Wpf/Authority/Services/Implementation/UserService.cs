using BgCommon.Prism.Wpf.Authority.Data;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BgCommon.Prism.Wpf.Authority.Services;

public class UserService : IUserService
{
    private readonly AuthorityDbContextSqlite _context; // 用于事务
    private readonly IAuthService _authService;
    private readonly IRepository<UserInfo> _userRepo;
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly ILoggingService _logger;

    public UserService(
        AuthorityDbContextSqlite context,
        IAuthService authService,
        IRepository<UserInfo> userRepo,
        IRepository<UserRole> userRoleRepo,
        ILoggingService logger)
    {
        this._context = context;
        this._authService = authService;
        this._userRepo = userRepo;
        this._userRoleRepo = userRoleRepo;
        this._logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> RegisterAsync(string userName, string password, int roleId)
    {
        // 注册是特殊操作，权限通常在调用方UI层控制，此处省略内部校验
        await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (await _userRepo.AnyAsync(u => u.UserName == userName))
            {
                return AuthorityResult.ToFail(Ioc.GetString("用户名 '{0}' 已存在。", userName));
            }

            // 警告：密码应哈希处理！
            var newUser = new UserInfo(userName, password);
            _ = _context.Users.Add(newUser);

            var userRole = new UserRole { User = newUser, RoleId = roleId };
            _ = _context.UserRoles.Add(userRole);

            _ = await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _logger.LogOperationAsync(newUser.Id, newUser.UserName, PermissionCodes.UserCreate, Ioc.GetString("新用户注册成功"));
            return AuthorityResult.ToSuccess(1, Ioc.GetString("用户注册成功"), newUser);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return AuthorityResult.ToFail(Ioc.GetString("注册失败: {0}", ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> UpdateUserInfoAsync(UserInfo userToUpdate, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.UserEdit);
        if (!authResult.Success)
        {
            return authResult;
        }

        UserInfo? existingUser = await _userRepo.GetByIdAsync(userToUpdate.Id);
        if (existingUser == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户不存在"));
        }

        existingUser.UserName = userToUpdate.UserName; // 更新所需字段
        existingUser.IsActive = userToUpdate.IsActive;

        // 不允许通过此方法更新密码
        await _userRepo.UpdateAsync(existingUser);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.UserEdit, Ioc.GetString("更新了用户'{0}'的信息", existingUser.UserName));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("用户信息更新成功"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> DeleteUserAsync(int userIdToDelete, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.UserDelete);
        if (!authResult.Success)
        {
            return authResult;
        }

        UserInfo? userToDelete = await _userRepo.GetByIdAsync((long)userIdToDelete);
        if (userToDelete == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户不存在"));
        }

        await _userRepo.DeleteAsync(userToDelete);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.UserDelete, Ioc.GetString("删除了用户'{0}'", userToDelete.UserName));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("用户删除成功"));
    }

    public async Task<AuthorityResult<List<UserInfo>>> GetAllUsersAsync()
    {
        List<UserInfo> users = await _userRepo.AsQueryable().Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        return AuthorityResult<List<UserInfo>>.ToSuccess(1, Ioc.GetString("获取用户列表成功"), users);
    }
}