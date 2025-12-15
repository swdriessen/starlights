using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class GetCharactersEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _driver = _integration.GetDriver<CharacterCreationDriver>();

        await _integration.InitializeElements();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetCharacters_Returns_List_Including_Created_Character()
    {
        // Arrange
        var characterId = await _driver.CreateCharacterAsync();

        // Act
        var characters = await _driver.GetCharacters();

        // Assert
        characters.Should().Contain(c => c.CharacterId == characterId);
        characters.Should().AllSatisfy(c =>
        {
            c.Name.Should().NotBeNullOrWhiteSpace();
            c.CharacterId.Should().Be(characterId);
            c.PortraitUrl.Should().NotBeNullOrWhiteSpace();
            c.Level.Should().Be(0);
            c.Build.Should().BeEmpty();
        });
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetCharacter_Returns_Created_Character()
    {
        // Arrange
        var characterId = await _driver.CreateCharacterAsync();

        // Act
        var character = await _driver.GetCharacter(characterId);

        // Assert
        character.CharacterId.Should().Be(characterId);
        character.Name.Should().NotBeNullOrWhiteSpace();
        character.PortraitUrl.Should().NotBeNullOrWhiteSpace();
        character.Level.Should().Be(0);
        character.Build.Should().BeEmpty();
    }
}
