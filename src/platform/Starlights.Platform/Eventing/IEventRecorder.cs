namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents a service that records domain events. Usually used to persist events on an aggregate root from child entities.
/// </summary>
public interface IEventRecorder
{
    /// <summary>
    /// Adds a domain event to the entity's list of domain events.
    /// </summary>
    void RecordEvent(IDomainEvent @event);
}