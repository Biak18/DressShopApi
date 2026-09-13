namespace DressShop.Domain.Entities;

public class Address
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Label { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = "US";

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
