using DressShop.Application.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace DressShop.Infrastructure.Persistence;

public sealed class TransactionManager(
    AppDbContext context
) : ITransactionManager
{
    public async Task<ITransaction> BeginTransactionAsync(
        CancellationToken cancellationToken)
    {
        var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        return new EfTransaction(transaction);
    }

    private sealed class EfTransaction(
        IDbContextTransaction transaction
    ) : ITransaction
    {
        public Task CommitAsync(
            CancellationToken cancellationToken) => transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(
            CancellationToken cancellationToken) => transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
