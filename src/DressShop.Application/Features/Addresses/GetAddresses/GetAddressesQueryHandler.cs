using DressShop.Application.Abstractions;
using DressShop.Application.Features.Addresses.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Addresses.GetAddresses;

public sealed class GetAddressesQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<
    GetAddressesQuery,
    IReadOnlyList<AddressDto>>
{
    public async Task<IReadOnlyList<AddressDto>> Handle(
        GetAddressesQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Addresses
            .AsNoTracking()
            .Where(address =>
                address.UserId == request.UserId)
            .OrderByDescending(address => address.IsDefault)
            .ThenByDescending(address => address.CreatedAt)
            .Select(address => new AddressDto(
                address.Id,
                address.Label,
                address.RecipientName,
                address.Phone,
                address.AddressLine1,
                address.AddressLine2,
                address.City,
                address.State,
                address.PostalCode,
                address.Country,
                address.IsDefault,
                address.CreatedAt,
                address.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
