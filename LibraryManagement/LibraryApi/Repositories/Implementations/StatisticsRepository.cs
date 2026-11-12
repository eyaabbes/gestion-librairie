using LibraryApi.Data;
using LibraryApi.DTOs.Statistics;
using LibraryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories.Implementations;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly ApplicationDbContext _context;

    public StatisticsRepository(ApplicationDbContext context) => _context = context;

    public async Task<DashboardStatisticsDto> GetDashboardAsync(DateTime now)
    {
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var activeLoans = _context.Loans.Where(l => l.ReturnDate == null);
        var revenue = await GetFinancialStatisticsAsync();

        return new DashboardStatisticsDto
        {
            TotalBooks = await _context.Books.CountAsync(),
            TotalStudents = await _context.Students.CountAsync(),
            TotalEmployees = await _context.Employees.CountAsync(),
            TotalLoans = await _context.Loans.CountAsync(),
            ActiveLoans = await activeLoans.CountAsync(),
            OverdueLoans = await activeLoans.CountAsync(l => l.DueDate < now),
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s =>
                s.IsActive && s.StartDate <= now && s.EndDate >= now),
            TotalRevenue = revenue.TotalRevenue
        };
    }

    public async Task<LoanStatisticsDto> GetLoanStatisticsAsync(DateTime now)
    {
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        return new LoanStatisticsDto
        {
            TotalLoans = await _context.Loans.CountAsync(),
            ActiveLoans = await _context.Loans.CountAsync(l => l.ReturnDate == null),
            ReturnedLoans = await _context.Loans.CountAsync(l => l.ReturnDate != null),
            OverdueLoans = await _context.Loans.CountAsync(l => l.ReturnDate == null && l.DueDate < now),
            LoansBorrowedThisMonth = await _context.Loans.CountAsync(l => l.BorrowDate >= startOfMonth && l.BorrowDate < now)
        };
    }

    public async Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync(DateTime now)
    {
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        return new SubscriptionStatisticsDto
        {
            TotalSubscriptions = await _context.Subscriptions.CountAsync(),
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s =>
                s.IsActive && s.StartDate <= now && s.EndDate >= now),
            ExpiredSubscriptions = await _context.Subscriptions.CountAsync(s =>
                s.EndDate < now || !s.IsActive),
            SubscriptionsStartedThisMonth = await _context.Subscriptions.CountAsync(s =>
                s.StartDate >= startOfMonth && s.StartDate < now)
        };
    }

    public async Task<FinancialStatisticsDto> GetFinancialStatisticsAsync()
    {
        var subscriptionRevenue = await _context.SubscriptionPayments
            .Where(p => p.Status == "Paid")
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        var paidPenalties = await _context.Penalties
            .Where(p => p.IsPaid)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        var unpaidPenalties = await _context.Penalties
            .Where(p => !p.IsPaid)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        return new FinancialStatisticsDto
        {
            PaidSubscriptionRevenue = subscriptionRevenue,
            PaidPenaltyRevenue = paidPenalties,
            OutstandingPenalties = unpaidPenalties,
            TotalRevenue = subscriptionRevenue + paidPenalties
        };
    }

    public async Task<List<MonthlyStatisticsDto>> GetMonthlyStatisticsAsync(DateTime from, DateTime to)
    {
        var loans = await _context.Loans
            .Where(l => l.BorrowDate >= from && l.BorrowDate < to)
            .GroupBy(l => new { l.BorrowDate.Year, l.BorrowDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();

        var subscriptions = await _context.Subscriptions
            .Where(s => s.StartDate >= from && s.StartDate < to)
            .GroupBy(s => new { s.StartDate.Year, s.StartDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();

        var subscriptionPayments = await _context.SubscriptionPayments
            .Where(p => p.Status == "Paid" && p.PaymentDate >= from && p.PaymentDate < to)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();

        var penaltyPayments = await _context.Penalties
            .Where(p => p.IsPaid && p.PaidAt != null && p.PaidAt >= from && p.PaidAt < to)
            .GroupBy(p => new { Year = p.PaidAt!.Value.Year, Month = p.PaidAt!.Value.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();

        var months = new List<MonthlyStatisticsDto>();
        var cursor = new DateTime(from.Year, from.Month, 1);
        var lastMonth = new DateTime(to.Year, to.Month, 1);
        while (cursor < lastMonth)
        {
            var year = cursor.Year;
            var month = cursor.Month;
            months.Add(new MonthlyStatisticsDto
            {
                Year = year,
                Month = month,
                Loans = loans.FirstOrDefault(x => x.Year == year && x.Month == month)?.Count ?? 0,
                NewSubscriptions = subscriptions.FirstOrDefault(x => x.Year == year && x.Month == month)?.Count ?? 0,
                SubscriptionRevenue = subscriptionPayments.FirstOrDefault(x => x.Year == year && x.Month == month)?.Amount ?? 0m,
                PenaltyRevenue = penaltyPayments.FirstOrDefault(x => x.Year == year && x.Month == month)?.Amount ?? 0m
            });
            cursor = cursor.AddMonths(1);
        }

        return months;
    }
}
