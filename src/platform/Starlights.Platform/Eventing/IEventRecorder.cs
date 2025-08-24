namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents a service that records domain events. Usually used as a delegate to add events on an aggregate root from child entities.
/// </summary>
public interface IEventRecorder
{
    /// <summary>
    /// Adds a domain event to the entity's list of domain events.
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);
}