namespace DressShop.Application.Features.Addresses.DTOs;

public sealed record UpdateAddressRequest(
    string? Label,
    string RecipientName,
    string? Phone,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string? State,
    string? PostalCode,
    string Country,
    bool IsDefault
);
