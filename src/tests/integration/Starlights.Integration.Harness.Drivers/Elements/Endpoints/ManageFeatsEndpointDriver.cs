using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Feats;
using Starlights.Modules.Elements.Endpoints.Content.Feats.Create;
using Starlights.Modules.Elements.Endpoints.Content.Feats.GetList;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public class ManageFeatsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageFeatsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new feat via the API <code>/api/elements/feats/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateFeatRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/feats/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateFeatResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent.Id;
    }

    /// <summary>
    /// Retrieve a feat via the API <code>/api/elements/feats/{id}</code>
    /// </summary>
    public async Task<FeatDataModel> GetByIdAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/feats/{id}", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<FeatDataModel>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent;
    }

    /// <summary>
    /// Retrieve feats via the API <code>/api/elements/feats</code>
    /// </summary>
    public async Task<GetFeatsResponse> GetListAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/feats", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetFeatsResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent;
    }
}
