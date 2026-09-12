namespace DressShop.Application.Features.Orders.DTOs;

public record ShippingAddressDto(
    string AddressLine1,
    string? AddressLine2,
    string City,
    string? State,
    string? PostalCode,
    string? Country,
    string? Phone
);
