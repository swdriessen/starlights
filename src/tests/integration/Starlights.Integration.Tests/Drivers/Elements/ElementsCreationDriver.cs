using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ElementsCreationDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsEndpointDriver _api;

    public ElementsCreationDriver(IIntegrationHost integration, ElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public Task<Guid> CreateSpellAsync(CreateSpellProperties properties)
    {
        return _api.CreateSpellAsync(
            properties.Name,
            properties.Level,
            properties.MagicSchool,
            properties.CastingTime,
            properties.Range,
            properties.Duration,
            properties.IsConcentration,
            properties.IsRitual,
            properties.HasSomatic,
            properties.HasVerbal,
            properties.HasMaterial,
            properties.MaterialComponent,
            properties.Description);
    }

    public Task<GetSpellByIdResponse?> GetSpellByIdAsync(Guid id)
    {
        return _api.GetSpellByIdAsync(id);
    }

    public Task<UpdateSpellResponse> UpdateSpellAsync(UpdateSpellRequest request)
    {
        return _api.UpdateSpellAsync(request);
    }

    public async Task<List<GetSpellsResponseItem>> GetSpellsAsync()
    {
        var spells = await _api.GetSpellsAsync();
        _integration.WriteLine($"retrieved {spells.Count} spells.");
        return spells;
    }
}
