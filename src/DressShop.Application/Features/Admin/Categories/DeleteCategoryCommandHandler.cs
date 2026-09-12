using DressShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Application.Features.Admin.Categories;

public sealed class DeleteCategoryCommandHandler(
    IApplicationDbContext context
) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (category is null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        context.Categories.Remove(category);

        await context.SaveChangesAsync(cancellationToken);
    }
}
