using DressShop.Application.Features.Notifications.DTOs;

namespace DressShop.Application.Abstractions;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> MarkReadAsync(
        Guid userId,
        Guid notificationId,
        CancellationToken cancellationToken);

    Task MarkAllReadAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<NotificationPreferenceDto> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<NotificationPreferenceDto> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken);

    Task CreateOrderConfirmedAsync(
        Guid userId,
        Guid orderId,
        decimal total,
        CancellationToken cancellationToken);

    Task CreateOrderCancelledAsync(
        Guid userId,
        Guid orderId,
        CancellationToken cancellationToken);
}
