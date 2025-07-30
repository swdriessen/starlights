using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Extensions;
using Starlights.Modules.Elements.Integration.Abstractions;
using Starlights.Modules.Elements.Integration.Abstractions.Models;
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
}
