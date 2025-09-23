using Starlights.Platform.Eventing;

namespace Starlights.Platform.Domain;

/// <summary>
/// Represents an aggregate root entity that can raise domain events.
/// </summary>
public abstract class AggregateRoot<TKey> : EntityBase<TKey>, IAggregateRoot<TKey> where TKey : struct
{
    protected AggregateRoot(TKey id)
        : base(id)
    {
    }
}
