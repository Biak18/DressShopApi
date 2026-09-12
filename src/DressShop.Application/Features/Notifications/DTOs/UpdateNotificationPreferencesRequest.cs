namespace DressShop.Application.Features.Notifications.DTOs;

public record UpdateNotificationPreferencesRequest(
    bool? OrderUpdates,
    bool? BackInStock,
    bool? PriceDrop
);
