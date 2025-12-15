using AwesomeAssertions;
using Starlights.Modules.Characters.Domain.Characters;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class CharacterTests
{
    [TestMethod]
    public void CreateCharacter()
    {
        // Arrange
        const string name = "Test Character";

        // Act
        var character = Character.Create(name);

        // Assert
        character.Name.Should().Be(name);
        character.Id.Value.Should().NotBeEmpty();
    }
}
