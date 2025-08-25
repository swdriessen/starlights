namespace Starlights.Platform.Eventing;

public abstract record EventBase : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOn => DateTime.UtcNow;
}