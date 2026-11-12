namespace LibraryApi.DTOs.Statistics;

public class LoanStatisticsDto
{
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int ReturnedLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int LoansBorrowedThisMonth { get; set; }
}
