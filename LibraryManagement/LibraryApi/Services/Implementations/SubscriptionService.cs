using LibraryApi.Data;
using LibraryApi.DTOs.Subscriptions;
using LibraryApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services.Implementations;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;

    public SubscriptionService(ApplicationDbContext context) => _context = context;

    public Task<List<SubscriptionListItemDto>> GetAllAsync()
    {
        var today = DateTime.UtcNow.Date;

        return _context.Subscriptions
            .AsNoTracking()
            .OrderByDescending(subscription => subscription.StartDate)
            .Select(subscription => new SubscriptionListItemDto
            {
                Id = subscription.Id,
                StudentName = subscription.Student!.FullName,
                StudentEmail = subscription.Student.Email,
                PlanName = subscription.SubscriptionPlan!.Name,
                PlanDescription = subscription.SubscriptionPlan.Description,
                Price = subscription.SubscriptionPlan.Price,
                DurationDays = subscription.SubscriptionPlan.DurationDays,
                MaxLoans = subscription.SubscriptionPlan.MaxLoans,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive,
                Status = !subscription.IsActive
                    ? "Inactif"
                    : subscription.EndDate < today
                        ? "Expiré"
                        : subscription.StartDate > today
                            ? "À venir"
                            : "Actif",
                PaymentStatus = _context.SubscriptionPayments
                    .Where(payment => payment.SubscriptionId == subscription.Id)
                    .OrderByDescending(payment => payment.PaymentDate)
                    .Select(payment => payment.Status)
                    .FirstOrDefault() ?? "Aucun paiement",
                PaymentAmount = _context.SubscriptionPayments
                    .Where(payment => payment.SubscriptionId == subscription.Id)
                    .OrderByDescending(payment => payment.PaymentDate)
                    .Select(payment => (decimal?)payment.Amount)
                    .FirstOrDefault(),
                PaymentDate = _context.SubscriptionPayments
                    .Where(payment => payment.SubscriptionId == subscription.Id)
                    .OrderByDescending(payment => payment.PaymentDate)
                    .Select(payment => (DateTime?)payment.PaymentDate)
                    .FirstOrDefault(),
                PaymentMethod = _context.SubscriptionPayments
                    .Where(payment => payment.SubscriptionId == subscription.Id)
                    .OrderByDescending(payment => payment.PaymentDate)
                    .Select(payment => payment.PaymentMethod)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}
