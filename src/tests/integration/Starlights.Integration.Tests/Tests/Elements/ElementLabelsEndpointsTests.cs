using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Create;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Create;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Delete;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.List;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Update;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ElementLabelsEndpointsTests : IntegrationTestBase
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
    public async Task ListLabels_WhenElementNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{Guid.NewGuid()}/labels", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task CreateLabel_WhenInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateElementLabelRequest([""]);

        var response = await client.PostAsJsonAsync($"/api/elements/{Guid.Empty}/labels", request, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateLabel_WhenValid_IsReturnedByList()
    {
        var client = _integration.CreateClient();

        var createElementRequest = new CreateElementRequest { Name = "Test Element", Type = "TestType", Description = null };
        var createElementResponse = await client.PostAsJsonAsync("/api/elements/create", createElementRequest, CancellationToken.None);
        createElementResponse.EnsureSuccessStatusCode();

        var createdElement = await createElementResponse.Content.ReadFromJsonAsync<CreateElementResponse>(CancellationToken.None);
        createdElement.Should().NotBeNull();

        var createLabelRequest = new CreateElementLabelRequest(["MyLabel"]);
        var createLabelResponse = await client.PostAsJsonAsync($"/api/elements/{createdElement.Id}/labels", createLabelRequest, CancellationToken.None);
        createLabelResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResponse = await client.GetAsync($"/api/elements/{createdElement.Id}/labels", CancellationToken.None);
        listResponse.EnsureSuccessStatusCode();

        var listPayload = await listResponse.Content.ReadFromJsonAsync<ListElementLabelsResponse>(CancellationToken.None);
        listPayload.Should().NotBeNull();
        listPayload!.Labels.Should().Contain("MyLabel");
    }

    [TestMethod]
    public async Task UpdateLabels_WhenInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var createElementRequest = new CreateElementRequest { Name = "Test Element", Type = "TestType", Description = null };
        var createElementResponse = await client.PostAsJsonAsync("/api/elements/create", createElementRequest, CancellationToken.None);
        createElementResponse.EnsureSuccessStatusCode();

        var createdElement = await createElementResponse.Content.ReadFromJsonAsync<CreateElementResponse>(CancellationToken.None);
        createdElement.Should().NotBeNull();

        var updateRequest = new UpdateElementLabelRequest([""]);

        var response = await client.PutAsJsonAsync($"/api/elements/{createdElement.Id}/labels", updateRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task UpdateLabels_WhenValid_ReplacesEntireList()
    {
        var client = _integration.CreateClient();

        var createElementRequest = new CreateElementRequest { Name = "Test Element", Type = "TestType", Description = null };
        var createElementResponse = await client.PostAsJsonAsync("/api/elements/create", createElementRequest, CancellationToken.None);
        createElementResponse.EnsureSuccessStatusCode();

        var createdElement = await createElementResponse.Content.ReadFromJsonAsync<CreateElementResponse>(CancellationToken.None);
        createdElement.Should().NotBeNull();

        var createLabelRequest = new CreateElementLabelRequest(["A", "B"]);
        var createLabelResponse = await client.PostAsJsonAsync($"/api/elements/{createdElement.Id}/labels", createLabelRequest, CancellationToken.None);
        createLabelResponse.EnsureSuccessStatusCode();

        var updateRequest = new UpdateElementLabelRequest(["C"]);
        var updateResponse = await client.PutAsJsonAsync($"/api/elements/{createdElement.Id}/labels", updateRequest, CancellationToken.None);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await client.GetAsync($"/api/elements/{createdElement.Id}/labels", CancellationToken.None);
        listResponse.EnsureSuccessStatusCode();

        var listPayload = await listResponse.Content.ReadFromJsonAsync<ListElementLabelsResponse>(CancellationToken.None);
        listPayload.Should().NotBeNull();
        listPayload!.Labels.Should().BeEquivalentTo(["C"]);
    }

    [TestMethod]
    public async Task DeleteLabel_WhenValid_RemovesItFromList()
    {
        var client = _integration.CreateClient();

        var createElementRequest = new CreateElementRequest { Name = "Test Element", Type = "TestType", Description = null };
        var createElementResponse = await client.PostAsJsonAsync("/api/elements/create", createElementRequest, CancellationToken.None);
        createElementResponse.EnsureSuccessStatusCode();

        var createdElement = await createElementResponse.Content.ReadFromJsonAsync<CreateElementResponse>(CancellationToken.None);
        createdElement.Should().NotBeNull();

        var createLabelRequest = new CreateElementLabelRequest(["ToDelete"]);
        var createLabelResponse = await client.PostAsJsonAsync($"/api/elements/{createdElement.Id}/labels", createLabelRequest, CancellationToken.None);
        createLabelResponse.EnsureSuccessStatusCode();

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/elements/{createdElement.Id}/labels")
        {
            Content = JsonContent.Create(new DeleteElementLabelRequest(["ToDelete"]))
        };

        var deleteResponse = await client.SendAsync(deleteRequest, CancellationToken.None);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listResponse = await client.GetAsync($"/api/elements/{createdElement.Id}/labels", CancellationToken.None);
        listResponse.EnsureSuccessStatusCode();

        var listPayload = await listResponse.Content.ReadFromJsonAsync<ListElementLabelsResponse>(CancellationToken.None);
        listPayload.Should().NotBeNull();
        listPayload!.Labels.Should().NotContain("ToDelete");
    }
}
