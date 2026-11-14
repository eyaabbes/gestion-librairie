using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Services.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IUserPermissionRepository userPermissionRepository,
        UserManager<ApplicationUser> userManager)
    {
        _permissionRepository = permissionRepository;
        _userPermissionRepository = userPermissionRepository;
        _userManager = userManager;
    }

    public Task<List<Permission>> GetAllAsync() => _permissionRepository.GetAllAsync();

    public Task<Permission?> GetByIdAsync(int id) => _permissionRepository.GetByIdAsync(id);

    public async Task<Permission> CreateAsync(Permission permission)
    {
        if (await _permissionRepository.GetByNameAsync(permission.Name) is not null)
            throw new InvalidOperationException("Une permission avec ce nom existe déjà.");

        permission.Id = 0;
        permission.UserPermissions = new List<UserPermission>();
        await _permissionRepository.AddAsync(permission);
        return permission;
    }

    public async Task<bool> UpdateAsync(int id, Permission updatedPermission)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission is null) return false;

        var duplicate = await _permissionRepository.GetByNameAsync(updatedPermission.Name);
        if (duplicate is not null && duplicate.Id != id)
            throw new InvalidOperationException("Une permission avec ce nom existe déjà.");

        permission.Name = updatedPermission.Name;
        permission.Description = updatedPermission.Description;
        await _permissionRepository.UpdateAsync(permission);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission is null) return false;

        await _userPermissionRepository.DeleteByPermissionIdAsync(id);
        await _permissionRepository.DeleteAsync(permission);
        return true;
    }

    public async Task<List<string>> GetUserPermissionNamesAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is null)
            throw new KeyNotFoundException("Utilisateur introuvable.");
        return await _userPermissionRepository.GetPermissionNamesByUserIdAsync(userId);
    }

    public async Task<bool> GrantAsync(string userId, int permissionId)
    {
        if (await _userManager.FindByIdAsync(userId) is null)
            throw new KeyNotFoundException("Utilisateur introuvable.");
        if (await _permissionRepository.GetByIdAsync(permissionId) is null)
            throw new KeyNotFoundException("Permission introuvable.");
        if (await _userPermissionRepository.GetAsync(userId, permissionId) is null)
            await _userPermissionRepository.AddAsync(new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            });
        return true;
    }

    public async Task<bool> RevokeAsync(string userId, int permissionId)
    {
        if (await _userManager.FindByIdAsync(userId) is null)
            throw new KeyNotFoundException("Utilisateur introuvable.");
        var assignment = await _userPermissionRepository.GetAsync(userId, permissionId);
        if (assignment is null) return false;
        await _userPermissionRepository.DeleteAsync(assignment);
        return true;
    }
}
