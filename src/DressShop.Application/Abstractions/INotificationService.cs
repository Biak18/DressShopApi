namespace DressShop.Application.Abstractions;

public interface INotificationService
{
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
