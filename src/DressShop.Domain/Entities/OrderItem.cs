namespace DressShop.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? VariantDescription { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public Order Order { get; set; } = null!;
}
