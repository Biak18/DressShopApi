namespace DressShop.Application.Abstractions;

public interface ITransactionManager
{
    Task<ITransaction> BeginTransactionAsync(
        CancellationToken cancellationToken);
}

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);

    Task RollbackAsync(CancellationToken cancellationToken);
}
