namespace DressShop.Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
