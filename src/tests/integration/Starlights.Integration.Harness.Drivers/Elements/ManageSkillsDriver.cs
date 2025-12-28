using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.Create;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageSkillsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;

    public ManageSkillsDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
    }

    public async Task<Guid> CreateSkillAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        if (!_elementsContext.CreatedMap.TryGetValue(properties.AbilityName, out var abilityId))
        {
            throw new KeyNotFoundException($"No ability score found with name '{properties.AbilityName}'.");
        }

        using var client = _integration.CreateClient();

        var request = new CreateSkillRequest(properties.Name, abilityId);

        var response = await client.PostAsJsonAsync("/api/elements/skills", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateSkillResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, payload.Id);

        if (storeAsLastCreated)
        {
            _integration.Set(payload.Id, "last-created-skill-id");
            _integration.Set(properties, "last-created-skill-properties");
        }

        return payload.Id;
    }

    public async Task<IReadOnlyList<SkillListItem>> GetSkillsAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/skills", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetSkillsResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Skills.Should().NotBeNull();

        return payload.Skills;
    }

    public async Task<SkillListItem> GetSkillByIdAsync(Guid id)
    {
        var items = await GetSkillsAsync();
        return items.Single(x => x.Id == id);
    }

    public sealed record CreateProperties
    {
        public required string Name { get; init; }
        public required string AbilityName { get; init; }
    }
}
