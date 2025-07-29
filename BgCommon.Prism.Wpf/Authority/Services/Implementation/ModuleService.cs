using BgCommon.Prism.Wpf.Authority.Data;
using BgCommon.Prism.Wpf.Authority.Entities;
using BgCommon.Prism.Wpf.Authority.Models;
using Microsoft.EntityFrameworkCore;
using ModuleInfo = BgCommon.Prism.Wpf.Authority.Entities.ModuleInfo;

namespace BgCommon.Prism.Wpf.Authority.Services;

public class ModuleService : IModuleService
{
    private readonly IAuthService _authService;
    private readonly IRepository<ModuleInfo> _moduleRepo;
    private readonly IRepository<Permission> _permissionRepo;
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly IRepository<RolePermission> _rolePermRepo;
    private readonly ILoggingService _logger;

    public ModuleService(
        IAuthService authService,
        IRepository<ModuleInfo> moduleRepo,
        IRepository<Permission> permissionRepo,
        IRepository<UserRole> userRoleRepo,
        IRepository<RolePermission> rolePermRepo,
        ILoggingService logger)
    {
        _authService = authService;
        _moduleRepo = moduleRepo;
        _permissionRepo = permissionRepo;
        _userRoleRepo = userRoleRepo;
        _rolePermRepo = rolePermRepo;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<ModuleInfo>>> GetAllSystemModulesAsync(UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.ModuleView);
        if (!authResult.Success)
        {
            return new AuthorityResult<List<ModuleInfo>>
            {
                Success = false,
                Message = authResult.Message
            };
        }

        List<ModuleInfo> modules = await _moduleRepo.ListAllAsync();
        return AuthorityResult<List<ModuleInfo>>.ToSuccess(1, "获取所有系统模块成功", modules);
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> AddModuleAsync(ModuleInfo moduleToAdd, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.ModuleCreate);
        if (!authResult.Success)
        {
            return authResult;
        }

        if (await _moduleRepo.AnyAsync(m => m.Name == moduleToAdd.Name))
        {
            return AuthorityResult.ToFail(Ioc.GetString("模块名称 '{0}' 已存在。", moduleToAdd.Name));
        }

        if (await _moduleRepo.AnyAsync(m => m.TypeFullName == moduleToAdd.TypeFullName))
        {
            return AuthorityResult.ToFail(Ioc.GetString("模块类型 '{0}' 已被注册。", moduleToAdd.TypeFullName));
        }

        moduleToAdd.CreatedBy = operatorUser.UserName;
        moduleToAdd.ModifiedBy = operatorUser.UserName;
        moduleToAdd.CreatedAt = DateTime.Now;
        moduleToAdd.LastModifiedAt = DateTime.Now;

        _ = await _moduleRepo.AddAsync(moduleToAdd);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.ModuleCreate, Ioc.GetString("新模块 '{0}' 已被创建。", moduleToAdd.Name));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("新模块添加成功。"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> UpdateModuleAsync(ModuleInfo moduleToUpdate, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.ModuleEdit);
        if (!authResult.Success)
        {
            return authResult;
        }

        ModuleInfo? existingModule = await _moduleRepo.GetByIdAsync(moduleToUpdate.Id);
        if (existingModule == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("指定的模块不存在。"));
        }

        if (await _moduleRepo.AnyAsync(m => m.Id != moduleToUpdate.Id && m.Name == moduleToUpdate.Name))
        {
            return AuthorityResult.ToFail(Ioc.GetString("模块名称 '{0}' 已存在。", moduleToUpdate.Name));
        }

        if (await _moduleRepo.AnyAsync(m => m.Id != moduleToUpdate.Id && m.TypeFullName == moduleToUpdate.TypeFullName))
        {
            return AuthorityResult.ToFail(Ioc.GetString("模块类型 '{0}' 已被注册。", moduleToUpdate.TypeFullName));
        }

        existingModule.Name = moduleToUpdate.Name;
        existingModule.ModifiedBy = operatorUser.UserName;
        existingModule.LastModifiedAt = DateTime.Now;

        await _moduleRepo.UpdateAsync(existingModule);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.ModuleEdit, Ioc.GetString("模块 '{0}' 的信息已被更新。", moduleToUpdate.Name));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("模块信息更新成功。"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult> DeleteModuleAsync(long moduleIdToDelete, UserInfo operatorUser)
    {
        AuthorityResult authResult = await _authService.VerifyPermissionAsync(operatorUser.Id, PermissionCodes.ModuleDelete);
        if (!authResult.Success)
        {
            return authResult;
        }

        ModuleInfo? moduleToDelete = await _moduleRepo.GetByIdAsync(moduleIdToDelete);
        if (moduleToDelete == null)
        {
            return AuthorityResult.ToFail(Ioc.GetString("指定的模块不存在。"));
        }

        if (await _moduleRepo.AnyAsync(m => m.ParentId == moduleIdToDelete))
        {
            return AuthorityResult.ToFail(Ioc.GetString("无法删除模块 '{0}'，因为它包含子模块。", moduleToDelete.Name));
        }

        if (await _permissionRepo.AnyAsync(p => p.ModuleId == moduleIdToDelete))
        {
            return AuthorityResult.ToFail(Ioc.GetString("无法删除模块 '{0}'，因为它已关联了权限项。", moduleToDelete.Name));
        }

        await _moduleRepo.DeleteAsync(moduleToDelete);
        await _logger.LogOperationAsync(operatorUser.Id, operatorUser.UserName, PermissionCodes.ModuleDelete, Ioc.GetString("模块 '{0}' (ID: {1}) 已被删除。", moduleToDelete.Name, moduleIdToDelete));
        return AuthorityResult.ToSuccess(1, Ioc.GetString("模块删除成功。"));
    }

    /// <inheritdoc/>
    public async Task<AuthorityResult<List<ModuleInfo>>> GetAuthorizedModulesAsync(UserInfo user, long? parentId = null)
    {
        try
        {
            List<int> roleIds = await _userRoleRepo.AsQueryable()
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            List<long> moduleIds = await _rolePermRepo.AsQueryable()
                .Where(rp => roleIds.Contains(rp.RoleId) && rp.Permission != null && rp.Permission.ModuleId.HasValue)
                .Select(rp => rp.Permission.ModuleId.Value)
                .Distinct()
                .ToListAsync();

            IQueryable<ModuleInfo> query = _moduleRepo.AsQueryable()
                .Where(m => m.IsEnabled && moduleIds.Contains(m.Id));

            if (parentId.HasValue)
            {
                query = query.Where(m => m.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(m => m.ParentId == null || m.ParentId == -1);
            }

            List<ModuleInfo> modules = await query.ToListAsync();
            return AuthorityResult<List<ModuleInfo>>.ToSuccess(1, "获取授权模块成功", modules);
        }
        catch (Exception ex)
        {
            return AuthorityResult<List<ModuleInfo>>.ToFail(Ioc.GetString("获取授权模块失败: {0}", ex.Message));
        }
    }
}