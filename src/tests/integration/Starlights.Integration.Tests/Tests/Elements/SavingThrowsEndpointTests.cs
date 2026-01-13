using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class SavingThrowsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task CreateSavingThrow_WhenAbilityDoesNotExist_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new
        {
            name = "Strength",
            abilityId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync("/api/elements/saving-throws", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUpdateDeleteSavingThrow_HappyPath()
    {
        var client = _integration.CreateClient();

        var createAbilityScoreResponse = await client.PostAsJsonAsync(
            "/api/elements/ability-scores/create",
            new { name = "Strength", abbreviation = "STR", description = "" },
            _integration.CancellationToken);

        createAbilityScoreResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var abilityScoreCreated = await createAbilityScoreResponse.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        abilityScoreCreated.Should().NotBeNull();
        var abilityId = abilityScoreCreated!.Id;
        abilityId.Should().NotBeEmpty();

        var createResponse = await client.PostAsJsonAsync(
            "/api/elements/saving-throws",
            new { name = "Strength", abilityId },
            _integration.CancellationToken);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateSavingThrowResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();

        var listResponse = await client.GetAsync("/api/elements/saving-throws", _integration.CancellationToken);
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<GetSavingThrowsResponse>(_integration.CancellationToken);
        list.Should().NotBeNull();
        list!.SavingThrows.Should().ContainSingle(x => x.Id == created.Id && x.Name == "Strength" && x.AbilityId == abilityId && x.Ability == "Strength" && x.Description == string.Empty);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/elements/saving-throws/{created.Id}",
            new { id = created.Id, name = "Strength", abilityId, description = "" },
            _integration.CancellationToken);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResponse = await client.DeleteAsync($"/api/elements/saving-throws/{created.Id}", _integration.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteAgainResponse = await client.DeleteAsync($"/api/elements/saving-throws/{created.Id}", _integration.CancellationToken);
        deleteAgainResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}