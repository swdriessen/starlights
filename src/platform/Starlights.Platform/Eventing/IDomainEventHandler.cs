namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents a handler for domain events.
/// </summary>
/// <typeparam name="TEvent">The type of domain event.</typeparam>
public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event asynchronously.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    Task HandleAsync(TEvent domainEvent);
}
