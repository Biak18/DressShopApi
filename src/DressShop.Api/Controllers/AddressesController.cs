using DressShop.Application.Abstractions;
using DressShop.Application.Features.Addresses.CreateAddress;
using DressShop.Application.Features.Addresses.DeleteAddress;
using DressShop.Application.Features.Addresses.DTOs;
using DressShop.Application.Features.Addresses.GetAddresses;
using DressShop.Application.Features.Addresses.SetDefaultAddress;
using DressShop.Application.Features.Addresses.UpdateAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DressShop.Api.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize]
public sealed class AddressesController(
    ISender sender,
    ICurrentUser currentUser
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AddressDto>>> GetAddresses(
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new GetAddressesQuery(userId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress(
        CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateAddressCommand(
                userId,
                request),
            cancellationToken);

        return Created(
            $"/api/addresses/{result.Id}",
            result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(
        Guid id,
        UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new UpdateAddressCommand(
                userId,
                id,
                request),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAddress(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await sender.Send(
            new DeleteAddressCommand(
                userId,
                id),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/default")]
    public async Task<IActionResult> SetDefaultAddress(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        await sender.Send(
            new SetDefaultAddressCommand(
                userId,
                id),
            cancellationToken);

        return NoContent();
    }
}
