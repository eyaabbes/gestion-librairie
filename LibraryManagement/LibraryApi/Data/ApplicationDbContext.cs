
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Database Tables dbset yaani ahna on va gerer collection taa lhaja heki
    public DbSet<Book> Books { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Student> Students { get; set; }

    public DbSet<Loan> Loans { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configuration des tables Identity
        base.OnModelCreating(builder);

        // Relation Employee - ApplicationUser relation 1 1
        builder.Entity<Employee>()
            .HasOne(e => e.ApplicationUser)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(e => e.ApplicationUserId)
            .IsRequired(false);

        // Clé primaire composée de UserPermission
        builder.Entity<UserPermission>()
            .HasKey(up => new { up.UserId, up.PermissionId });

        // Relation UserPermission - ApplicationUser
        builder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId);

        // Relation UserPermission - Permission
        builder.Entity<UserPermission>()
            .HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId);
    }
}
