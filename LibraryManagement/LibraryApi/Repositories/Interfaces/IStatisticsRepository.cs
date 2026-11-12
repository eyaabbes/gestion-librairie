using LibraryApi.DTOs.Statistics;

namespace LibraryApi.Repositories.Interfaces;

public interface IStatisticsRepository
{
    Task<DashboardStatisticsDto> GetDashboardAsync(DateTime now);
    Task<LoanStatisticsDto> GetLoanStatisticsAsync(DateTime now);
    Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync(DateTime now);
    Task<FinancialStatisticsDto> GetFinancialStatisticsAsync();
    Task<List<MonthlyStatisticsDto>> GetMonthlyStatisticsAsync(DateTime from, DateTime to);
}
