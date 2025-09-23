using FluentAssertions;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class GetCharactersEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private Guid _createdCharacterId;

    public GetCharactersEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        // ensure required elements are initialized
        await client.InitializeElementsAsync(TestCancellationToken);

        // create one character to appear in the list
        var options = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        var portraits = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);

        var name = $"Test Character {Guid.NewGuid()}";
        var created = await client.CreateCharacterAsync(options.Options[0].Id, name, portraits.Portraits[0].Url, TestCancellationToken);
        _createdCharacterId = created.Id;
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetCharacters_Returns_List_Including_Created_Character()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetCharactersAsync(TestCancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.Characters.Should().NotBeEmpty();
        response.Characters.Should().Contain(c => c.CharacterId == _createdCharacterId);

        var created = response.Characters.First(c => c.CharacterId == _createdCharacterId);
        created.Name.Should().NotBeNullOrWhiteSpace();
        created.CharacterId.Should().Be(_createdCharacterId);
        created.PortraitUrl.Should().NotBeNullOrWhiteSpace();
        created.Level.Should().Be(0);
        created.Build.Should().BeEmpty();
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetCharacter_Returns_Created_Character()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetCharacterDetailsAsync(_createdCharacterId, TestCancellationToken);

        // Assert
        response!.Character.Should().NotBeNull();
        response.Character.Name.Should().NotBeNullOrWhiteSpace();
        response.Character.PortraitUrl.Should().NotBeNullOrWhiteSpace();
        response.Character.CharacterId.Should().Be(_createdCharacterId);
        response.Character.Level.Should().Be(0);
        response.Character.Build.Should().BeEmpty();
    }
}
