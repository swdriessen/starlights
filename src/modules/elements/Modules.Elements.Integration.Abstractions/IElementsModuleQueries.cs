using Starlights.Modules.Elements.Integration.Models;
using Starlights.Modules.Elements.Integration.Models.Rules;

namespace Starlights.Modules.Elements.Integration;

/// <summary>
/// Represents the (read-only) queries available from the Elements module, usually for character creation.
/// </summary>
public interface IElementsModuleQueries
{
    /// <summary>
    /// Retrieves a character creation element by its unique identifier.
    /// </summary>
    Task<CharacterCreationDataModel?> GetCharacterCreationElement(Guid elementId);

    /// <summary>
    /// Retrieves a list of all character creation elements available in the system.
    /// </summary>
    Task<List<CharacterCreationDataModel>> GetCharacterCreationElements();

    /// <summary>
    /// Retrieves an element header with rules by its unique identifier.
    /// </summary>
    Task<ElementDataModel?> GetElementWithRules(Guid elementId);

    /// <summary>
    /// Retrieves a list of include rules for a specific element by its unique identifier.
    /// </summary>
    Task<List<IncludeRuleDataModel>> GetElementIncludeRules(Guid elementId);

    /// <summary>
    /// Retrieves an ability model by its unique identifier.
    /// </summary>
    Task<AbilityDataModel?> GetAbilityModel(Guid elementId);

    /// <summary>
    /// Retrieves a skill model by its unique identifier.
    /// </summary>
    Task<SkillDataModel?> GetSkillModel(Guid elementId);

    /// <summary>
    /// Retrieves a savnng throw model by its unique identifier.
    /// </summary>
    Task<SavingThrowDataModel?> GetSavingThrowModel(Guid elementId);




    Task<IEnumerable<ElementDataModel>> GetElementsByType(string elementType);
}
