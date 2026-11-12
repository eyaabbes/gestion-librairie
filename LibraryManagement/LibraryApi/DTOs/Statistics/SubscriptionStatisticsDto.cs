namespace LibraryApi.DTOs.Statistics;

public class SubscriptionStatisticsDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int SubscriptionsStartedThisMonth { get; set; }
}
