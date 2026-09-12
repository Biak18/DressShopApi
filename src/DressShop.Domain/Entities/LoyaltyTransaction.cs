namespace DressShop.Domain.Entities;

public class LoyaltyTransaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Points { get; set; }

    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Order? Order { get; set; }
}
