namespace DressShop.Application.Features.Products.DTOs;

public record UpdateProductRequest(
    string Name,
    string Slug,
    decimal BasePrice,
    Guid? CategoryId,
    string? Description,
    string? Occasion,
    string? Style
);
