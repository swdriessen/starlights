using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.Create;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageSkillsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageSkillsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Create a new skill via the API <code>/api/elements/skills</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateSkillRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/skills", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadFromJsonAsync<CreateSkillResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();
        responseContent!.Id.Should().NotBeEmpty();

        return responseContent.Id;
    }

    /// <summary>
    /// Retrieve skills via the API <code>/api/elements/skills</code>
    /// </summary>
    public async Task<GetSkillsResponse> GetListAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/skills", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetSkillsResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    /// <summary>
    /// Update a skill via the API <code>/api/elements/skills/{id}</code>
    /// </summary>
    public async Task UpdateAsync(UpdateSkillRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/skills/{request.Id}", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
