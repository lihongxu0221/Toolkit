using BgCommon.Prism.Wpf.Authority.Data;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;

namespace BgCommon.Prism.Wpf.Authority.Services;

internal class RoleService : IRoleService
{
    private readonly IAuthService _authService;
    private readonly IRepository<Role> _roleRepo;
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly ILoggingService _logger;

    public RoleService(
        IAuthService authService,
        IRepository<Role> roleRepo,
        IRepository<UserRole> userRoleRepo,
        ILoggingService logger)
    {
        _authService = authService;
        _roleRepo = roleRepo;
        _userRoleRepo = userRoleRepo;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<Role>>> GetAllRolesAsync()
    {
        try
        {
            // 假设查看角色列表也需要权限
            // var authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.RoleView);
            // if (!authResult.Success) { return new AuthorityResult<List<Role>> { Success = false, Message = authResult.Message }; }
            List<Role> roles = await _roleRepo.ListAllAsync();
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
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.RoleCreate);
        if (!authResult.Success)
        {
            return authResult;
        }

        if (await _roleRepo.AnyAsync(r => r.Name == roleToAdd.Name))
        {
            return AuthorityResult.ToFail(Ioc.GetString("角色名称 '{0}' 已存在。", roleToAdd.Name));
        }

        _ = await _roleRepo.AddAsync(roleToAdd);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.RoleCreate, Ioc.GetString("新角色 '{0}' 已被创建。", roleToAdd.Name));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("新角色添加成功。"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> UpdateRoleAsync(Role roleToUpdate, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.RoleEdit);
        if (!authResult.Success)
        {
            return authResult;
        }

        Role? existingRole = await _roleRepo.GetByIdAsync(roleToUpdate.Id);
        if (existingRole == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("指定的角色不存在。"));
        }

        if (await _roleRepo.AnyAsync(r => r.Id != roleToUpdate.Id && r.Name == roleToUpdate.Name))
        {
            return AuthorityResult.ToFail(Ioc.GetString("角色名称 '{0}' 已存在。", roleToUpdate.Name));
        }

        existingRole.Name = roleToUpdate.Name;
        existingRole.Description = roleToUpdate.Description;
        existingRole.Enabled = roleToUpdate.Enabled;
        existingRole.Authority = roleToUpdate.Authority;

        await _roleRepo.UpdateAsync(existingRole);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.RoleEdit, Ioc.GetString("角色 '{0}' 的信息已被更新。", roleToUpdate.Name));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("角色信息更新成功。"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> DeleteRoleAsync(int roleIdToDelete, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.RoleDelete);
        if (!authResult.Success)
        {
            return authResult;
        }

        Role? roleToDelete = await _roleRepo.GetByIdAsync(roleIdToDelete);
        if (roleToDelete == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("指定的角色不存在。"));
        }

        if (await _userRoleRepo.AnyAsync(ur => ur.RoleId == roleIdToDelete))
        {
            return AuthorityResult.ToFail(Ioc.GetString("无法删除角色 '{0}'，因为它已被分配给一个或多个用户。", roleToDelete.Name));
        }

        await _roleRepo.DeleteAsync(roleToDelete);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.RoleDelete, Ioc.GetString("角色 '{0}' (ID: {1}) 已被删除。", roleToDelete.Name, roleIdToDelete));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("角色删除成功。"));
    }
}
