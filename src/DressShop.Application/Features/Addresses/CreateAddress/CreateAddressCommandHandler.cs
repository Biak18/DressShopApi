using DressShop.Application.Abstractions;
using DressShop.Application.Features.Addresses.DTOs;
using DressShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Addresses.CreateAddress;

public sealed class CreateAddressCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<
    CreateAddressCommand,
    AddressDto>
{
    public async Task<AddressDto> Handle(
        CreateAddressCommand request,
        CancellationToken cancellationToken)
    {
        Validate(
            request.Request.RecipientName,
            request.Request.AddressLine1,
            request.Request.City,
            request.Request.Country);

        if (request.Request.IsDefault)
        {
            var existingDefaults = await context.Addresses
                .Where(address =>
                    address.UserId == request.UserId &&
                    address.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var adrs in existingDefaults)
            {
                adrs.IsDefault = false;
                adrs.UpdatedAt = DateTime.UtcNow;
            }
        }

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Label = NormalizeOptional(request.Request.Label),
            RecipientName = request.Request.RecipientName.Trim(),
            Phone = NormalizeOptional(request.Request.Phone),
            AddressLine1 = request.Request.AddressLine1.Trim(),
            AddressLine2 = NormalizeOptional(
                request.Request.AddressLine2),
            City = request.Request.City.Trim(),
            State = NormalizeOptional(request.Request.State),
            PostalCode = NormalizeOptional(
                request.Request.PostalCode),
            Country = request.Request.Country.Trim(),
            IsDefault = request.Request.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Addresses.Add(address);

        await context.SaveChangesAsync(cancellationToken);

        return Map(address);
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

    private static AddressDto Map(Address address)
    {
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
}
