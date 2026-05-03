using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageAbilityScoresDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly ManageAbilityScoresEndpointDriver _api;
    private readonly EventObserverCollection _events;

    public ManageAbilityScoresDriver(IIntegrationHost integration, ElementsDriverContext driverContext)
    {
        _integration = integration;
        _driverContext = driverContext;

        _api = _integration.GetDriver<ManageAbilityScoresEndpointDriver>();
        _events = _integration.Events;
    }

    public async Task<Guid> CreateAbilityScoreAsync(CreateProperties properties)
    {
        var request = new CreateAbilityScoreRequest(properties.Name, properties.Abbreviation, properties.Description);

        var id = await _api.CreateAsync(request);

        await _events.EnsureObservation<ElementCreatedEvent>(e => e.ElementId == id && e.Type == ElementTypeConstants.Ability);

        _driverContext.WithCreatedElement(id, request.Name);

        return id;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByIdAsync(Guid id)
    {
        return await _api.GetByIdAsync(id);
    }

    public async Task<AbilityScoreDataModel> GetLastCreatedAbilityScore()
    {
        return await _api.GetByIdAsync(_driverContext.CurrentElement.Id);
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByNameAsync(string name)
    {
        var element = _driverContext.GetElement(name);
        return await _api.GetByIdAsync(element.Id);
    }

    public async Task<IReadOnlyList<AbilityScoreDataModel>> GetAbilityScoresAsync()
    {
        var payload = await _api.GetListAsync();
        payload.Items.Should().NotBeNull();

        return [.. payload.Items!];
    }

    public async Task UpdateAbilityScoreAsync(Guid id, UpdateProperties properties)
    {
        var current = await GetAbilityScoreByIdAsync(id);

        var request = new UpdateAbilityScoreRequest(
            Id: id,
            Name: properties.Name ?? current.Name,
            Abbreviation: properties.Abbreviation ?? current.Abbreviation,
            Description: properties.Description ?? current.Description);

        await _api.PutAsync(request);

        _integration.Set(request, "last-updated-ability-score-request");
    }

    public sealed record CreateProperties
    {
        public required string Name { get; init; }
        public required string Abbreviation { get; init; }
        public string? Description { get; init; }
    }

    public sealed record UpdateProperties
    {
        public static UpdateProperties FromDictionary(IReadOnlyDictionary<string, string?> values)
        {
            values.TryGetValue("name", out var name);
            values.TryGetValue("abbreviation", out var abbreviation);
            values.TryGetValue("description", out var description);

            return new UpdateProperties
            {
                Name = string.IsNullOrWhiteSpace(name) ? null : name,
                Abbreviation = string.IsNullOrWhiteSpace(abbreviation) ? null : abbreviation,
                Description = description
            };
        }

        public string? Name { get; init; }
        public string? Abbreviation { get; init; }
        public string? Description { get; init; }
    }
}
