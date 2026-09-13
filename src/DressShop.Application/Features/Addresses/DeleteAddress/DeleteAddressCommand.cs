using MediatR;

namespace DressShop.Application.Features.Addresses.DeleteAddress;

public sealed record DeleteAddressCommand(
    Guid UserId,
    Guid AddressId
) : IRequest;
