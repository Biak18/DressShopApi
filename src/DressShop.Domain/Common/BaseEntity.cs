using DressShop.Domain.Events;

namespace DressShop.Domain.Common;

/// <summary>
/// Base type for all domain entities. Uses a GUID identity so IDs can be
/// generated in the domain layer (e.g. inside factory methods) without a
/// round trip to the database.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    private readonly List<BaseDomainEvent> _domainEvents = [];

    /// <summary>
    /// Domain events raised by this entity that have not yet been dispatched.
    /// Cleared by infrastructure after SaveChanges via an interceptor/dispatcher.
    /// </summary>
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
