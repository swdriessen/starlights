using AwesomeAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class CharacterComponentsTests
{
    [TestMethod]
    public void AddComponent_ShouldAdd_WhenParentMatches()
    {
        // Arrange
        var character = Character.Create("Test");
        var progression = ProgressionComponent.Create(character.Id);

        // Act
        character.AddComponent(progression);

        // Assert
        character.Components.Should().Contain(progression);
        character.GetRequiredComponent<ProgressionComponent>().Should().BeSameAs(progression);
        character.GetComponents<ProgressionComponent>().Should().ContainSingle().Which.Should().BeSameAs(progression);
    }

    [TestMethod]
    public void AddComponent_ShouldThrow_WhenParentDoesNotMatch()
    {
        // Arrange
        var character1 = Character.Create("One");
        var character2 = Character.Create("Two");
        var progression = ProgressionComponent.Create(character1.Id);

        // Act
        var act = () => character2.AddComponent(progression);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void GetRequiredComponent_ShouldThrow_WhenNotPresent()
    {
        // Arrange
        var character = Character.Create("Test");

        // Act
        var act = () => character.GetRequiredComponent<ProgressionComponent>();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void GetComponents_ShouldReturnEmpty_WhenNonePresent()
    {
        // Arrange
        var character = Character.Create("Test");

        // Act
        var components = character.GetComponents<ProgressionComponent>();

        // Assert
        components.Should().BeEmpty();
    }
}
