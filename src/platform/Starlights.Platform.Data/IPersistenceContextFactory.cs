namespace Starlights.Platform.Data;

/// <summary>
/// Represents a factory for creating persistence contexts.
/// </summary>
/// <remarks>
/// This avoids the context being created at the time of the repository instantiation, allowing for more flexible management of persistence contexts.
/// </remarks>
public interface IPersistenceContextFactory
{
    /// <summary>
    /// Creates a new persistence context.
    /// </summary>
    /// <returns>A new instance of <see cref="IPersistenceContext"/>.</returns>
    IPersistenceContext CreateContext();
}
