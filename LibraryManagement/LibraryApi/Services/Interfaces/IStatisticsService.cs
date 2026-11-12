using LibraryApi.DTOs.Statistics;

namespace LibraryApi.Services.Interfaces;

public interface IStatisticsService
{
    Task<DashboardStatisticsDto> GetDashboardAsync();
    Task<LoanStatisticsDto> GetLoanStatisticsAsync();
    Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync();
    Task<FinancialStatisticsDto> GetFinancialStatisticsAsync();
    Task<List<MonthlyStatisticsDto>> GetMonthlyStatisticsAsync(int months = 12);
}
