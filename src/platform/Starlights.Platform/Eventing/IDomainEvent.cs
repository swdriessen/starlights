namespace Starlights.Platform.Eventing;

/// <summary>
/// Represents a domain event.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}

public abstract record EventBase : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOn => DateTime.UtcNow;
}