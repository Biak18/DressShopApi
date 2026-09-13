using DressShop.Application.Abstractions;
using DressShop.Application.Features.Addresses.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Addresses.UpdateAddress;

public sealed class UpdateAddressCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    UpdateAddressCommand,
    AddressDto>
{
    public async Task<AddressDto> Handle(
        UpdateAddressCommand request,
        CancellationToken cancellationToken)
    {
        Validate(
            request.Request.RecipientName,
            request.Request.AddressLine1,
            request.Request.City,
            request.Request.Country);

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

        if (request.Request.IsDefault)
        {
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
        }

        address.Label = NormalizeOptional(
            request.Request.Label);

        address.RecipientName =
            request.Request.RecipientName.Trim();

        address.Phone = NormalizeOptional(
            request.Request.Phone);

        address.AddressLine1 =
            request.Request.AddressLine1.Trim();

        address.AddressLine2 = NormalizeOptional(
            request.Request.AddressLine2);

        address.City =
            request.Request.City.Trim();

        address.State = NormalizeOptional(
            request.Request.State);

        address.PostalCode = NormalizeOptional(
            request.Request.PostalCode);

        address.Country =
            request.Request.Country.Trim();

        address.IsDefault =
            request.Request.IsDefault;

        address.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new AddressDto(
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
        );
    }

    private static void Validate(
        string recipientName,
        string addressLine1,
        string city,
        string country)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
        {
            throw new InvalidOperationException(
                "Recipient name is required.");
        }

        if (string.IsNullOrWhiteSpace(addressLine1))
        {
            throw new InvalidOperationException(
                "Address line 1 is required.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new InvalidOperationException(
                "City is required.");
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new InvalidOperationException(
                "Country is required.");
        }
    }

    private static string? NormalizeOptional(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
