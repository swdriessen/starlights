using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.Create;
using Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.GetProficiencies;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ProficienciesEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task CreateProficiency_WhenRequestInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/elements/proficiencies",
            new { name = "", proficiencyType = "" },
            _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUpdateDeleteProficiency_HappyPath()
    {
        var client = _integration.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/elements/proficiencies",
            new { name = "Animal Handling", proficiencyType = "Skill" },
            _integration.CancellationToken);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProficiencyResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();

        var listResponse = await client.GetAsync("/api/elements/proficiencies", _integration.CancellationToken);
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<GetProficienciesResponse>(_integration.CancellationToken);
        list.Should().NotBeNull();
        list!.Items.Should().ContainSingle(x => x.Id == created.Id && x.Name == "Animal Handling" && x.ProficiencyType == "Skill" && x.Description == string.Empty);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/elements/proficiencies/{created.Id}",
            new { id = created.Id, name = "Animal Handling", proficiencyType = "Skill", description = "" },
            _integration.CancellationToken);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResponse = await client.DeleteAsync($"/api/elements/proficiencies/{created.Id}", _integration.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteAgainResponse = await client.DeleteAsync($"/api/elements/proficiencies/{created.Id}", _integration.CancellationToken);
        deleteAgainResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
