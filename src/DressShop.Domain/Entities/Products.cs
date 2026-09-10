namespace DressShop.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public Guid? CategoryId { get; set; }

    public string? Description { get; set; }

    public string? Occasion { get; set; }

    public string? Style { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Category? Category { get; set; }

    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();

    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();

    public ICollection<Review> Reviews { get; set; }
        = new List<Review>();
}
