using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Create;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ElementsEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ElementsEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task InitializeElementsAsync(CancellationToken cancellation)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/initialize", cancellation);
        response.EnsureSuccessStatusCode();

        response.Content.ReadFromJsonAsync<object>(cancellation)
            .Should().NotBeNull("expected response content to be deserializable");
    }

    public async Task<Guid> CreateSpellAsync(string name, int level, string school, string time, string range, string duration,
        bool isConcentration, bool isRitual, bool hasSomatic, bool hasVerbal, bool hasMaterial, string? materialComponent, string description)
    {
        using var client = _integration.CreateClient();

        var request = new CreateSpellRequest()
        {
            Name = name,
            Level = level,
            MagicSchool = school,
            CastingTime = time,
            Range = range,
            Duration = duration,
            IsConcentration = isConcentration,
            IsRitual = isRitual,
            HasSomatic = hasSomatic,
            HasVerbal = hasVerbal,
            HasMaterial = hasMaterial,
            MaterialComponent = materialComponent,
            Description = description
        };

        var response = await client.PostAsJsonAsync("/api/elements/spells/create", request, _integration.CancellationToken);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateSpellResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent.Id;
    }

    public async Task<SpellDataModel?> GetSpellByIdAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/spells/{id}", _integration.CancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SpellDataModel>(_integration.CancellationToken);
    }

    public async Task<UpdateSpellResponse> UpdateSpellAsync(UpdateSpellRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/spells/{request.Id}", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateSpellResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    public async Task<List<SpellDataModel>> GetSpellsAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/spells", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetSpellsResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload.Items.Should().NotBeNull("expected Items to be not null");

        return [.. payload.Items];
    }
}
