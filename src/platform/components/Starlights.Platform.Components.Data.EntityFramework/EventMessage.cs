using System.Diagnostics;
using Starlights.Platform.Domain;
using Starlights.Platform.Eventing;

namespace Starlights.Platform.Components.Data.EntityFramework;

/// <summary>
/// Represents a generic message that encapsulates a domain event for processing.
/// </summary>
[DebuggerDisplay("EventType = {EventType}, OccurredOn = {OccurredOn}, ProcessedOn = {ProcessedOn}")]
public sealed class EventMessage : EntityBase<Guid>
{
    public EventMessage()
        : base(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Gets the type of the event that this message represents.
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// Gets the JSON payload of the event. This is the serialized representation of the IDomainEvent.
    /// </summary>
    public required string Payload { get; init; }

    /// <summary>
    /// Gets the date and time the event occurred. This is already a property on the IDomainEvent interface
    /// </summary>
    public required DateTime OccurredOn { get; init; }

    /// <summary>
    /// Gets the date and time when the event message was processed. This is set when the message is handled by the processing system.
    /// </summary>
    public DateTime? ProcessedOn { get; set; } = null;

    /// <summary>
    /// Gets the error message if the event processing failed. This is set when an error occurs during processing.
    /// </summary>
    public string? ErrorMessage { get; set; } = null;

    /// <summary>
    /// Creates a new instance of the <see cref="EventMessage"/> class from a domain event.
    /// </summary>
    public static EventMessage Create(IDomainEvent @event, Type eventType, string jsonPayload)
    {
        return new EventMessage
        {
            OccurredOn = @event.OccurredOn,
            EventType = eventType.FullName ?? eventType.Name,
            Payload = jsonPayload
        };
    }
}
