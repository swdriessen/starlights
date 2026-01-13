using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageSubClassesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageSubClassesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<Guid> CreateAsync(CreateSubClassRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/sub-classes/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CreateSubClassResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return payload!.Id;
    }

    public async Task<SubClassDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/sub-classes/{id}", _integration.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<SubClassDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        return payload;
    }
}
