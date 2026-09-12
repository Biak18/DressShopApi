namespace DressShop.Domain.Entities;

public class NotificationPreference
{
    public Guid UserId { get; set; }

    public bool OrderUpdates { get; set; } = true;

    public bool BackInStock { get; set; } = true;

    public bool PriceDrop { get; set; } = true;

    public DateTime UpdatedAt { get; set; }
}
