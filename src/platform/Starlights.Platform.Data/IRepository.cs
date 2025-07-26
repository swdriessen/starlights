namespace Starlights.Platform.Data;

/// <summary>
/// Represents a repository interface that can be used to interact with the persistence context.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Sets the persistence context for the repository.
    /// </summary>
    void SetPersistenceContext(IPersistenceContext persistenceContext);
}