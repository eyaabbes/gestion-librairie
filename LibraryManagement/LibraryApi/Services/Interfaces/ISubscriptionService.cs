using LibraryApi.DTOs.Subscriptions;

namespace LibraryApi.Services.Interfaces;

public interface ISubscriptionService
{
    Task<List<SubscriptionListItemDto>> GetAllAsync();
}
