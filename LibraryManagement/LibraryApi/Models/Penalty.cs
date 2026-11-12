namespace LibraryApi.Models;

public class Penalty
{
    public int Id { get; set; }

    public int LoanId { get; set; }
    public Loan? Loan { get; set; }

    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public bool IsPaid { get; set; }
}
