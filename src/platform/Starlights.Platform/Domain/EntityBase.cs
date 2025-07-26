namespace Starlights.Platform.Domain;

/// <summary>
/// Represents a base class for entities with a unique identifier.
/// </summary>
public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : struct
{
    protected EntityBase(TKey id)
    {
        Id = id;
    }

    /// <inheritdoc />
    public TKey Id { get; protected set; }
}
