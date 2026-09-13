using DressShop.Application.Features.Addresses.DTOs;
using MediatR;

namespace DressShop.Application.Features.Addresses.UpdateAddress;

public sealed record UpdateAddressCommand(
    Guid UserId,
    Guid AddressId,
    UpdateAddressRequest Request
) : IRequest<AddressDto>;
