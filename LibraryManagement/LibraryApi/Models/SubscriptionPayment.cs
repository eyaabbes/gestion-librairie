namespace LibraryApi.Models;

public class SubscriptionPayment
{
    public int Id { get; set; }

    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Paid";
}
