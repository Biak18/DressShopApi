namespace DressShop.Application.Features.Admin.DTOs;

public sealed record AdminStatsDto(
    int Products,
    int Orders,
    int LowStock,
    int Categories,
    int Customers
);
