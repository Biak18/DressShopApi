using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<UpdateOrderStatusCommand>
{
    private static readonly HashSet<string> AllowedStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "pending",
            "confirmed",
            "processing",
            "shipped",
            "delivered",
            "cancelled"
        };

    private static readonly Dictionary<string, HashSet<string>>
        AllowedTransitions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["pending"] =
                [
                    "confirmed",
                    "cancelled"
                ],

                ["confirmed"] =
                [
                    "processing",
                    "cancelled"
                ],

                ["processing"] =
                [
                    "shipped",
                    "cancelled"
                ],

                ["shipped"] =
                [
                    "delivered"
                ],

                ["delivered"] = [],

                ["cancelled"] = []
            };

    public async Task Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var newStatus = request.Status.Trim().ToLowerInvariant();

        if (!AllowedStatuses.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid order status: {request.Status}");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(
                order => order.Id == request.OrderId,
                cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException(
                "Order not found.");
        }

        if (string.Equals(
                order.Status,
                newStatus,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!AllowedTransitions.TryGetValue(
                order.Status,
                out var allowedNextStatuses) ||
            !allowedNextStatuses.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot change order status from '{order.Status}' to '{newStatus}'.");
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
