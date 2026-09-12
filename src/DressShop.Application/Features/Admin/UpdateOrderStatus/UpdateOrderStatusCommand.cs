using MediatR;

namespace DressShop.Application.Features.Admin.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    string Status
) : IRequest;
