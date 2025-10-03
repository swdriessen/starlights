using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class CharacterPortraitOptionsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
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