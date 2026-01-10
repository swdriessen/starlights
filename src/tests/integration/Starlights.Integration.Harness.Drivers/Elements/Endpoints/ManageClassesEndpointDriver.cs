using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageClassesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageClassesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<Guid> CreateAsync(CreateClassRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return payload.Id;
    }

    public async Task<ClassDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/classes/{id}", _integration.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<ClassDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        return payload;
    }
}
