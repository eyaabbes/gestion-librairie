
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
    public DbSet<Penalty> Penalties { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
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

        builder.Entity<Penalty>()
            .HasOne(p => p.Loan)
            .WithMany()
            .HasForeignKey(p => p.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Subscription>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Subscription>()
            .HasOne(s => s.SubscriptionPlan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SubscriptionPlan>()
            .HasIndex(p => p.Name)
            .IsUnique();

        builder.Entity<SubscriptionPlan>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        builder.Entity<Penalty>()
            .Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Entity<SubscriptionPayment>()
            .HasOne(p => p.Subscription)
            .WithMany()
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SubscriptionPayment>()
            .Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Entity<Book>()
            .Property(book => book.Price)
            .HasPrecision(10, 2);
    }
}
