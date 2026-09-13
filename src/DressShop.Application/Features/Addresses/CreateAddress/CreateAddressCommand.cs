using DressShop.Application.Features.Addresses.DTOs;
using MediatR;

namespace DressShop.Application.Features.Addresses.CreateAddress;

public sealed record CreateAddressCommand(
    Guid UserId,
    CreateAddressRequest Request
) : IRequest<AddressDto>;
