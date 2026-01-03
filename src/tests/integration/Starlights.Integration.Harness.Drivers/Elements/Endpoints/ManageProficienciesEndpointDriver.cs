using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.GetProficiencies;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageProficienciesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageProficienciesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new proficiency via the API <code>/api/elements/proficiencies</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateProficiencyRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/proficiencies", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadFromJsonAsync<CreateProficiencyResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();
        responseContent!.Id.Should().NotBeEmpty();

        return responseContent.Id;
    }

    /// <summary>
    /// Retrieve proficiencies via the API <code>/api/elements/proficiencies</code>
    /// </summary>
    public async Task<GetProficienciesResponse> GetListAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/proficiencies", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetProficienciesResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Update a proficiency via the API <code>/api/elements/proficiencies/{id}</code>
    /// </summary>
    public async Task UpdateAsync(UpdateProficiencyRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/proficiencies/{request.Id}", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Delete a proficiency via the API <code>/api/elements/proficiencies/{id}</code>
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.DeleteAsync($"/api/elements/proficiencies/{id}", _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
