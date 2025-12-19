using Starlights.Integration.Core;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Spells.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Drivers.Elements;

internal class ElementsCreationDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsEndpointDriver _api;

    public ElementsCreationDriver(IIntegrationHost integration, ElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public Task<Guid> CreateSpellAsync(string name, int level, string school, string time, string range, string duration, bool isConcentration,
        bool isRitual, bool hasSomatic, bool hasVerbal, bool hasMaterial, string? materialComponent = null, string? description = null)
    {
        return _api.CreateSpellAsync(name, level, school, time, range, duration, isConcentration,
            isRitual, hasSomatic, hasVerbal, hasMaterial, materialComponent, description);
    }

    public Task<GetSpellByIdResponse?> GetSpellByIdAsync(Guid id)
    {
        return _api.GetSpellByIdAsync(id);
    }

    public Task<UpdateSpellResponse> UpdateSpellAsync(UpdateSpellRequest request)
    {
        return _api.UpdateSpellAsync(request);
    }

    public Task<GetSpellsResponse> GetSpellsAsync()
    {
        return _api.GetSpellsAsync();
    }
}

internal class ElementsManagementDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsEndpointDriver _api;

    public ElementsManagementDriver(IIntegrationHost integration, ElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }
}
