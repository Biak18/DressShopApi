namespace DressShop.Domain.Entities;

public class LoyaltyAccount
{
    public Guid UserId { get; set; }

    public int Points { get; set; }

    public string Tier { get; set; } = "bronze";

    public string? ReferralCode { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
