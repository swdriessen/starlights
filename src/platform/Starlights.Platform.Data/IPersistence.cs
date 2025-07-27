namespace Starlights.Platform.Data;

/// <summary>
/// IPersistence interface defines the contract for a persistence layer that manages repositories.
/// </summary>
public interface IPersistence : IDisposable
{
    /// <summary>
    /// Gets a repository of the given type.
    /// </summary>
    T GetRepository<T>() where T : IRepository;

    /// <summary>
    /// Asynchronously commits the changes made by all the repositories. Which use the same persistence context.
    /// </summary>
    Task<int> SaveChangesAsync();
}
