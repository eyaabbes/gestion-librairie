namespace LibraryApi.DTOs.Statistics;

public class MonthlyStatisticsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Loans { get; set; }
    public int NewSubscriptions { get; set; }
    public decimal SubscriptionRevenue { get; set; }
    public decimal PenaltyRevenue { get; set; }
    public decimal TotalRevenue => SubscriptionRevenue + PenaltyRevenue;
}
