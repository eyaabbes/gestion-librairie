
namespace LibraryApi.Models;

public class Loan
{
    public int Id { get; set; }

    // Foreign Keys
    public int BookId { get; set; }

    public int StudentId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    // Navigation Properties
    public Book? Book { get; set; }

    public Student? Student { get; set; }
}
