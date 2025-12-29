using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Skills.Create;
using Starlights.Modules.Elements.Endpoints.Content.Skills.GetSkills;
using Starlights.Modules.Elements.Endpoints.Content.Skills.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageSkillsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;
    private readonly ManageSkillsEndpointDriver _endpoints;

    public ManageSkillsDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
        _endpoints = _integration.GetDriver<ManageSkillsEndpointDriver>();
    }

    public async Task<Guid> CreateSkillAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        if (!_elementsContext.CreatedMap.TryGetValue(properties.AbilityName, out var abilityId))
        {
            throw new KeyNotFoundException($"No ability score found with name '{properties.AbilityName}'.");
        }

        var request = new CreateSkillRequest(properties.Name, abilityId);

        var id = await _endpoints.CreateAsync(request);

        _elementsContext.ElementCreated(properties.Name, id);

        if (storeAsLastCreated)
        {
            _integration.Set(id, "last-created-skill-id");
            _integration.Set(properties, "last-created-skill-properties");
        }

        return id;
    }

    public async Task<IReadOnlyList<SkillListItem>> GetSkillsAsync()
    {
        var payload = await _endpoints.GetListAsync();
        payload.Skills.Should().NotBeNull();

        return payload.Skills!;
    }

    public async Task<SkillListItem> GetSkillByIdAsync(Guid id)
    {
        var items = await GetSkillsAsync();
        return items.Single(x => x.Id == id);
    }

    public async Task UpdateSkillAsync(Guid id, UpdateProperties properties)
    {
        var current = await GetSkillByIdAsync(id);

        var request = new UpdateSkillRequest(
            Id: id,
            Name: properties.Name ?? current.Name,
            AbilityId: current.AbilityId,
            Description: properties.Description ?? current.Description);

        if (properties.AbilityName is not null)
        {
            var ability = await _integration.GetDriver<ManageAbilityScoresDriver>().GetAbilityScoreByNameAsync(properties.AbilityName);
            request = request with { AbilityId = ability.Id };
        }

        await _endpoints.UpdateAsync(request);
    }

    public sealed record CreateProperties
    {
        public required string Name { get; init; }
        public required string AbilityName { get; init; }
    }

    public sealed record UpdateProperties
    {
        public string? Name { get; init; }
        public string? AbilityName { get; init; }
        public string? Description { get; init; }

        public static UpdateProperties FromDictionary(IReadOnlyDictionary<string, string?> values)
        {
            values.TryGetValue("name", out var name);
            values.TryGetValue("ability", out var ability);
            values.TryGetValue("description", out var description);

            return new UpdateProperties
            {
                Name = name,
                AbilityName = ability,
                Description = description
            };
        }
    }
}
