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
        CreateClasses(repository);
        CreateSpecies(repository);
        CreateBackgrounds(repository);
        CreateAlignments(repository);

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

        // class selection
        var classSelectionRule = ElementBuilder.Create(ElementTypeConstants.Rule, "Class Selection")
            .WithSelectionRule(ElementTypeConstants.Class, "Class")
            .Build();

        // origin selection
        var originSelectionRule = ElementBuilder.Create(ElementTypeConstants.Rule, "Origin Selection")
            .WithSelectionRule(ElementTypeConstants.Species, "Species")
            .WithSelectionRule(ElementTypeConstants.Background, "Background")
            .Build();

        // alignment selection
        var alignmentSelectionRule = ElementBuilder.Create(ElementTypeConstants.Rule, "Alignment Selection")
            .WithSelectionRule(ElementTypeConstants.Alignment, "Alignment")
            .Build();

        // default character creation option
        var defaultCharacter = ElementBuilder.Create(ElementTypeConstants.CharacterCreation, "Default Character")
            .WithShortDescription("This is a default character for testing purposes.")
            .WithIncludeRule(proficiencyRule.Id)
            .WithIncludeRule(abilitiesRule.Id)
            .WithIncludeRule(skillsRule.Id)
            .WithIncludeRule(classSelectionRule.Id)
            .WithIncludeRule(originSelectionRule.Id)
            .WithIncludeRule(alignmentSelectionRule.Id)
            .Build();

        repository.Add(defaultCharacter);
        repository.Add(classSelectionRule);
        repository.Add(originSelectionRule);
        repository.Add(alignmentSelectionRule);

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


        // create and element of type Proficiency for the arcana skill
        // with a statistic rule that uses the proficiency bonus
        var arcanaProficiency = ElementBuilder.Create(ElementTypeConstants.Proficiency, $"{arcana.Name} Proficiency")
            .WithStatisticRule($"{arcana.Name.ToLowerInvariant().Replace(" ", "-")}:proficiency", "proficiency", "proficiency")
            .Build();

        var athleticsProficiency = ElementBuilder.Create(ElementTypeConstants.Proficiency, $"{athletics.Name} Proficiency")
            .WithStatisticRule($"{athletics.Name.ToLowerInvariant().Replace(" ", "-")}:proficiency", "proficiency", "proficiency")
            .Build();

        var stealthProficiency = ElementBuilder.Create(ElementTypeConstants.Proficiency, $"{stealth.Name} Proficiency")
            .WithStatisticRule($"{stealth.Name.ToLowerInvariant().Replace(" ", "-")}:proficiency", "proficiency", "proficiency")
            .Build();

        repository.Add(arcanaProficiency);
        repository.Add(athleticsProficiency);
        repository.Add(stealthProficiency);

        return skillsRule;
    }

    private static void CreateClasses(IElementsRepository repository)
    {
        var barbarianFeature1 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Barbarian Feature 1")
            .WithComponent(id => new SortingComponent(id, 1))
            .WithStatisticRule("barbarian-stat", "1", "", 0)
            .WithDescription("This is the first feature of the Barbarian class.")
            .Build();

        var barbarianFeature2 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Barbarian Feature 2")
            .WithComponent(id => new SortingComponent(id, 2))
            .WithStatisticRule("barbarian-stat", "2", "base", 2)
            .WithDescription("This is the second feature of the Barbarian class.")
            .Build();

        var barbarianFeature2_1 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Barbarian Feature 2.1")
            .WithComponent(id => new SortingComponent(id, 2.1))
            .WithStatisticRule("barbarian-stat", "2", "", 0)
            .WithDescription("This is a nested feature of the second feature of the Barbarian class.")
            .Build();

        var barbarianFeature3 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Barbarian Feature 3")
            .WithComponent(id => new SortingComponent(id, 3))
            .WithStatisticRule("barbarian-stat", "4", "base", 3)
            .WithDescription("This is the third feature of the Barbarian class.")
            .Build();

        var barbarian = ElementBuilder.Create(ElementTypeConstants.Class, "Barbarian")
            .WithDescription("This is the class description.")
            .WithIncludeRule(barbarianFeature1.Id, levelRequirement: 1)
            .WithIncludeRule(barbarianFeature2.Id, levelRequirement: 2)
            .WithIncludeRule(barbarianFeature2_1.Id, levelRequirement: 2)
            .WithIncludeRule(barbarianFeature3.Id, levelRequirement: 3)
            .WithSelectionRule(ElementTypeConstants.SubClass, "Primal Path", levelRequirement: 3)
            .Build();

        var subclass1 = ElementBuilder.Create(ElementTypeConstants.SubClass, "Barbarian SubClass 1")
            .WithDescription("This is a barbarian subclass")
            .WithStatisticRule("barbarian-stat", "10", "base", 0)
            .WithStatisticRule("extra-stat", "5", "base", 0)
            .WithSelectionRule("Proficiency", "Skill Proficiency A")
            .Build();

        var subclass2 = ElementBuilder.Create(ElementTypeConstants.SubClass, "Barbarian SubClass 2")
            .WithDescription("This is another barbarian subclass")
            .WithStatisticRule("barbarian-stat", "20", "base", 0)
            .WithStatisticRule("extra-stat", "5", "base", 0)
            .WithSelectionRule("Proficiency", "Skill Proficiency B")
            .Build();

        var subclass3 = ElementBuilder.Create(ElementTypeConstants.SubClass, "Path of the Strong Dude")
            .WithDescription("This is yet another barbarian subclass")
            .WithStatisticRule("barbarian-stat", "proficiency", "base", 0)
            .WithStatisticRule("strength", "2", "strong-dude", 0)
            .WithStatisticRule("constitution", "2", "strong-dude", 0)
            .WithStatisticRule("intimidation", "proficiency", "strong-dude", 0)
            .WithStatisticRule("athletics:misc", "2", "strong-dude", 0)
            .WithStatisticRule("hp:dude", "constitution:modifier", "strong-dude", 0)
            .WithStatisticRule("hp", "hp:dude", "strong-dude", 0)
            .WithStatisticRule("hp", "hp:dude", "strong-dude2", 0)
            .WithSelectionRule("Proficiency", "Skill Proficiency (Strong Dude)")
            .Build();


        repository.Add(barbarian);
        repository.Add(barbarianFeature1);
        repository.Add(barbarianFeature2);
        repository.Add(barbarianFeature2_1);
        repository.Add(barbarianFeature3);
        repository.Add(subclass1);
        repository.Add(subclass2);
        repository.Add(subclass3);


        var rogueFeature1 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Rogue Feature 1")
            .Build();

        var rogueFeature2 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Rogue Feature 2")
            .Build();

        var rogueFeature3 = ElementBuilder.Create(ElementTypeConstants.ClassFeature, "Rogue Feature 3")
            .Build();

        repository.Add(rogueFeature1);
        repository.Add(rogueFeature2);
        repository.Add(rogueFeature3);

        var rogue = ElementBuilder.Create(ElementTypeConstants.Class, "Rogue")
            .WithIncludeRule(rogueFeature1.Id, levelRequirement: 1)
            .WithIncludeRule(rogueFeature2.Id, levelRequirement: 2)
            .WithIncludeRule(rogueFeature3.Id, levelRequirement: 3)
            .Build();

        repository.Add(rogue);
    }

    private static void CreateSpecies(IElementsRepository repository)
    {
        var humanFeature = ElementBuilder.Create(ElementTypeConstants.SpeciesFeature, "Human Feature")
            .Build();

        var human = ElementBuilder.Create(ElementTypeConstants.Species, "Human")
            .WithIncludeRule(humanFeature.Id, levelRequirement: 3)
            .Build();

        repository.Add(humanFeature);
        repository.Add(human);

        var elfFeature = ElementBuilder.Create(ElementTypeConstants.SpeciesFeature, "Elf Feature")
            .Build();

        var elf = ElementBuilder.Create(ElementTypeConstants.Species, "Elf")
            .WithIncludeRule(elfFeature.Id, levelRequirement: 3)
            .Build();

        repository.Add(elfFeature);
        repository.Add(elf);
    }

    private static void CreateBackgrounds(IElementsRepository repository)
    {
        var acolyteFeature = ElementBuilder.Create(ElementTypeConstants.BackgroundFeature, "Acolyte Feature")
            .Build();

        var acolyte = ElementBuilder.Create(ElementTypeConstants.Background, "Acolyte")
            .WithIncludeRule(acolyteFeature.Id, 3)
            .Build();

        repository.Add(acolyteFeature);
        repository.Add(acolyte);


        var charlatanFeature = ElementBuilder.Create(ElementTypeConstants.BackgroundFeature, "Charlatan Feature")
            .Build();

        var charlatan = ElementBuilder.Create(ElementTypeConstants.Background, "Charlatan")
            .WithIncludeRule(charlatanFeature.Id, 3)
            .Build();

        repository.Add(charlatanFeature);
        repository.Add(charlatan);
    }

    private static void CreateAlignments(IElementsRepository repository)
    {
        var lawfulGood = ElementBuilder.Create(ElementTypeConstants.Alignment, "Lawful Good")
            .WithAbbreviationComponent("LG")
            .Build();

        repository.Add(lawfulGood);

        var neutralGood = ElementBuilder.Create(ElementTypeConstants.Alignment, "Neutral Good")
            .WithAbbreviationComponent("NG")
            .Build();

        repository.Add(neutralGood);

        var chaoticGood = ElementBuilder.Create(ElementTypeConstants.Alignment, "Chaotic Good")
            .WithAbbreviationComponent("CG")
            .Build();

        repository.Add(chaoticGood);

        var lawfulNeutral = ElementBuilder.Create(ElementTypeConstants.Alignment, "Lawful Neutral")
            .WithAbbreviationComponent("LN")
            .Build();

        repository.Add(lawfulNeutral);

        var trueNeutral = ElementBuilder.Create(ElementTypeConstants.Alignment, "True Neutral")
            .WithAbbreviationComponent("TN")
            .Build();

        repository.Add(trueNeutral);

        var chaoticNeutral = ElementBuilder.Create(ElementTypeConstants.Alignment, "Chaotic Neutral")
            .WithAbbreviationComponent("CN")
            .Build();

        repository.Add(chaoticNeutral);

        var lawfulEvil = ElementBuilder.Create(ElementTypeConstants.Alignment, "Lawful Evil")
            .WithAbbreviationComponent("LE")
            .Build();

        repository.Add(lawfulEvil);

        var neutralEvil = ElementBuilder.Create(ElementTypeConstants.Alignment, "Neutral Evil")
            .WithAbbreviationComponent("NE")
            .Build();

        repository.Add(neutralEvil);

        var chaoticEvil = ElementBuilder.Create(ElementTypeConstants.Alignment, "Chaotic Evil")
            .WithAbbreviationComponent("CE")
            .Build();

        repository.Add(chaoticEvil);

        var unaligned = ElementBuilder.Create(ElementTypeConstants.Alignment, "Unaligned")
            .WithAbbreviationComponent("U")
            .Build();

        repository.Add(unaligned);
    }
}
