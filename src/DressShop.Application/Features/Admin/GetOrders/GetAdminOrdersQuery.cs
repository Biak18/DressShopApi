using DressShop.Application.Features.Orders.DTOs;
using MediatR;

namespace DressShop.Application.Features.Admin.GetOrders;

public sealed record GetAdminOrdersQuery(
    int Limit = 50
) : IRequest<IReadOnlyList<OrderDto>>;
