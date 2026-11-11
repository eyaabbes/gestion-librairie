using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public Employee? Employee { get; set; }
    public ICollection<UserPermission> UserPermissions { get; set; }
    = new List<UserPermission>();
}