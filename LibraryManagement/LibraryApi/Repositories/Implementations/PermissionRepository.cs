using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context) => _context = context;

    public Task<List<Permission>> GetAllAsync() =>
        _context.Permissions.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

    public Task<Permission?> GetByIdAsync(int id) =>
        _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Permission?> GetByNameAsync(string name) =>
        _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);

    public async Task AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Permission permission)
    {
        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
    }
}
