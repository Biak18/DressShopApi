namespace DressShop.Domain.Entities;

public class ProductVariant
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string? Color { get; set; }

    public string? Size { get; set; }

    public decimal? Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
