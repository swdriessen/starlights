using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.GetAbilities;
using Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class AbilityScoresEndpointsTests
{
    private readonly IntegrationHost _integration;
    private Guid _characterId;
    private string _characterName = string.Empty;

    public AbilityScoresEndpointsTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Ensure elements are seeded
        await client.GetAsync("/api/elements/initialize", CancellationToken.None);

        // Get creation options
        var optionsResponse = await client.GetAsync("/api/characters/creation-options", CancellationToken.None);
        var optionsJson = await optionsResponse.Content.ReadFromJsonAsync<GetCharacterCreationOptionsResponse>(CancellationToken.None);
        optionsJson?.Options.Should().NotBeEmpty();

        // Get portrait options
        var portraitsResponse = await client.GetAsync("/api/characters/portrait-options", CancellationToken.None);
        var portraitsJson = await portraitsResponse.Content.ReadFromJsonAsync<GetCharacterPortraitOptionsResponse>(CancellationToken.None);
        portraitsJson?.Portraits.Should().NotBeEmpty();

        _characterName = $"Integration Test Character {Guid.NewGuid()}";
        var request = new CreateCharacterRequest(optionsJson!.Options.First().Id, _characterName, portraitsJson!.Portraits.First().Url);

        // Act
        var createResponse = await client.PostAsJsonAsync("/api/characters/create", request, CancellationToken.None);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createCharacterResponseObject = await createResponse.Content.ReadFromJsonAsync<CreateCharacterResponse>(CancellationToken.None);
        _characterId = createCharacterResponseObject!.Id;

        // Warm up abilities (if event processing is async)
        await WaitForAbilitiesAsync(client, _characterId, minCount: 1, timeoutMs: 3000);
    }

    [TestMethod]
    public async Task GetAbilities_Returns_DetailedScores()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var abilities = await GetAbilitiesAsync(client, _characterId);

        // Assert
        abilities.Should().NotBeNull();
        abilities!.AbilityScores.Should().NotBeEmpty();

        var first = abilities.AbilityScores.First();
        first.AbilityScoreId.Should().NotBe(Guid.Empty);
        first.Name.Should().NotBeNullOrWhiteSpace();
        first.Abbreviation.Should().NotBeNullOrWhiteSpace();
        first.CalculatedScore.Should().Be(first.BaseScore + first.AdditionalScore);
        first.CalculatedModifier.Should().Be((int)Math.Floor((first.CalculatedScore - 10) / 2.0));
    }

    [TestMethod]
    public async Task UpdateBaseScore_Updates_CalculatedFields()
    {
        // Arrange
        var client = _integration.CreateClient();
        var abilitiesBefore = await GetAbilitiesAsync(client, _characterId);
        var target = abilitiesBefore!.AbilityScores.First();
        var newBase = 18;

        // Act
        var postResponse = await client.PostAsJsonAsync($"/api/characters/{_characterId}/abilities/{target.AbilityScoreId}/base", new { value = newBase }, CancellationToken.None);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await GetAbilitiesAsync(client, _characterId);
        var updated = abilitiesAfter!.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.BaseScore.Should().Be(newBase);
        updated.CalculatedScore.Should().Be(updated.BaseScore + updated.AdditionalScore);
        updated.CalculatedModifier.Should().Be((int)Math.Floor((updated.CalculatedScore - 10) / 2.0));
    }

    [TestMethod]
    public async Task UpdateAdditionalScore_Updates_CalculatedFields()
    {
        // Arrange
        var client = _integration.CreateClient();
        var abilitiesBefore = await GetAbilitiesAsync(client, _characterId);
        var target = abilitiesBefore!.AbilityScores.First();
        var newAdditional = 2;

        // Act
        var postResponse = await client.PostAsJsonAsync($"/api/characters/{_characterId}/abilities/{target.AbilityScoreId}/additional", new { value = newAdditional }, CancellationToken.None);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await GetAbilitiesAsync(client, _characterId);
        var updated = abilitiesAfter!.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.AdditionalScore.Should().Be(newAdditional);
        updated.CalculatedScore.Should().Be(updated.BaseScore + updated.AdditionalScore);
        updated.CalculatedModifier.Should().Be((int)Math.Floor((updated.CalculatedScore - 10) / 2.0));
    }

    private static async Task<GetAbilityScoresResponse?> GetAbilitiesAsync(HttpClient client, Guid characterId)
    {
        var response = await client.GetAsync($"/api/characters/{characterId}/abilities", CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<GetAbilityScoresResponse>(CancellationToken.None);
    }

    private static async Task WaitForAbilitiesAsync(HttpClient client, Guid characterId, int minCount, int timeoutMs)
    {
        var start = DateTimeOffset.UtcNow;
        while (true)
        {
            var data = await GetAbilitiesAsync(client, characterId);
            if (data?.AbilityScores.Count >= minCount)
            {
                return;
            }

            if ((DateTimeOffset.UtcNow - start).TotalMilliseconds > timeoutMs)
            {
                return;
            }

            await Task.Delay(100);
        }
    }
}
