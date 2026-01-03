using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.GetSavingThrows;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageSavingThrowsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageSavingThrowsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new saving throw via the API <code>/api/elements/saving-throws</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateSavingThrowRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/saving-throws", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadFromJsonAsync<CreateSavingThrowResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();
        responseContent!.Id.Should().NotBeEmpty();

        return responseContent.Id;
    }

    /// <summary>
    /// Retrieve saving throws via the API <code>/api/elements/saving-throws</code>
    /// </summary>
    public async Task<GetSavingThrowsResponse> GetListAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/saving-throws", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetSavingThrowsResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Update a saving throw via the API <code>/api/elements/saving-throws/{{id}}</code>
    /// </summary>
    public async Task UpdateAsync(UpdateSavingThrowRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/saving-throws/{request.Id}", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
