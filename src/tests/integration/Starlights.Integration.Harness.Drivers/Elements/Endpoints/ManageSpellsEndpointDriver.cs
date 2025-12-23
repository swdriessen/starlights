using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Create;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public class ManageSpellsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageSpellsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Get all spells via the API <code>/api/elements/spells</code>
    /// </summary>
    /// <returns></returns>
    public async Task<List<SpellDataModel>> GetAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/spells", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetSpellsResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload.Items.Should().NotBeNull("expected Items to be not null");

        return [.. payload.Items];
    }

    /// <summary>
    /// Create a new spell via the API <code>/api/elements/spells/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateSpellRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/spells/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateSpellResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent.Id;
    }

    /// <summary>
    /// Get a spell by ID via the API <code>/api/elements/spells/{id}</code>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SpellDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/spells/{id}", _integration.CancellationToken);

        if (response.IsSuccessStatusCode)
        {
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<SpellDataModel>(_integration.CancellationToken);
            responseContent.Should().NotBeNull();

            return responseContent;
        }

        return null;
    }

    /// <summary>
    /// Update an existing spell via the API <code>/api/elements/spells/{id}</code>
    /// </summary>
    public async Task<bool> PutAsync(UpdateSpellRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/spells/{request.Id}", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateSpellResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return true;
    }
}
