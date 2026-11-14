namespace LibraryApi.Repositories.Interfaces;

public interface IUserPermissionRepository
{
    Task<List<string>> GetPermissionNamesByUserIdAsync(string userId);
    Task<UserPermission?> GetAsync(string userId, int permissionId);
    Task AddAsync(UserPermission userPermission);
    Task DeleteAsync(UserPermission userPermission);
    Task DeleteByPermissionIdAsync(int permissionId);
}
