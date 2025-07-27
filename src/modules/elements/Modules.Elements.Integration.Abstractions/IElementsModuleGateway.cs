namespace Starlights.Modules.Elements.Integration.Abstractions;

/// <summary>
/// Defines the gateway for accessing elements in the system.
/// </summary>
public interface IElementsModuleGateway
{
    /// <summary>
    /// Retrieves an element by its identifier.
    /// </summary>
    Task<ElementModel?> GetElement(string identifier);

    /// <summary>
    /// Retrieves all elements of a specific type.
    /// </summary>
    Task<IEnumerable<ElementModel>> GetElements(string type);
}