using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageAbilityScoresDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;
    private readonly ManageAbilityScoresEndpointDriver _endpoints;

    public ManageAbilityScoresDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
        _endpoints = _integration.GetDriver<ManageAbilityScoresEndpointDriver>();
    }

    public async Task<Guid> CreateAbilityScoreAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        var request = new CreateAbilityScoreRequest(properties.Name, properties.Abbreviation, properties.Description);

        var id = await _endpoints.CreateAsync(request);

        _elementsContext.ElementCreated(properties.Name, id);

        if (storeAsLastCreated)
        {
            _integration.Set(id, "last-created-ability-score-id");
            _integration.Set(properties, "last-created-ability-score-properties");
        }

        return id;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByIdAsync(Guid id)
    {
        return await _endpoints.GetByIdAsync(id);
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByNameAsync(string name)
    {
        return !_elementsContext.CreatedMap.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No ability score found with name '{name}'.")
            : await GetAbilityScoreByIdAsync(id);
    }

    public async Task<IReadOnlyList<AbilityScoreDataModel>> GetAbilityScoresAsync()
    {
        var payload = await _endpoints.GetListAsync();
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

        await _endpoints.PutAsync(request);

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
