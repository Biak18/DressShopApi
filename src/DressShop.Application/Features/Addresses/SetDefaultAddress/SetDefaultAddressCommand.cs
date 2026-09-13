using MediatR;

namespace DressShop.Application.Features.Addresses.SetDefaultAddress;

public sealed record SetDefaultAddressCommand(
    Guid UserId,
    Guid AddressId
) : IRequest;
