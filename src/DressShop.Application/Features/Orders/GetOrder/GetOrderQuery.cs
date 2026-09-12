using DressShop.Application.Features.Orders.DTOs;
using MediatR;

namespace DressShop.Application.Features.Orders.GetOrder;

public record GetOrderQuery(
    Guid UserId,
    Guid OrderId
) : IRequest<OrderDto?>;
