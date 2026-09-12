namespace DressShop.Application.Abstractions;

public interface INotificationService
{
    Task CreateOrderConfirmedAsync(
        Guid userId,
        Guid orderId,
        decimal total,
        CancellationToken cancellationToken);
}
