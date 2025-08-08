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

        var defaultCharacter = CreateDefaultCharacterCreationOption(repository);

        var abilities = CreateAbilities();

        // create include rules for each ability 
        foreach (var ability in abilities)
        {
            var includeRule = new IncludeRuleComponent(defaultCharacter.Id, ability.Id, 0);
            defaultCharacter.AddComponent(includeRule); // EF should track this automatically
        }

        newElements.AddRange(abilities);

        foreach (var newElement in newElements)
        {
            repository.Add(newElement);
        }

        var rows = await _persistence.SaveChangesAsync();

        return new InitializationResult(rows);
    }

    private static Element CreateDefaultCharacterCreationOption(IElementsRepository repository)
    {
        var defaultCharacter = Element.Create("Default Character", ElementTypeConstants.CharacterCreation);
        defaultCharacter.AddComponent(new ShortDescriptionComponent(defaultCharacter.Id, "This is a default character for testing purposes."));

        repository.Add(defaultCharacter);

        return defaultCharacter;
    }

    private static List<Element> CreateAbilities()
    {
        var newElements = new List<Element>();

        var strengthAbility = Element.Create("Strength", ElementTypeConstants.Ability);
        strengthAbility.AddComponent(new AbbreviationComponent(strengthAbility.Id, "STR"));

        var dexterityAbility = Element.Create("Dexterity", ElementTypeConstants.Ability);
        dexterityAbility.AddComponent(new AbbreviationComponent(dexterityAbility.Id, "DEX"));

        var constitutionAbility = Element.Create("Constitution", ElementTypeConstants.Ability);
        constitutionAbility.AddComponent(new AbbreviationComponent(constitutionAbility.Id, "CON"));

        var intelligenceAbility = Element.Create("Intelligence", ElementTypeConstants.Ability);
        intelligenceAbility.AddComponent(new AbbreviationComponent(intelligenceAbility.Id, "INT"));

        var wisdomAbility = Element.Create("Wisdom", ElementTypeConstants.Ability);
        wisdomAbility.AddComponent(new AbbreviationComponent(wisdomAbility.Id, "WIS"));

        var charismaAbility = Element.Create("Charisma", ElementTypeConstants.Ability);
        charismaAbility.AddComponent(new AbbreviationComponent(charismaAbility.Id, "CHA"));

        newElements.Add(strengthAbility);
        newElements.Add(dexterityAbility);
        newElements.Add(constitutionAbility);
        newElements.Add(intelligenceAbility);
        newElements.Add(wisdomAbility);
        newElements.Add(charismaAbility);

        return newElements;
    }
}
