namespace Starlights.Platform.Domain;

/// <summary>
/// Represents a base interface for entities with a unique identifier.
/// </summary>
public interface IEntity<TKey> : IEntity where TKey : struct
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    TKey Id { get; }
}

public interface IEntity;