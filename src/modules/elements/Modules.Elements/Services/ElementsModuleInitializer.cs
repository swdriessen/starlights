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

        // create include rules for each element
        foreach (var e in defaultElements)
        {
            var includeRule = new IncludeRuleComponent(defaultCharacter.Id, e.Id, 0);
            defaultCharacter.AddComponent(includeRule); // EF should track this automatically
        }

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

        // Abilities
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

        // D&D 5e skills (18 total) with their primary abilities
        var acrobatics = Element.Create("Acrobatics", ElementTypeConstants.Skill);
        acrobatics.AddComponent(new PrimaryAbilityComponent(acrobatics.Id, dexterityAbility.Id));

        var animalHandling = Element.Create("Animal Handling", ElementTypeConstants.Skill);
        animalHandling.AddComponent(new PrimaryAbilityComponent(animalHandling.Id, wisdomAbility.Id));

        var arcana = Element.Create("Arcana", ElementTypeConstants.Skill);
        arcana.AddComponent(new PrimaryAbilityComponent(arcana.Id, intelligenceAbility.Id));

        var athletics = Element.Create("Athletics", ElementTypeConstants.Skill);
        athletics.AddComponent(new PrimaryAbilityComponent(athletics.Id, strengthAbility.Id));

        var deception = Element.Create("Deception", ElementTypeConstants.Skill);
        deception.AddComponent(new PrimaryAbilityComponent(deception.Id, charismaAbility.Id));

        var history = Element.Create("History", ElementTypeConstants.Skill);
        history.AddComponent(new PrimaryAbilityComponent(history.Id, intelligenceAbility.Id));

        var insight = Element.Create("Insight", ElementTypeConstants.Skill);
        insight.AddComponent(new PrimaryAbilityComponent(insight.Id, wisdomAbility.Id));

        var intimidation = Element.Create("Intimidation", ElementTypeConstants.Skill);
        intimidation.AddComponent(new PrimaryAbilityComponent(intimidation.Id, charismaAbility.Id));

        var investigation = Element.Create("Investigation", ElementTypeConstants.Skill);
        investigation.AddComponent(new PrimaryAbilityComponent(investigation.Id, intelligenceAbility.Id));

        var medicine = Element.Create("Medicine", ElementTypeConstants.Skill);
        medicine.AddComponent(new PrimaryAbilityComponent(medicine.Id, wisdomAbility.Id));

        var nature = Element.Create("Nature", ElementTypeConstants.Skill);
        nature.AddComponent(new PrimaryAbilityComponent(nature.Id, intelligenceAbility.Id));

        var perception = Element.Create("Perception", ElementTypeConstants.Skill);
        perception.AddComponent(new PrimaryAbilityComponent(perception.Id, wisdomAbility.Id));

        var performance = Element.Create("Performance", ElementTypeConstants.Skill);
        performance.AddComponent(new PrimaryAbilityComponent(performance.Id, charismaAbility.Id));

        var persuasion = Element.Create("Persuasion", ElementTypeConstants.Skill);
        persuasion.AddComponent(new PrimaryAbilityComponent(persuasion.Id, charismaAbility.Id));

        var religion = Element.Create("Religion", ElementTypeConstants.Skill);
        religion.AddComponent(new PrimaryAbilityComponent(religion.Id, intelligenceAbility.Id));

        var sleightOfHand = Element.Create("Sleight of Hand", ElementTypeConstants.Skill);
        sleightOfHand.AddComponent(new PrimaryAbilityComponent(sleightOfHand.Id, dexterityAbility.Id));

        var stealth = Element.Create("Stealth", ElementTypeConstants.Skill);
        stealth.AddComponent(new PrimaryAbilityComponent(stealth.Id, dexterityAbility.Id));

        var survival = Element.Create("Survival", ElementTypeConstants.Skill);
        survival.AddComponent(new PrimaryAbilityComponent(survival.Id, wisdomAbility.Id));

        newElements.Add(acrobatics);
        newElements.Add(animalHandling);
        newElements.Add(arcana);
        newElements.Add(athletics);
        newElements.Add(deception);
        newElements.Add(history);
        newElements.Add(insight);
        newElements.Add(intimidation);
        newElements.Add(investigation);
        newElements.Add(medicine);
        newElements.Add(nature);
        newElements.Add(perception);
        newElements.Add(performance);
        newElements.Add(persuasion);
        newElements.Add(religion);
        newElements.Add(sleightOfHand);
        newElements.Add(stealth);
        newElements.Add(survival);


        // D&D 5e saving throws (6 total) with their primary abilities

        var strengthSavingThrow = Element.Create("Strength Saving Throw", ElementTypeConstants.SavingThrow);
        strengthSavingThrow.AddComponent(new PrimaryAbilityComponent(strengthSavingThrow.Id, strengthAbility.Id));

        var dexteritySavingThrow = Element.Create("Dexterity Saving Throw", ElementTypeConstants.SavingThrow);
        dexteritySavingThrow.AddComponent(new PrimaryAbilityComponent(dexteritySavingThrow.Id, dexterityAbility.Id));

        var constitutionSavingThrow = Element.Create("Constitution Saving Throw", ElementTypeConstants.SavingThrow);
        constitutionSavingThrow.AddComponent(new PrimaryAbilityComponent(constitutionSavingThrow.Id, constitutionAbility.Id));

        var intelligenceSavingThrow = Element.Create("Intelligence Saving Throw", ElementTypeConstants.SavingThrow);
        intelligenceSavingThrow.AddComponent(new PrimaryAbilityComponent(intelligenceSavingThrow.Id, intelligenceAbility.Id));

        var wisdomSavingThrow = Element.Create("Wisdom Saving Throw", ElementTypeConstants.SavingThrow);
        wisdomSavingThrow.AddComponent(new PrimaryAbilityComponent(wisdomSavingThrow.Id, wisdomAbility.Id));

        var charismaSavingThrow = Element.Create("Charisma Saving Throw", ElementTypeConstants.SavingThrow);
        charismaSavingThrow.AddComponent(new PrimaryAbilityComponent(charismaSavingThrow.Id, charismaAbility.Id));

        newElements.Add(strengthSavingThrow);
        newElements.Add(dexteritySavingThrow);
        newElements.Add(constitutionSavingThrow);
        newElements.Add(intelligenceSavingThrow);
        newElements.Add(wisdomSavingThrow);
        newElements.Add(charismaSavingThrow);

        return newElements;
    }
}
