using LibraryApi.DTOs.Statistics;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _repository;

    public StatisticsService(IStatisticsRepository repository) => _repository = repository;

    public Task<DashboardStatisticsDto> GetDashboardAsync() =>
        _repository.GetDashboardAsync(DateTime.UtcNow);

    public Task<LoanStatisticsDto> GetLoanStatisticsAsync() =>
        _repository.GetLoanStatisticsAsync(DateTime.UtcNow);

    public Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync() =>
        _repository.GetSubscriptionStatisticsAsync(DateTime.UtcNow);

    public Task<FinancialStatisticsDto> GetFinancialStatisticsAsync() =>
        _repository.GetFinancialStatisticsAsync();

    public Task<List<MonthlyStatisticsDto>> GetMonthlyStatisticsAsync(int months = 12)
    {
        if (months is < 1 or > 60)
            throw new ArgumentOutOfRangeException(nameof(months), "La période doit être comprise entre 1 et 60 mois.");

        var end = DateTime.UtcNow;
        var from = new DateTime(end.Year, end.Month, 1).AddMonths(-(months - 1));
        return _repository.GetMonthlyStatisticsAsync(from, end.AddTicks(1));
    }
}
