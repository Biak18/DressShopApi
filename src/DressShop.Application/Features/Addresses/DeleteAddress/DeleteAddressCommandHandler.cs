using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Addresses.DeleteAddress;

public sealed class DeleteAddressCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<DeleteAddressCommand>
{
    public async Task Handle(
        DeleteAddressCommand request,
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

        context.Addresses.Remove(address);

        await context.SaveChangesAsync(cancellationToken);
    }
}
