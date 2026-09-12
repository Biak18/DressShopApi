using MediatR;

namespace DressShop.Application.Features.Admin.Categories;

public sealed record DeleteCategoryCommand(
    Guid CategoryId
) : IRequest;
