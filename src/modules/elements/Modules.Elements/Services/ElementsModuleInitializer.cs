using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration;
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
        using var _ = ElementsInstrumentation.StartActivity();

        var repository = _persistence.GetRepository<IElementsRepository>();
        var newElements = new List<Element>();

        var defaultCharacter = Element.Create("Default Character", ElementTypeConstants.CharacterCreation);
        defaultCharacter.AddComponent(new ShortDescriptionComponent(defaultCharacter.Id, "This is a default character for testing purposes."));
        newElements.Add(defaultCharacter);

        var strengthAbility = Element.Create("Strength", ElementTypeConstants.Ability);
        strengthAbility.AddComponent(new AbbreviationComponent(strengthAbility.Id, "STR"));
        newElements.Add(strengthAbility);

        var dexterityAbility = Element.Create("Dexterity", ElementTypeConstants.Ability);
        dexterityAbility.AddComponent(new AbbreviationComponent(dexterityAbility.Id, "DEX"));
        newElements.Add(dexterityAbility);

        var constitutionAbility = Element.Create("Constitution", ElementTypeConstants.Ability);
        constitutionAbility.AddComponent(new AbbreviationComponent(constitutionAbility.Id, "CON"));
        newElements.Add(constitutionAbility);

        var intelligenceAbility = Element.Create("Intelligence", ElementTypeConstants.Ability);
        intelligenceAbility.AddComponent(new AbbreviationComponent(intelligenceAbility.Id, "INT"));
        newElements.Add(intelligenceAbility);

        var wisdomAbility = Element.Create("Wisdom", ElementTypeConstants.Ability);
        wisdomAbility.AddComponent(new AbbreviationComponent(wisdomAbility.Id, "WIS"));
        newElements.Add(wisdomAbility);

        var charismaAbility = Element.Create("Charisma", ElementTypeConstants.Ability);
        charismaAbility.AddComponent(new AbbreviationComponent(charismaAbility.Id, "CHA"));
        newElements.Add(charismaAbility);

        foreach (var newElement in newElements)
        {
            await repository.AddAsync(newElement);
        }

        var rows = await _persistence.SaveChangesAsync();

        return new InitializationResult(rows);
    }
}
