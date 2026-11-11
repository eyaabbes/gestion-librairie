using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Loans;

public class CreateLoanDto
{
    [Required]
    public int BookId { get; set; }

    [Required]
    public int StudentId { get; set; }
}