using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Labels.Create;
using Starlights.Modules.Elements.Endpoints.Content.Labels.Delete;
using Starlights.Modules.Elements.Endpoints.Content.Labels.List;
using Starlights.Modules.Elements.Endpoints.Content.Labels.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageElementLabelsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageElementLabelsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create one or more labels for an element via the API <code>/api/elements/{elementId}/labels</code>
    /// </summary>
    public async Task<HttpStatusCode> CreateAsync(Guid elementId, IReadOnlyCollection<string> labels)
    {
        using var client = _integration.CreateClient();

        var request = new CreateElementLabelRequest(labels.ToArray());

        var response = await client.PostAsJsonAsync($"/api/elements/{elementId}/labels", request, _integration.CancellationToken);
        return response.StatusCode;
    }

    /// <summary>
    /// List labels for an element via the API <code>/api/elements/{elementId}/labels</code>
    /// </summary>
    public async Task<IReadOnlyList<string>> ListAsync(Guid elementId)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{elementId}/labels", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ListElementLabelsResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return payload!.Labels.ToArray();
    }

    /// <summary>
    /// Delete one or more labels for an element via the API <code>/api/elements/{elementId}/labels</code>
    /// </summary>
    public async Task<HttpStatusCode> DeleteAsync(Guid elementId, IReadOnlyCollection<string> labels)
    {
        using var client = _integration.CreateClient();

        var request = new DeleteElementLabelRequest(labels.ToArray());

        var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/elements/{elementId}/labels")
        {
            Content = JsonContent.Create(request)
        };

        var response = await client.SendAsync(message, _integration.CancellationToken);
        return response.StatusCode;
    }

    /// <summary>
    /// Replace all labels for an element via the API <code>/api/elements/{elementId}/labels</code>
    /// </summary>
    public async Task<(UpdateElementLabelResponse Response, HttpStatusCode StatusCode)> UpdateAsync(Guid elementId, IReadOnlyCollection<string> labels)
    {
        using var client = _integration.CreateClient();

        var request = new UpdateElementLabelRequest(labels.ToArray());

        var response = await client.PutAsJsonAsync($"/api/elements/{elementId}/labels", request, _integration.CancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return (new UpdateElementLabelResponse([]), response.StatusCode);
        }

        var payload = await response.Content.ReadFromJsonAsync<UpdateElementLabelResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return (payload!, response.StatusCode);
    }
}
