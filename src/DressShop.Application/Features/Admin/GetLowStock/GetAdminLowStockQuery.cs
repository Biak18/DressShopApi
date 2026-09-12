using DressShop.Application.Features.Admin.DTOs;
using MediatR;

namespace DressShop.Application.Features.Admin.GetLowStock;

public sealed record GetAdminLowStockQuery(
    int Threshold = 3,
    int Limit = 20
) : IRequest<IReadOnlyList<AdminLowStockItemDto>>;
