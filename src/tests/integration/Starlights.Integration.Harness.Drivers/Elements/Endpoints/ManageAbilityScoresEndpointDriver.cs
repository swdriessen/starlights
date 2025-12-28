using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.GetList;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageAbilityScoresEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageAbilityScoresEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new ability score via the API <code>/api/elements/ability-scores/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateAbilityScoreRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/ability-scores/create", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();
        responseContent!.Id.Should().NotBeEmpty();

        return responseContent.Id;
    }

    /// <summary>
    /// Retrieve an ability score via the API <code>/api/elements/ability-scores/{id}</code>
    /// </summary>
    public async Task<AbilityScoreDataModel> GetByIdAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/ability-scores/{id}", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<AbilityScoreDataModel>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Retrieve ability scores via the API <code>/api/elements/ability-scores</code>
    /// </summary>
    public async Task<GetAbilityScoresResponse> GetListAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/ability-scores", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetAbilityScoresResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Update an ability score via the API <code>/api/elements/ability-scores/{id}</code>
    /// </summary>
    public async Task PutAsync(UpdateAbilityScoreRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/ability-scores/{request.Id}", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
