namespace LibraryApi.DTOs.Statistics;

public class FinancialStatisticsDto
{
    public decimal PaidSubscriptionRevenue { get; set; }
    public decimal PaidPenaltyRevenue { get; set; }
    public decimal OutstandingPenalties { get; set; }
    public decimal TotalRevenue { get; set; }
}
