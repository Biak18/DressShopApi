using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Addresses.SetDefaultAddress;

public sealed class SetDefaultAddressCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<SetDefaultAddressCommand>
{
    public async Task Handle(
        SetDefaultAddressCommand request,
        CancellationToken cancellationToken)
    {
        var address = await context.Addresses
            .FirstOrDefaultAsync(
                address =>
                    address.Id == request.AddressId &&
                    address.UserId == request.UserId,
                cancellationToken);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Address not found.");
        }

        var existingDefaults = await context.Addresses
            .Where(existing =>
                existing.UserId == request.UserId &&
                existing.IsDefault &&
                existing.Id != request.AddressId)
            .ToListAsync(cancellationToken);

        foreach (var existing in existingDefaults)
        {
            existing.IsDefault = false;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        address.IsDefault = true;
        address.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
