using LibraryApi.Models;

namespace LibraryApi.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission?> GetByNameAsync(string name);
    Task AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
    Task DeleteAsync(Permission permission);
}
