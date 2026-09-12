using DressShop.Application.Features.Orders.DTOs;
using MediatR;

namespace DressShop.Application.Features.Orders.GetOrders;

public record GetOrdersQuery(
    Guid UserId
) : IRequest<IReadOnlyList<OrderDto>>;
