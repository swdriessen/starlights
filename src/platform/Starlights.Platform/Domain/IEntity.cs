namespace Starlights.Platform.Domain;

/// <summary>
/// Represents a base interface for entities with a unique identifier.
/// </summary>
public interface IEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    TKey Id { get; }
}
