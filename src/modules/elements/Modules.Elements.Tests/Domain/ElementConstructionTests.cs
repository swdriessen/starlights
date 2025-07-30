using FluentAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain;

/// <summary>
/// Tests for the construction of elements in the domain, the assertion is that elements can be created with various components.
/// </summary>
[TestClass]
public class ElementConstructionTests
{
    [TestMethod]
    public void CharacterCreation()
    {
        // Act
        var element = Element.Create("Name", ElementTypeConstants.CharacterCreation);
        element.AddComponent(new ShortDescriptionComponent(element.Id, "short description"));
        element.AddComponent(new DescriptionComponent(element.Id, "long description"));

        // Assert
        element.Should().NotBeNull();
    }

    [TestMethod]
    public void Ability()
    {
        // Act
        var element = Element.Create("Strength", ElementTypeConstants.Ability);
        element.AddComponent(new AbbreviationComponent(element.Id, "STR"));
        element.AddComponent(new DescriptionComponent(element.Id, "Strength is a measure of physical power and athleticism."));

        // Assert
        element.Should().NotBeNull();
    }

    [TestMethod]
    public void Skill()
    {
        // Arrange
        var abilityElement = Element.Create("Intelligence", ElementTypeConstants.Ability);
        abilityElement.AddComponent(new AbbreviationComponent(abilityElement.Id, "INT"));

        // Act
        var skillElement = Element.Create("Arcana", ElementTypeConstants.Skill);
        skillElement.AddComponent(new PrimaryAbilityComponent(skillElement.Id, abilityElement.Id));
        skillElement.AddComponent(new DescriptionComponent(skillElement.Id, "Arcana is a measure of magical knowledge and ability."));

        // Assert
        skillElement.Should().NotBeNull();
    }

    [TestMethod]
    public void SavingThrow()
    {
        // Arrange
        var abilityElement = Element.Create("Intelligence", ElementTypeConstants.Ability);
        abilityElement.AddComponent(new AbbreviationComponent(abilityElement.Id, "INT"));

        // Act
        var saveElement = Element.Create("Intelligence", ElementTypeConstants.SavingThrow);
        saveElement.AddComponent(new PrimaryAbilityComponent(saveElement.Id, abilityElement.Id));
        saveElement.AddComponent(new DescriptionComponent(saveElement.Id, "Intelligence saving throw is used to resist effects that target mental acuity."));

        // Assert
        saveElement.Should().NotBeNull();
    }

    [TestMethod]
    public void Language()
    {
        // Act
        var element = Element.Create("Common", ElementTypeConstants.Language);
        element.AddComponent(new LanguageComponent(element.Id, "Sigil"));
        element.AddComponent(new DescriptionComponent(element.Id, "Common is the most widely spoken language in the world."));

        // Assert
        element.Should().NotBeNull();
    }

    [TestMethod]
    public void ElementWithIncludeRule()
    {
        // Arrange
        var freeElement = Element.Create("Free Element", "Type");

        // Act
        var element = Element.Create("Advanced Element", "Rule");
        element.AddComponent(new IncludeRuleComponent(element.Id, freeElement.Id, 0));

        // Assert
        element.Should().NotBeNull();
        var includeRule = element.GetComponent<IncludeRuleComponent>();
        includeRule.IncludeElement.Should().Be(freeElement.Id);
        includeRule.LevelRequirement.Should().Be(0);
    }
}
