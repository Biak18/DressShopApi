using MediatR;

namespace DressShop.Application.Features.Admin.UpdateProductActive;

public sealed record UpdateProductActiveCommand(
    Guid ProductId,
    bool IsActive
) : IRequest;
