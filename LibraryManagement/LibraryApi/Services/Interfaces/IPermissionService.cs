using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface IPermissionService
{
    Task<List<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission> CreateAsync(Permission permission);
    Task<bool> UpdateAsync(int id, Permission updatedPermission);
    Task<bool> DeleteAsync(int id);
    Task<List<UserPermission>> GetUserPermissionsAsync(string userId);
    Task<bool> GrantAsync(string userId, int permissionId);
    Task<bool> RevokeAsync(string userId, int permissionId);
}
