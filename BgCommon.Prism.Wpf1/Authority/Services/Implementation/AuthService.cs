using BgCommon.Prism.Wpf.Authority.Data;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using Microsoft.EntityFrameworkCore;
using ModuleInfo = BgCommon.Prism.Wpf.Authority.Entities.ModuleInfo;

namespace BgCommon.Prism.Wpf.Authority.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<UserInfo> _userRepo;
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly IRepository<RolePermission> _rolePermRepo;
    private readonly IRepository<Role> _roleRepo; // 需要访问角色信息
    private readonly IRepository<ModuleInfo> _moduleRepo; // 需要访问模块信息
    private readonly ILoggingService _logger;

    public AuthService(
        IRepository<UserInfo> userRepo,
        IRepository<UserRole> userRoleRepo,
        IRepository<RolePermission> rolePermRepo,
        IRepository<Role> roleRepo,
        IRepository<ModuleInfo> moduleRepo,
        ILoggingService logger)
    {
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _rolePermRepo = rolePermRepo;
        _roleRepo = roleRepo;
        _moduleRepo = moduleRepo;
        _logger = logger;
    }

    public async Task<AuthorityResult> LoginAsync(string userName, string password)
    {
        UserInfo? user = await _userRepo.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户不存在。"));
        }

        // 警告：此处未对密码进行哈希校验，存在严重安全风险！
        if (user.Password != password)
        {
            return AuthorityResult.ToFail(Ioc.GetString("密码错误。"));
        }

        if (!user.IsActive)
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户已被禁用，请联系管理员。"));
        }

        user.IsOnline = true;
        await _userRepo.UpdateAsync(user);
        await _logger.LogOperationAsync(user.Id, user.UserName, "User.Login", Ioc.GetString("用户 '{0}' 成功登录。", userName));
        return AuthorityResult<UserInfo>.ToSuccess(1, Ioc.GetString("登录成功"), user);
    }

    public async Task<AuthorityResult> LogoutAsync(long userId)
    {
        UserInfo? user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户不存在。"));
        }

        user.IsOnline = false;
        await _userRepo.UpdateAsync(user);
        await _logger.LogOperationAsync(user.Id, user.UserName, "User.Logout", Ioc.GetString("用户 '{0}' 已注销。", user.UserName));
        return AuthorityResult.ToSuccess(0, Ioc.GetString("注销成功"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> VerifyPermissionAsync(long operatorUserId, string requiredPermissionCode)
    {
        List<int> roleIds = await _userRoleRepo.AsQueryable()
            .Where(ur => ur.UserId == operatorUserId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户未分配任何角色，无权执行操作。"));
        }

        var hasPermission = await _rolePermRepo.AsQueryable()
            .Include(rp => rp.Permission)
            .AnyAsync(rp => roleIds.Contains(rp.RoleId) &&
                            rp.Permission != null && rp.Permission.Code == requiredPermissionCode);

        if (hasPermission)
        {
            return AuthorityResult.ToSuccess(1, "权限验证通过");
        }
        else
        {
            return AuthorityResult.ToFail(Ioc.GetString("没有执行 '{0}' 操作的权限。", requiredPermissionCode));
        }
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> VerifyAsync(UserInfo operatorUser, long moduleId, string? actionCode = "")
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(actionCode))
            {
                // Case 1: 检查具体的操作权限 (细粒度)
                return await VerifyActionPermission(operatorUser, moduleId, actionCode);
            }
            else
            {
                // Case 2: 检查模块的访问权限 (粗粒度)
                return await VerifyModuleAccess(operatorUser, moduleId);
            }
        }
        catch (Exception ex)
        {
            return AuthorityResult.ToFail(Ioc.GetString("权限验证时发生内部错误: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 辅助方法：校验用户对模块的访问权 (通过 Authority 等级).
    /// </summary>
    private async Task<AuthorityResult> VerifyModuleAccess(UserInfo operatorUser, long moduleId)
    {
        // 1. 查找模块所需权限等级
        ModuleInfo? module = await _moduleRepo.GetByIdAsync(moduleId);
        if (module == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("验证失败：指定的模块ID '{0}' 不存在。", moduleId));
        }

        var requiredAuthority = module.Authority;

        // 2. 查找用户拥有的最高权限等级
        List<int> userRoleIds = await _userRoleRepo.AsQueryable()
            .Where(ur => ur.UserId == operatorUser.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!userRoleIds.Any())
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户 '{0}' 未分配任何角色，无权访问。", operatorUser.UserName));
        }

        var maxUserAuthority = await _roleRepo.AsQueryable()
            .Where(r => userRoleIds.Contains(r.Id) && r.Enabled)
            .MaxAsync(r => (int?)r.Authority) ?? 0; // 如果没有角色，默认为0

        // 3. 比较权限等级
        if (maxUserAuthority >= requiredAuthority)
        {
            return AuthorityResult.ToSuccess(1, "模块访问权限验证通过");
        }
        else
        {
            string errorMessage = Ioc.GetString("访问被拒绝。用户权限等级 ({0}) 低于模块要求等级 ({1})。", maxUserAuthority, requiredAuthority);
            return AuthorityResult.ToFail(errorMessage);
        }
    }

    /// <summary>
    /// 辅助方法：校验用户对模块内具体操作的权限 (通过 Permission.Code).
    /// </summary>
    private async Task<AuthorityResult> VerifyActionPermission(UserInfo operatorUser, long moduleId, string actionCode)
    {
        List<int> roleIds = await _userRoleRepo.AsQueryable()
            .Where(ur => ur.UserId == operatorUser.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return AuthorityResult.ToFail(Ioc.GetString("用户 '{0}' 未分配任何角色，无权执行操作。", operatorUser.UserName));
        }

        var hasPermission = await _rolePermRepo.AsQueryable()
            .Include(rp => rp.Permission) // 关键的 Include
            .AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.Permission != null &&
                rp.Permission.ModuleId == moduleId &&
                rp.Permission.Code == actionCode
            );

        if (hasPermission)
        {
            return AuthorityResult.ToSuccess(1, "操作权限验证通过");
        }
        else
        {
            string errorMessage = Ioc.GetString("用户 '{0}' 没有对模块ID '{1}' 执行 '{2}' 操作的权限。", operatorUser.UserName, moduleId, actionCode);
            return AuthorityResult.ToFail(errorMessage);
        }
    }
}
