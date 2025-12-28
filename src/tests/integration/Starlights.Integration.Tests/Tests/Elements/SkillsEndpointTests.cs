using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.Create;
using Starlights.Modules.Elements.Endpoints.Entities.Skills.GetSkills;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class SkillsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task CreateSkill_WhenAbilityDoesNotExist_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new
        {
            name = "Acrobatics",
            abilityId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync("/api/elements/skills", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUpdateDeleteSkill_HappyPath()
    {
        var client = _integration.CreateClient();

        var createAbilityScoreResponse = await client.PostAsJsonAsync(
            "/api/elements/ability-scores/create",
            new { name = "Dexterity", abbreviation = "DEX", description = "" },
            _integration.CancellationToken);

        createAbilityScoreResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var abilityScoreCreated = await createAbilityScoreResponse.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        abilityScoreCreated.Should().NotBeNull();
        var abilityId = abilityScoreCreated!.Id;
        abilityId.Should().NotBeEmpty();

        var createResponse = await client.PostAsJsonAsync(
            "/api/elements/skills",
            new { name = "Acrobatics", abilityId },
            _integration.CancellationToken);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateSkillResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();

        var listResponse = await client.GetAsync("/api/elements/skills", _integration.CancellationToken);
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<GetSkillsResponse>(_integration.CancellationToken);
        list.Should().NotBeNull();
        list!.Skills.Should().ContainSingle(x => x.Id == created.Id && x.Name == "Acrobatics" && x.AbilityId == abilityId && x.Ability == "Dexterity" && x.Description == string.Empty);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/elements/skills/{created.Id}",
            new { id = created.Id, name = "Acrobatics", abilityId, description = "" },
            _integration.CancellationToken);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResponse = await client.DeleteAsync($"/api/elements/skills/{created.Id}", _integration.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteAgainResponse = await client.DeleteAsync($"/api/elements/skills/{created.Id}", _integration.CancellationToken);
        deleteAgainResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
