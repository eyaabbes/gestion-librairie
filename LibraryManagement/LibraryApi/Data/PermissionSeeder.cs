
using LibraryApi.Models;

namespace LibraryApi.Data;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Liste des permissions disponibles
        var permissions = new List<Permission>
        {
            new Permission
            {
                Name = "Books.View",
                Description = "Consulter les livres"
            },
            new Permission
            {
                Name = "Books.Create",
                Description = "Ajouter des livres"
            },
            new Permission
            {
                Name = "Books.Edit",
                Description = "Modifier des livres"
            },
            new Permission
            {
                Name = "Books.Delete",
                Description = "Supprimer des livres"
            },
            new Permission
            {
                Name = "Students.View",
                Description = "Consulter les étudiants"
            },
            new Permission { Name = "Students.Create", Description = "Ajouter des étudiants" },
            new Permission { Name = "Students.Edit", Description = "Modifier des étudiants" },
            new Permission { Name = "Students.Delete", Description = "Supprimer des étudiants" },
            new Permission { Name = "Authors.View", Description = "Consulter les auteurs" },
            new Permission { Name = "Authors.Create", Description = "Ajouter des auteurs" },
            new Permission { Name = "Authors.Edit", Description = "Modifier des auteurs" },
            new Permission { Name = "Authors.Delete", Description = "Supprimer des auteurs" },
            new Permission { Name = "Categories.View", Description = "Consulter les catégories" },
            new Permission { Name = "Categories.Create", Description = "Ajouter des catégories" },
            new Permission { Name = "Categories.Edit", Description = "Modifier des catégories" },
            new Permission { Name = "Categories.Delete", Description = "Supprimer des catégories" },
            new Permission
            {
                Name = "Loans.View",
                Description = "Consulter les emprunts"
            },
            new Permission
            {
                Name = "Loans.Create",
                Description = "Enregistrer un emprunt"
            },
            new Permission
            {
                Name = "Loans.Return",
                Description = "Enregistrer un retour"
            },
            new Permission
            {
                Name = "Employees.View",
                Description = "Consulter les employés"
            },
            new Permission { Name = "Employees.Create", Description = "Ajouter des employés et créer leurs comptes" },
            new Permission { Name = "Employees.Edit", Description = "Modifier des employés" },
            new Permission { Name = "Employees.Delete", Description = "Supprimer des employés" },
            new Permission
            {
                Name = "Reports.View",
                Description = "Consulter les statistiques"
            }
        };

        foreach (var permission in permissions)
        {
            bool exists = context.Permissions
                .Any(p => p.Name == permission.Name);

            if (!exists)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();
    }
}
