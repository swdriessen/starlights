using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Drivers.CharacterCreation.Endpoints;

internal sealed class SavingThrowEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    public SavingThrowEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Retrieves the saving throws for the current character.
    /// </summary>
    public async Task<GetSavingThrowsResponse> GetSavingThrows()
    {
        using var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/saving-throws";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetSavingThrowsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
