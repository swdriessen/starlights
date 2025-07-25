using Starlights.Platform.Eventing;

namespace Starlights.Platform.Domain;

/// <summary>
/// Represents an aggregate root entity that can raise domain events.
/// </summary>
public interface IAggregateRoot<TKey> : IEntity<TKey>, IEventEntity where TKey : struct;
