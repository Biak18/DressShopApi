namespace DressShop.Application.Features.Reviews.DTOs;

public record ReviewEligibilityDto(
    bool Verified,
    Guid? OrderItemId
);
