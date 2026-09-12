namespace DressShop.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Body { get; set; }

    public string? Data { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
