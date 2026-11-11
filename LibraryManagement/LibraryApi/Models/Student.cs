
namespace LibraryApi.Models;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Loan> Loans { get; set; }
        = new List<Loan>();
}
