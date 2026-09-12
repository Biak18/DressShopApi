using DressShop.Application.Features.Admin.DTOs;
using MediatR;

namespace DressShop.Application.Features.Admin.Categories;

public sealed record CreateCategoryCommand(
    CreateCategoryRequest Request
) : IRequest<AdminCategoryDto>;
