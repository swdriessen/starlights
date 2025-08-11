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

        var defaultElements = CreateDefaultElements();

        // create include rules for each ability 
        foreach (var e in defaultElements)//.Where(x => x.Type == "Ability"))
        {
            var includeRule = new IncludeRuleComponent(defaultCharacter.Id, e.Id, 0);
            defaultCharacter.AddComponent(includeRule); // EF should track this automatically
        }

        //var skillsRule = CreateSkillsRule(repository);


        //defaultCharacter.AddComponent(new IncludeRuleComponent(defaultCharacter.Id, skillsRule.Id, 0));

        // create include rules for each ability 
        //foreach (var e in defaultElements.Where(x => x.Type == "Skill"))
        //{
        //    var includeRule = new IncludeRuleComponent(skillsRule.Id, e.Id, 0);
        //    skillsRule.AddComponent(includeRule); // EF should track this automatically
        //}


        newElements.AddRange(defaultElements);

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
    private static Element CreateSkillsRule(IElementsRepository repository)
    {
        var skillsRule = Element.Create("Skills", "Rule");

        repository.Add(skillsRule);

        return skillsRule;
    }

    private static List<Element> CreateDefaultElements()
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


        var athletics = Element.Create("Athletics", ElementTypeConstants.Skill);
        athletics.AddComponent(new PrimaryAbilityComponent(athletics.Id, strengthAbility.Id));

        var arcana = Element.Create("Arcana", ElementTypeConstants.Skill);
        arcana.AddComponent(new PrimaryAbilityComponent(arcana.Id, intelligenceAbility.Id));

        var history = Element.Create("History", ElementTypeConstants.Skill);
        history.AddComponent(new PrimaryAbilityComponent(history.Id, intelligenceAbility.Id));

        var perception = Element.Create("Perception", ElementTypeConstants.Skill);
        perception.AddComponent(new PrimaryAbilityComponent(perception.Id, wisdomAbility.Id));

        var stealth = Element.Create("Stealth", ElementTypeConstants.Skill);
        stealth.AddComponent(new PrimaryAbilityComponent(stealth.Id, dexterityAbility.Id));



        newElements.Add(athletics);
        newElements.Add(arcana);
        newElements.Add(history);
        newElements.Add(perception);
        newElements.Add(stealth);




        return newElements;
    }
}
