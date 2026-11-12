namespace LibraryApi.Models;

public class SubscriptionPlan
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public int MaxLoans { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; }
        = new List<Subscription>();
}
