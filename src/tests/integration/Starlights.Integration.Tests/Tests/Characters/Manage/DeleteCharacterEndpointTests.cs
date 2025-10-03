using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class DeleteCharacterEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationDriver _characterCreationDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _characterCreationDriver = _integration.GetDriver<CharacterCreationDriver>();

        await _integration.InitializeElements();
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task DeleteCharacter_Removes_Character_From_List()
    {
        // Arrange
        var characterId = await _characterCreationDriver.CreateCharacterAsync();
        var characters = await _characterCreationDriver.GetCharacters();
        characters.Should().Contain(c => c.CharacterId == characterId);

        // Act
        await _characterCreationDriver.DeleteCharacter(characterId);

        // Assert
        characters = await _characterCreationDriver.GetCharacters();
        characters.Should().NotContain(c => c.CharacterId == characterId);
    }
}
