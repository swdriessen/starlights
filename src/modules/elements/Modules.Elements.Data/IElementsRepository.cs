using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Data;

public interface IElementsRepository : IRepository
{
    /// <summary>
    /// Adds a new element to the repository asynchronously.
    /// </summary>
    Task AddAsync(Element element);

    /// <summary>
    /// Retrieves an element by its identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier of the element.</param>
    /// <returns>The element if found; otherwise, null.</returns>
    Task<Element?> GetElementAsync(Guid identifier);

    /// <summary>
    /// Retrieves all elements of a specific type.
    /// </summary>
    /// <param name="type">The type of elements to retrieve.</param>
    /// <returns>A collection of elements of the specified type.</returns>
    Task<List<Element>> GetElementsByTypeAsync(string type);

    /// <summary>
    /// Retrieves all elements of specified types.
    /// </summary>
    Task<List<Element>> GetElementsByTypesAsync(IEnumerable<string> types);
}
