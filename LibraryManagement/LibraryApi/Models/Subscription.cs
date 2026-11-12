namespace LibraryApi.Models;

public class Subscription
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
