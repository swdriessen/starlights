using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.GetSkills;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Skills.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageSkillsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly EventObserverCollection _events;
    private readonly ManageSkillsEndpointDriver _skillsEndpoint;

    public ManageSkillsDriver(IIntegrationHost integration, ElementsDriverContext driverContext)
    {
        _integration = integration;
        _driverContext = driverContext;

        _skillsEndpoint = _integration.GetDriver<ManageSkillsEndpointDriver>();
        _events = _integration.Events;
    }

    public async Task<Guid> CreateSkillAsync(CreateProperties properties)
    {
        var existingAbility = _driverContext.GetElement(properties.AbilityName);

        var request = new CreateSkillRequest(properties.Name, existingAbility.Id);

        var id = await _skillsEndpoint.CreateAsync(request);

        _driverContext.WithCreatedElement(id, request.Name, ElementTypeConstants.Skill);

        await _events.EnsureObservation<ElementCreatedEvent>(e => e.ElementId == id && e.Type == ElementTypeConstants.Skill);

        return id;
    }

    public async Task<IReadOnlyList<SkillListItem>> GetSkillsAsync()
    {
        var payload = await _skillsEndpoint.GetListAsync();
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

        await _skillsEndpoint.UpdateAsync(request);
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
