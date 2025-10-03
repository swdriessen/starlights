using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows;
using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class SavingThrowEndpointDriver
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

internal sealed class SavingThrowDriver
{
    private readonly IIntegrationHost _integration;
    private readonly SavingThrowEndpointDriver _api;

    public SavingThrowDriver(IIntegrationHost integration, SavingThrowEndpointDriver api)
    {
        _integration = integration;
        _api = api;
    }

    public async Task<List<SavingThrowDataModel>> GetSavingThrows()
    {
        var response = await _api.GetSavingThrows();
        response.SavingThrows.Should().NotBeNull();
        return response.SavingThrows;
    }
}