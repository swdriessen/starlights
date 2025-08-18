using Starlights.Platform.Eventing;

namespace Starlights.Platform.Domain;

/// <summary>
/// Represents an aggregate root entity that can raise domain events.
/// </summary>
public abstract class AggregateRoot<TKey> : EntityBase<TKey>, IAggregateRoot<TKey> where TKey : struct
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TKey id)
        : base(id)
    {
    }

    /// <summary>
    /// Gets the domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
