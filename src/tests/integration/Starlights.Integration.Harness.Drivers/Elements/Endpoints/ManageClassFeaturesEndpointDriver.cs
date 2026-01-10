using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageClassFeaturesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageClassFeaturesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<Guid> CreateAsync(CreateClassFeatureRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/class-features/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CreateClassFeatureResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return payload!.Id;
    }

    public async Task<ClassFeatureDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/class-features/{id}", _integration.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<ClassFeatureDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        return payload;
    }
}
