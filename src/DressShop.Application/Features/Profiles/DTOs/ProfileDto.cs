namespace DressShop.Application.Features.Profiles.DTOs;

public sealed record ProfileDto(
    Guid Id,
    string? FullName,
    string? AvatarUrl,
    string Role,
    string? Email,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
