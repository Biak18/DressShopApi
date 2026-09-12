using MediatR;

namespace DressShop.Application.Features.Admin.UpdateVariantStock;

public sealed record UpdateVariantStockCommand(
    Guid VariantId,
    int Quantity
) : IRequest;
