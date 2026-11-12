namespace LibraryApi.DTOs.Statistics;

public class DashboardStatisticsDto
{
    public int TotalBooks { get; set; }
    public int TotalStudents { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal TotalRevenue { get; set; }
}
