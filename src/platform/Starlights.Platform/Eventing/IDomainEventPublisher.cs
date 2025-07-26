namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents a publisher for domain events.
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publishes the specified domain event asynchronously.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    Task PublishAsync(IDomainEvent domainEvent);

    /// <summary>
    /// Publishes a collection of domain events asynchronously.
    /// </summary>
    /// <param name="domainEvents">The domain events to publish.</param>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
}
