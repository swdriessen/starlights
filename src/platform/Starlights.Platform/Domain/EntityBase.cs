using Starlights.Platform.Eventing;

namespace Starlights.Platform.Domain;

/// <summary>
/// Represents a base class for entities with a unique identifier.
/// </summary>
public abstract class EntityBase<TKey> : IEntity<TKey>, IEventEntity where TKey : struct
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected EntityBase(TKey id)
    {
        Id = id;
    }

    /// <inheritdoc />
    public TKey Id { get; protected set; }


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
