using DressShop.Application.Abstractions;
using DressShop.Application.Features.Products.CreateProduct;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(200)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage(
                "Slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0);

        //RuleFor(x => x.CategoryId)
        //    .MustAsync(CategoryExists)
        //    .When(x => x.CategoryId.HasValue)
        //    .WithMessage("Category does not exist.");
    }

    private async Task<bool> CategoryExists(
        Guid? categoryId,
        CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(
                c => c.Id == categoryId,
                cancellationToken);
    }
}
