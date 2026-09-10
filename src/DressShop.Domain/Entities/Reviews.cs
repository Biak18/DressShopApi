namespace DressShop.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? OrderItemId { get; set; }

    public int Rating { get; set; }

    public string? Body { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
