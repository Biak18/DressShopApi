using DressShop.Application.Abstractions;
using MediatR;

namespace DressShop.Application.Features.Cart.AddCartItem;

public sealed class AddCartItemCommandHandler(
    ICartItemWriter cartItemWriter
) : IRequestHandler<AddCartItemCommand>
{
    public async Task Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be at least 1");
        }

        await cartItemWriter.AddAsync(
            request.UserId,
            request.VariantId,
            request.Quantity,
            cancellationToken);
    }
}
