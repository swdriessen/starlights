using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Create;

namespace Starlights.Integration.Drivers.Elements;

internal class ElementsEndpointDriver : IDriver
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
        bool isConcentration, bool isRitual, bool hasSomatic, bool hasVerbal, bool hasMaterial, string? materialComponent = null, string? description = null)
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
}