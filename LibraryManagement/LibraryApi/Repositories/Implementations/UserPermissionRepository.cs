using LibraryApi.Data;
using LibraryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories.Implementations;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public UserPermissionRepository(ApplicationDbContext context) => _context = context;

    public Task<List<UserPermission>> GetByUserIdAsync(string userId) =>
        _context.UserPermissions
            .AsNoTracking()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .OrderBy(up => up.Permission!.Name)
            .ToListAsync();

    public Task<UserPermission?> GetAsync(string userId, int permissionId) =>
        _context.UserPermissions.FirstOrDefaultAsync(up =>
            up.UserId == userId && up.PermissionId == permissionId);

    public async Task AddAsync(UserPermission userPermission)
    {
        await _context.UserPermissions.AddAsync(userPermission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserPermission userPermission)
    {
        _context.UserPermissions.Remove(userPermission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByPermissionIdAsync(int permissionId)
    {
        var assignments = await _context.UserPermissions
            .Where(up => up.PermissionId == permissionId)
            .ToListAsync();
        _context.UserPermissions.RemoveRange(assignments);
        await _context.SaveChangesAsync();
    }
}
