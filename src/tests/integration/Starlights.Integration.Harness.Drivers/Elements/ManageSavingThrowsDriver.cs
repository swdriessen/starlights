using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.Create;
using Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageSavingThrowsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;
    private readonly ManageSavingThrowsEndpointDriver _endpoints;

    public ManageSavingThrowsDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
        _endpoints = _integration.GetDriver<ManageSavingThrowsEndpointDriver>();
    }

    public async Task<Guid> CreateSavingThrowAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        if (!_elementsContext.CreatedMap.TryGetValue(properties.AbilityName, out var abilityId))
        {
            throw new KeyNotFoundException($"No ability score found with name '{properties.AbilityName}'.");
        }

        var request = new CreateSavingThrowRequest(properties.Name, abilityId);

        var id = await _endpoints.CreateAsync(request);

        _elementsContext.ElementCreated(properties.Name, id);

        if (storeAsLastCreated)
        {
            _integration.Set(id, "last-created-saving-throw-id");
            _integration.Set(properties, "last-created-saving-throw-properties");
        }

        return id;
    }

    public async Task<IReadOnlyList<SavingThrowListItem>> GetSavingThrowsAsync()
    {
        var payload = await _endpoints.GetListAsync();
        payload.SavingThrows.Should().NotBeNull();

        return payload.SavingThrows!;
    }

    public async Task<SavingThrowListItem> GetSavingThrowByIdAsync(Guid id)
    {
        var items = await GetSavingThrowsAsync();
        return items.Single(x => x.Id == id);
    }

    public sealed record CreateProperties
    {
        public required string Name { get; init; }
        public required string AbilityName { get; init; }
    }
}
