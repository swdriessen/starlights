using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.Create;
using Starlights.Modules.Elements.Endpoints.Entities.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageSavingThrowsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;

    public ManageSavingThrowsDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
    }

    public async Task<Guid> CreateSavingThrowAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        if (!_elementsContext.CreatedMap.TryGetValue(properties.AbilityName, out var abilityId))
        {
            throw new KeyNotFoundException($"No ability score found with name '{properties.AbilityName}'.");
        }

        using var client = _integration.CreateClient();

        var request = new CreateSavingThrowRequest(properties.Name, abilityId);

        var response = await client.PostAsJsonAsync("/api/elements/saving-throws", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateSavingThrowResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, payload.Id);

        if (storeAsLastCreated)
        {
            _integration.Set(payload.Id, "last-created-saving-throw-id");
            _integration.Set(properties, "last-created-saving-throw-properties");
        }

        return payload.Id;
    }

    public async Task<IReadOnlyList<SavingThrowListItem>> GetSavingThrowsAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/saving-throws", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetSavingThrowsResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.SavingThrows.Should().NotBeNull();

        return payload.SavingThrows;
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
