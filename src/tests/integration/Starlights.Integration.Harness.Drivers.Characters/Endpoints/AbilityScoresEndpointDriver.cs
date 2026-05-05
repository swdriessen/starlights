using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.GetAbilities;
using Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateBaseScore;

namespace Starlights.Integration.Drivers.Characters.Endpoints;

internal sealed class AbilityScoresEndpointDriver : IDriver
{
    private readonly IIntegrationHost _host;
    private readonly IntegrationTestContext _context;

    public AbilityScoresEndpointDriver(IIntegrationHost host)
    {
        _host = host;
        _context = _host.IntegrationContext;
    }

    public async Task<GetAbilityScoresResponse> GetAbilityScoresAsync(Guid characterId)
    {
        using var api = _host.CreateClient();

        var url = $"/api/characters/{characterId}/ability-scores";

        var response = await api.GetAsync(url, _context.CancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetAbilityScoresResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<UpdateAbilityBaseScoreResponse> UpdateBaseScoreAsync(Guid characterId, Guid abilityScoreId, int value)
    {
        using var api = _host.CreateClient();

        var url = $"/api/characters/{characterId}/ability-scores/{abilityScoreId}/base";
        var payload = new UpdateAbilityBaseScoreRequest { Value = value };

        var response = await api.PostAsJsonAsync(url, payload, _context.CancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<UpdateAbilityBaseScoreResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }
}
