using Starlights.Modules.Elements.Integration.Abstractions.Models;

namespace Starlights.Modules.Elements.Integration.Abstractions;

/// <summary>
/// Represents the (read-only) queries available from the Elements module, usually for character creation.
/// </summary>
public interface IElementsModuleQueries
{
    /// <summary>
    /// Retrieves a character creation element by its unique identifier (UIID).
    /// </summary>
    Task<CharacterCreationInfo?> GetCharacterCreationElement(Guid uiid);

    /// <summary>
    /// Retrieves a list of all character creation elements available in the system.
    /// </summary>
    Task<List<CharacterCreationInfo>> GetCharacterCreationElements();
}
