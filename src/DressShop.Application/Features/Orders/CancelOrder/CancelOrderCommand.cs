using DressShop.Application.Features.Orders.DTOs;
using MediatR;

namespace DressShop.Application.Features.Orders.CancelOrder;

public record CancelOrderCommand(
    Guid UserId,
    Guid OrderId
) : IRequest<OrderDto?>;
