namespace DressShop.Domain.Events;

/// <summary>
/// Marker base type for domain events. Kept in the Domain project (not a
/// separate "Events" namespace file split) so entities can raise events
/// without taking a dependency on MediatR or any messaging library.
/// </summary>
public abstract class BaseDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
