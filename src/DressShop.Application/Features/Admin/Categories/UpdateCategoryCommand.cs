using DressShop.Application.Features.Admin.DTOs;
using MediatR;

namespace DressShop.Application.Features.Admin.Categories;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    UpdateCategoryRequest Request
) : IRequest<AdminCategoryDto>;
