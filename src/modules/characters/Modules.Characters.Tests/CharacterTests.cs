using FluentAssertions;
using Starlights.Modules.Characters.Domain;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class CharacterTests
{
    [TestMethod]
    public void CreateCharacter()
    {
        // Arrange
        const string name = "Test Character";

        // Act
        var character = new Character(name);

        // Assert
        character.Name.Should().Be(name);
        character.Id.Value.Should().NotBeEmpty();
    }
}
