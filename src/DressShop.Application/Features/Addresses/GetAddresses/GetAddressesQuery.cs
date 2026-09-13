using DressShop.Application.Features.Addresses.DTOs;
using MediatR;

namespace DressShop.Application.Features.Addresses.GetAddresses;

public sealed record GetAddressesQuery(
    Guid UserId
) : IRequest<IReadOnlyList<AddressDto>>;
