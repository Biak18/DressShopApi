namespace DressShop.Application.Features.Notifications.DTOs;

public record NotificationPreferenceDto(
    bool OrderUpdates,
    bool BackInStock,
    bool PriceDrop,
    DateTime UpdatedAt
);
