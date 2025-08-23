using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Modules.Elements.Integration.Models.Rules;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Services;

internal class ElementsModuleQueries : IElementsModuleQueries
{
    private readonly IPersistence _persistence;

    public ElementsModuleQueries(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<CharacterCreationDataModel?> GetCharacterCreationElement(Guid uiid)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(uiid);
        return element?.AsCharacterCreationDataModel();
    }

    public async Task<List<CharacterCreationDataModel>> GetCharacterCreationElements()
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(ElementTypeConstants.CharacterCreation);
        return elements.ConvertAll(element => element.AsCharacterCreationDataModel());
    }

    public async Task<ElementDataModel?> GetElementWithRules(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        return element?.AsElementDataModel();
    }

    public async Task<List<IncludeRuleDataModel>> GetElementIncludeRules(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        return element?.AsElementDataModel().IncludeRules ?? [];
    }

    public async Task<AbilityDataModel?> GetAbilityModel(Guid elementId)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var element = await repository.GetElementAsync(elementId);
        return element?.AsAbilityDataModel();
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

    public async Task<IEnumerable<ElementDataModel>> GetElementsByType(string elementType)
    {
        var repository = _persistence.GetRepository<IElementsRepository>();
        var elements = await repository.GetElementsByTypeAsync(elementType);
        return elements.Select(e => e.AsElementDataModel());
    }
}
