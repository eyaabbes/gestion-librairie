namespace LibraryApi.Models;

public class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Le compte utilisateur est facultatif
    public string? ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }
}