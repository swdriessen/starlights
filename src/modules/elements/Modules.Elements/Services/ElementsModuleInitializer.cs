using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Builders;
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

        CreateDefaultCharacterCreationOption(repository);

        var rows = await _persistence.SaveChangesAsync();

        return new InitializationResult(rows);
    }

    private static Element CreateDefaultCharacterCreationOption(IElementsRepository repository)
    {
        // proficiency bonus rule
        var proficiencyRule = CreateProficiencyRule(repository);

        // abilities rule
        var (abilitiesRule, abilities) = CreateAbilitiesRule(repository);

        // skills rule
        var skillsRule = CreateSkillsRule(repository, abilities);

        // default character creation option
        var defaultCharacter = ElementBuilder.Create(ElementTypeConstants.CharacterCreation, "Default Character")
            .WithShortDescription("This is a default character for testing purposes.")
            .WithIncludeRule(proficiencyRule.Id)
            .WithIncludeRule(abilitiesRule.Id)
            .WithIncludeRule(skillsRule.Id)
            .Build();

        repository.Add(defaultCharacter);

        return defaultCharacter;
    }

    private static Element CreateProficiencyRule(IElementsRepository repository)
    {
        // proficiency bonus rule
        var element = ElementBuilder.Create(ElementTypeConstants.Rule, "Proficiency Bonus")
            .WithStatisticRule("proficiency", "2", "base", 0)
            .WithStatisticRule("proficiency", "3", "base", 5)
            .WithStatisticRule("proficiency", "4", "base", 9)
            .WithStatisticRule("proficiency", "5", "base", 13)
            .WithStatisticRule("proficiency", "6", "base", 17)
            .Build();

        repository.Add(element);

        return element;
    }

    private static (Element AbilitiesRule, IEnumerable<Element> Abilities) CreateAbilitiesRule(IElementsRepository repository)
    {
        var strength = ElementBuilder.Create(ElementTypeConstants.Ability, "Strength")
            .WithAbbreviationComponent("STR")
            .WithComponent(id => new SortingComponent(id, 1))
            .Build();

        var dexterity = ElementBuilder.Create(ElementTypeConstants.Ability, "Dexterity")
            .WithAbbreviationComponent("DEX")
            .WithComponent(id => new SortingComponent(id, 2))
            .Build();

        var constitution = ElementBuilder.Create(ElementTypeConstants.Ability, "Constitution")
            .WithAbbreviationComponent("CON")
            .WithComponent(id => new SortingComponent(id, 3))
            .Build();

        var intelligence = ElementBuilder.Create(ElementTypeConstants.Ability, "Intelligence")
            .WithAbbreviationComponent("INT")
            .WithComponent(id => new SortingComponent(id, 4))
            .Build();

        var wisdom = ElementBuilder.Create(ElementTypeConstants.Ability, "Wisdom")
            .WithAbbreviationComponent("WIS")
            .WithComponent(id => new SortingComponent(id, 5))
            .Build();

        var charisma = ElementBuilder.Create(ElementTypeConstants.Ability, "Charisma")
            .WithAbbreviationComponent("CHA")
            .WithComponent(id => new SortingComponent(id, 6))
            .Build();

        repository.Add(strength);
        repository.Add(dexterity);
        repository.Add(constitution);
        repository.Add(intelligence);
        repository.Add(wisdom);
        repository.Add(charisma);

        var strengthSave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Strength Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, strength.Id))
            .WithComponent(id => new SortingComponent(id, 1))
            .Build();

        var dexteritySave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Dexterity Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, dexterity.Id))
            .WithComponent(id => new SortingComponent(id, 2))
            .Build();

        var constitutionSave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Constitution Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, constitution.Id))
            .WithComponent(id => new SortingComponent(id, 3))
            .Build();

        var intelligenceSave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Intelligence Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithComponent(id => new SortingComponent(id, 4))
            .Build();

        var wisdomSave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Wisdom Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithComponent(id => new SortingComponent(id, 5))
            .Build();

        var charismaSave = ElementBuilder.Create(ElementTypeConstants.SavingThrow, "Charisma Saving Throw")
            .WithComponent(id => new PrimaryAbilityComponent(id, charisma.Id))
            .WithComponent(id => new SortingComponent(id, 6))
            .Build();

        repository.Add(strengthSave);
        repository.Add(dexteritySave);
        repository.Add(constitutionSave);
        repository.Add(intelligenceSave);
        repository.Add(wisdomSave);
        repository.Add(charismaSave);

        // abilties rule
        var abilitiesRule = ElementBuilder.Create(ElementTypeConstants.Rule, "Abilities")
            .WithIncludeRule(strength.Id)
            .WithIncludeRule(dexterity.Id)
            .WithIncludeRule(constitution.Id)
            .WithIncludeRule(intelligence.Id)
            .WithIncludeRule(wisdom.Id)
            .WithIncludeRule(charisma.Id)
            .WithIncludeRule(strengthSave.Id)
            .WithIncludeRule(dexteritySave.Id)
            .WithIncludeRule(constitutionSave.Id)
            .WithIncludeRule(intelligenceSave.Id)
            .WithIncludeRule(wisdomSave.Id)
            .WithIncludeRule(charismaSave.Id)
            .Build();

        repository.Add(abilitiesRule);

        return (abilitiesRule, [strength, dexterity, constitution, intelligence, wisdom, charisma]);
    }

    private static Element CreateSkillsRule(IElementsRepository repository, IEnumerable<Element> abilities)
    {
        var strength = abilities.Single(a => a.Name == "Strength");
        var dexterity = abilities.Single(a => a.Name == "Dexterity");
        var intelligence = abilities.Single(a => a.Name == "Intelligence");
        var wisdom = abilities.Single(a => a.Name == "Wisdom");
        var charisma = abilities.Single(a => a.Name == "Charisma");

        // Skills (18)
        var acrobatics = ElementBuilder.Create(ElementTypeConstants.Skill, "Acrobatics")
            .WithComponent(id => new PrimaryAbilityComponent(id, dexterity.Id))
            .WithSorting(1)
            .Build();
        var animalHandling = ElementBuilder.Create(ElementTypeConstants.Skill, "Animal Handling")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithSorting(2)
            .Build();
        var arcana = ElementBuilder.Create(ElementTypeConstants.Skill, "Arcana")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithSorting(3)
            .Build();
        var athletics = ElementBuilder.Create(ElementTypeConstants.Skill, "Athletics")
            .WithComponent(id => new PrimaryAbilityComponent(id, strength.Id))
            .WithSorting(4)
            .Build();
        var deception = ElementBuilder.Create(ElementTypeConstants.Skill, "Deception")
            .WithComponent(id => new PrimaryAbilityComponent(id, charisma.Id))
            .WithSorting(5)
            .Build();
        var history = ElementBuilder.Create(ElementTypeConstants.Skill, "History")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithSorting(6)
            .Build();
        var insight = ElementBuilder.Create(ElementTypeConstants.Skill, "Insight")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithSorting(7)
            .Build();
        var intimidation = ElementBuilder.Create(ElementTypeConstants.Skill, "Intimidation")
            .WithComponent(id => new PrimaryAbilityComponent(id, charisma.Id))
            .WithSorting(8)
            .Build();
        var investigation = ElementBuilder.Create(ElementTypeConstants.Skill, "Investigation")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithSorting(9)
            .Build();
        var medicine = ElementBuilder.Create(ElementTypeConstants.Skill, "Medicine")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithSorting(10)
            .Build();
        var nature = ElementBuilder.Create(ElementTypeConstants.Skill, "Nature")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithSorting(11)
            .Build();
        var perception = ElementBuilder.Create(ElementTypeConstants.Skill, "Perception")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithSorting(12)
            .Build();
        var performance = ElementBuilder.Create(ElementTypeConstants.Skill, "Performance")
            .WithComponent(id => new PrimaryAbilityComponent(id, charisma.Id))
            .WithSorting(13)
            .Build();
        var persuasion = ElementBuilder.Create(ElementTypeConstants.Skill, "Persuasion")
            .WithComponent(id => new PrimaryAbilityComponent(id, charisma.Id))
            .WithSorting(14)
            .Build();
        var religion = ElementBuilder.Create(ElementTypeConstants.Skill, "Religion")
            .WithComponent(id => new PrimaryAbilityComponent(id, intelligence.Id))
            .WithSorting(15)
            .Build();
        var sleightOfHand = ElementBuilder.Create(ElementTypeConstants.Skill, "Sleight of Hand")
            .WithComponent(id => new PrimaryAbilityComponent(id, dexterity.Id))
            .WithSorting(16)
            .Build();
        var stealth = ElementBuilder.Create(ElementTypeConstants.Skill, "Stealth")
            .WithComponent(id => new PrimaryAbilityComponent(id, dexterity.Id))
            .WithSorting(17)
            .Build();
        var survival = ElementBuilder.Create(ElementTypeConstants.Skill, "Survival")
            .WithComponent(id => new PrimaryAbilityComponent(id, wisdom.Id))
            .WithSorting(18)
            .Build();

        // Add all skills to repository
        repository.Add(acrobatics);
        repository.Add(animalHandling);
        repository.Add(arcana);
        repository.Add(athletics);
        repository.Add(deception);
        repository.Add(history);
        repository.Add(insight);
        repository.Add(intimidation);
        repository.Add(investigation);
        repository.Add(medicine);
        repository.Add(nature);
        repository.Add(perception);
        repository.Add(performance);
        repository.Add(persuasion);
        repository.Add(religion);
        repository.Add(sleightOfHand);
        repository.Add(stealth);
        repository.Add(survival);

        // skills rule includes all skills
        var skillsRule = ElementBuilder.Create(ElementTypeConstants.Rule, "Skills")
            .WithIncludeRule(acrobatics.Id)
            .WithIncludeRule(animalHandling.Id)
            .WithIncludeRule(arcana.Id)
            .WithIncludeRule(athletics.Id)
            .WithIncludeRule(deception.Id)
            .WithIncludeRule(history.Id)
            .WithIncludeRule(insight.Id)
            .WithIncludeRule(intimidation.Id)
            .WithIncludeRule(investigation.Id)
            .WithIncludeRule(medicine.Id)
            .WithIncludeRule(nature.Id)
            .WithIncludeRule(perception.Id)
            .WithIncludeRule(performance.Id)
            .WithIncludeRule(persuasion.Id)
            .WithIncludeRule(religion.Id)
            .WithIncludeRule(sleightOfHand.Id)
            .WithIncludeRule(stealth.Id)
            .WithIncludeRule(survival.Id)
            .Build();

        repository.Add(skillsRule);

        return skillsRule;
    }
}
