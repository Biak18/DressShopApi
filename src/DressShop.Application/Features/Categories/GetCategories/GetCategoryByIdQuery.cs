using DressShop.Application.Features.Categories.DTOs;
using MediatR;

namespace DressShop.Application.Features.Categories.GetCategories;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
