using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class ProgressionComponentTests
{
    [TestMethod]
    public void DefaultLevel_ShouldBeZero()
    {
        // Arrange
        var character = Character.Create("Test");

        // Act
        var progression = new ProgressionComponent(character.Id);

        // Assert
        progression.CharacterLevel.Should().Be(0);
    }

    [TestMethod]
    public void SetCharacterLevel_ShouldUpdateLevel_WhenValid()
    {
        // Arrange
        var character = Character.Create("Test");
        var progression = new ProgressionComponent(character.Id);

        // Act
        progression.SetCharacterLevel(5);

        // Assert
        progression.CharacterLevel.Should().Be(5);
    }

    [TestMethod]
    public void SetCharacterLevel_ShouldThrow_WhenNegative()
    {
        // Arrange
        var character = Character.Create("Test");
        var progression = new ProgressionComponent(character.Id);

        // Act
        var act = () => progression.SetCharacterLevel(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
