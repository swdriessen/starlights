using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Update;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class AbilityScoresEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _integration.Dispose();
    }

    [TestMethod]
    public async Task CreateAbilityScore_WhenValid_ReturnsCreated()
    {
        var client = _integration.CreateClient();

        var request = new CreateAbilityScoreRequest("Strength", "STR", null);

        var response = await client.PostAsJsonAsync("/api/elements/ability-scores/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreateAbilityScore_WhenInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateAbilityScoreRequest("", "", null);

        var response = await client.PostAsJsonAsync("/api/elements/ability-scores/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task GetAbilityScores_WhenCreated_ReturnsListContainingIt()
    {
        var client = _integration.CreateClient();

        var createRequest = new CreateAbilityScoreRequest("Strength", "STR", "");
        var createResponse = await client.PostAsJsonAsync("/api/elements/ability-scores/create", createRequest, _integration.CancellationToken);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();

        var listResponse = await client.GetAsync("/api/elements/ability-scores", _integration.CancellationToken);
        listResponse.EnsureSuccessStatusCode();

        var listPayload = await listResponse.Content.ReadFromJsonAsync<GetAbilityScoresResponse>(_integration.CancellationToken);
        listPayload.Should().NotBeNull();

        listPayload!.Items.Should().ContainSingle(x => x.Id == created!.Id);
    }

    [TestMethod]
    public async Task UpdateAbilityScore_WhenNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var request = new UpdateAbilityScoreRequest(Guid.NewGuid(), "Intelligence", "INT", "Measures mental acuity...");

        var response = await client.PutAsJsonAsync($"/api/elements/ability-scores/{request.Id}", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task UpdateAbilityScore_WhenValid_UpdatesFields()
    {
        var client = _integration.CreateClient();

        var createRequest = new CreateAbilityScoreRequest("Intelligence", "INT", "");
        var createResponse = await client.PostAsJsonAsync("/api/elements/ability-scores/create", createRequest, _integration.CancellationToken);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<CreateAbilityScoreResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();

        var updateRequest = new UpdateAbilityScoreRequest(created!.Id, "Intelligence", "INT", "Measures mental acuity...");
        var updateResponse = await client.PutAsJsonAsync($"/api/elements/ability-scores/{updateRequest.Id}", updateRequest, _integration.CancellationToken);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/elements/ability-scores/{created.Id}", _integration.CancellationToken);
        getResponse.EnsureSuccessStatusCode();

        var ability = await getResponse.Content.ReadFromJsonAsync<AbilityScoreDataModel>(_integration.CancellationToken);
        ability.Should().NotBeNull();
        ability!.Description.Should().Be("Measures mental acuity...");
    }

    [TestMethod]
    public async Task DeleteAbilityScore_WhenNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var id = Guid.NewGuid();

        var response = await client.DeleteAsync($"/api/elements/ability-scores/{id}", _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
