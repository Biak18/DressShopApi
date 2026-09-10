namespace DressShop.Application.Features.Reviews.DTOs;

public record ReviewDto(
    Guid Id,
    Guid UserId,
    Guid ProductId,
    Guid? OrderItemId,
    int Rating,
    string? Body,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? FullName,
    string? AvatarUrl
);
