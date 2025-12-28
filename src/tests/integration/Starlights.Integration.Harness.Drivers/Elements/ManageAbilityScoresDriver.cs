using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.GetList;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageAbilityScoresDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;

    public ManageAbilityScoresDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
    }

    public async Task<Guid> CreateAbilityScoreAsync(CreateProperties properties, bool storeAsLastCreated = true)
    {
        using var client = _integration.CreateClient();

        var request = new CreateAbilityScoreRequest(properties.Name, properties.Abbreviation, properties.Description);

        var response = await client.PostAsJsonAsync("/api/elements/ability-scores/create", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, payload.Id);

        if (storeAsLastCreated)
        {
            _integration.Set(payload.Id, "last-created-ability-score-id");
            _integration.Set(properties, "last-created-ability-score-properties");
        }

        return payload.Id;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByIdAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/ability-scores/{id}", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AbilityScoreDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        return payload!;
    }

    public async Task<AbilityScoreDataModel> GetAbilityScoreByNameAsync(string name)
    {
        return !_elementsContext.CreatedMap.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No ability score found with name '{name}'.")
            : await GetAbilityScoreByIdAsync(id);
    }

    public async Task<IReadOnlyList<AbilityScoreDataModel>> GetAbilityScoresAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/ability-scores", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetAbilityScoresResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();

        return [.. payload.Items];
    }

    public async Task UpdateAbilityScoreAsync(Guid id, UpdateProperties properties)
    {
        using var client = _integration.CreateClient();

        var current = await GetAbilityScoreByIdAsync(id);

        var request = new UpdateAbilityScoreRequest(
            Id: id,
            Name: properties.Name ?? current.Name,
            Abbreviation: properties.Abbreviation ?? current.Abbreviation,
            Description: properties.Description ?? current.Description);

        var response = await client.PutAsJsonAsync($"/api/elements/ability-scores/{id}", request, _integration.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

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
