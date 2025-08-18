using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterCreationOptionsTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;

    public CharacterCreationOptionsTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.GetAsync("/api/elements/initialize", TestCancellationToken);
    }

    [TestMethod]
    public async Task GetCharacterCreationOptions()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/characters/creation-options", TestCancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(TestCancellationToken);
        responseJson.Should().NotBeNull();
        responseJson.Options.Should().NotBeEmpty();
    }
}
