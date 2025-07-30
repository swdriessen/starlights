using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Integration.Abstractions;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Services;

// TODO: this should be in another 'extension' project that deals with the specifics of the game system
internal class ElementsModuleInitializer : IElementsModuleInitializer
{
    private readonly ILogger<ElementsModuleInitializer> _logger;
    private readonly IPersistence _persistence;

    public ElementsModuleInitializer(ILogger<ElementsModuleInitializer> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public async Task<InitializationResult> InitializeAsync()
    {
        var defaultCharacter = Element.Create("Default Character Creation", ElementTypeConstants.CharacterCreation);

        var repository = _persistence.GetRepository<IElementsRepository>();

        await repository.AddAsync(defaultCharacter);

        var rows = await _persistence.SaveChangesAsync();

        return new InitializationResult(rows);
    }
}
