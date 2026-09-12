using DressShop.Application.Features.Orders.DTOs;
using MediatR;

namespace DressShop.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    CreateOrderRequest Request
) : IRequest<OrderDto>;
