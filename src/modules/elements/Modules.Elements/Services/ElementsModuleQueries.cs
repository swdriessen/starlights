using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Services;

internal class ElementsModuleQueries : IElementsModuleQueries
{
    private readonly ILogger<ElementsModuleQueries> _logger;
    private readonly IPersistence _persistence;

    public ElementsModuleQueries(ILogger<ElementsModuleQueries> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public async Task<CharacterCreationInfo?> GetCharacterCreationElement(Guid uiid)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var element = await repository.GetElementAsync(uiid);

        return element?.AsCharacterCreationInfo();
    }

    public async Task<List<CharacterCreationInfo>> GetCharacterCreationElements()
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.CharacterCreation);
        if (elements.Count == 0)
        {
            _logger.LogWarning("No character creation elements found.");
            return [];
        }

        return elements.ConvertAll(element => element.AsCharacterCreationInfo());
    }

    public async Task<List<ElementInfo>> GetElements()
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var elements = await repository.GetElementsByTypesAsync([ElementTypeConstants.CharacterCreation, ElementTypeConstants.Ability, ElementTypeConstants.Skill]);

        if (elements.Count == 0)
        {
            _logger.LogWarning("No elements found.");
            return [];
        }

        return elements.ConvertAll(element => element.AsElementInfo());
    }

    public async Task<ElementDataModel> GetElementWithRules(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var element = await repository.GetElementAsync(elementId);

        return element.AsElementDataModel();
    }

    public async Task<List<IncludeRuleDataModel>> GetElementIncludeRules(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var element = await repository.GetElementAsync(elementId);

        return element.AsElementDataModel().IncludeRules;
    }

    public async Task<AbilityDataModel> GetAbilityModel(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();

        var element = await repository.GetElementAsync(elementId);

        return element.AsAbilityDataModel();
    }

    public async Task<SkillDataModel?> GetSkillModel(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        return element?.AsSkillDataModel();
    }

    public async Task<SavingThrowDataModel?> GetSavingThrowModel(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        return element?.AsSavingThrowDataModel();
    }
}
