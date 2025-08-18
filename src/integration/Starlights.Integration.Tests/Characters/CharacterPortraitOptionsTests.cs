using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterPortraitOptionsTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;

    public CharacterPortraitOptionsTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestMethod]
    public async Task GetCharacterPortraitOptions_EnsureSuccessStatusCode()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/characters/portrait-options", TestCancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public async Task GetCharacterPortraitOptions_NotEmpty()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/characters/portrait-options", TestCancellationToken);
        var responseJson = await response.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>(TestCancellationToken);

        // Assert
        responseJson?.Portraits.Should().NotBeEmpty();
    }
}