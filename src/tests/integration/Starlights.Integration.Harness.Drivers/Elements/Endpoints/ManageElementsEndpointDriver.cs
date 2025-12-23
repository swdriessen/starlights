using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Create;
using Starlights.Modules.Elements.Endpoints.Content.Elements.GetList;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageElementsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageElementsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new element via the API <code>/api/elements/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateElementRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateElementResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!.Id;
    }

    /// <summary>
    /// Retrieve elements via the API <code>/api/elements</code>
    /// </summary>
    public async Task<GetElementsResponse> GetListAsync(string? type = null)
    {
        using var client = _integration.CreateClient();

        var url = string.IsNullOrWhiteSpace(type)
            ? "/api/elements"
            : $"/api/elements?type={Uri.EscapeDataString(type)}";

        var response = await client.GetAsync(url, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetElementsResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Retrieve an element by ID via the API <code>/api/elements/{id}</code>
    /// </summary>
    public async Task<ElementDataModel> GetByIdAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{id}", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<ElementDataModel>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }
}
