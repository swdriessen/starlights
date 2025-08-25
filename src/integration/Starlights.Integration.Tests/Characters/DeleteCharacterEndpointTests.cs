using System.Net;
using FluentAssertions;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class DeleteCharacterEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;

    public DeleteCharacterEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();
        await client.InitializeElementsAsync(TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task DeleteCharacter_Removes_Character_From_List()
    {
        // Arrange
        var client = _integration.CreateClient();

        var options = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        options.Options.Should().NotBeEmpty();

        var portraits = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);
        portraits.Portraits.Should().NotBeEmpty();

        var character = await client.CreateCharacterAsync(options.Options[0].Id, $"ITest {Guid.NewGuid()}", portraits.Portraits[0].Url, TestCancellationToken);

        // Ensure it exists
        var before = await client.GetCharactersAsync(TestCancellationToken);
        before.Characters.Should().Contain(c => c.CharacterId == character.Id);

        // Act
        await client.DeleteCharacterAsync(character.Id, HttpStatusCode.NoContent, TestCancellationToken);

        // Assert - not present anymore
        var after = await client.GetCharactersAsync(TestCancellationToken);
        after.Characters.Should().NotContain(c => c.CharacterId == character.Id);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task DeleteCharacter_Unknown_Id_Returns_NotFound()
    {
        // Arrange
        var client = _integration.CreateClient();
        var unknown = Guid.NewGuid();

        // Act
        var response = await client.DeleteCharacterAsync(unknown, HttpStatusCode.NotFound, TestCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
