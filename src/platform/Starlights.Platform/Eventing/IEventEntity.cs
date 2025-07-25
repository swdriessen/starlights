namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents an entity that can raise domain events.
/// </summary>
public interface IEventEntity
{
    /// <summary>
    /// Gets the domain events raised by this entity.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clear all domain events.
    /// </summary>
    void ClearDomainEvents();
}
