using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharactersEndpointTests
{
    private readonly IntegrationHost _integration;

    public CharactersEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            //.WithConsoleActivityProcessor()
            .Build();
    }

    [TestMethod]
    public async Task CreateCharacter()
    {
        // Arrange
        var client = _integration.CreateClient();
        var request = new CreateCharacterRequest
        {
            Name = "Test Character"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/characters/create", request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
