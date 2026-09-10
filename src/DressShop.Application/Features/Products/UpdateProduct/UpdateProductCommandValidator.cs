using FluentValidation;

namespace DressShop.Application.Features.Products.UpdateProduct;

public class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.");

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
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Base price must be greater than or equal to 0.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Occasion)
            .MaximumLength(100)
            .When(x => x.Occasion is not null);

        RuleFor(x => x.Style)
            .MaximumLength(100)
            .When(x => x.Style is not null);
    }
}
