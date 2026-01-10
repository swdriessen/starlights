using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.GetList;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ClassesEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task GetClasses_WhenClassCreated_ReturnsListContainingIt()
    {
        var client = _integration.CreateClient();

        var createRequest = new CreateClassRequest
        {
            Name = "Wizard",
            HitPointDieSize = 6,
            HitPointDieAmount = 1,
            Description = "A scholarly magic user.",
            ShortDescription = null
        };

        var createResponse = await client.PostAsJsonAsync("/api/elements/classes/create", createRequest, _integration.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();

        var listResponse = await client.GetAsync("/api/elements/classes", _integration.CancellationToken);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await listResponse.Content.ReadFromJsonAsync<GetClassesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().ContainSingle(x => x.Id == created!.Id);
    }

    [TestMethod]
    public async Task GetClassById_WhenNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/classes/{Guid.NewGuid()}", _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetClassById_WhenCreated_ReturnsDetails()
    {
        var client = _integration.CreateClient();

        var createRequest = new CreateClassRequest
        {
            Name = "Barbarian",
            HitPointDieSize = 12,
            HitPointDieAmount = 1,
            Description = "A fierce warrior.",
            ShortDescription = "Raging frontliner"
        };

        var createResponse = await client.PostAsJsonAsync("/api/elements/classes/create", createRequest, _integration.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        created.Should().NotBeNull();

        var getResponse = await client.GetAsync($"/api/elements/classes/{created!.Id}", _integration.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await getResponse.Content.ReadFromJsonAsync<ClassDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(created.Id);
        payload.Name.Should().Be("Barbarian");
        payload.HitPointDie.Should().Be("d12");
        payload.ShortDescription.Should().Be("Raging frontliner");
        payload.Description.Should().Be("A fierce warrior.");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _integration.Dispose();
    }

    [TestMethod]
    public async Task CreateClass_WhenValidWithoutShortDescription_ReturnsCreated()
    {
        var client = _integration.CreateClient();

        var request = new CreateClassRequest
        {
            Name = "Wizard",
            HitPointDieSize = 6,
            HitPointDieAmount = 1,
            Description = "A scholarly magic user.",
            ShortDescription = null
        };

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreateClass_WhenValidWithShortDescription_ReturnsCreated()
    {
        var client = _integration.CreateClient();

        var request = new CreateClassRequest
        {
            Name = "Fighter",
            HitPointDieSize = 10,
            HitPointDieAmount = 1,
            Description = "A master of martial combat.",
            ShortDescription = "Martial powerhouse"
        };

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreateClass_WhenMissingName_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateClassRequest
        {
            Name = "",
            HitPointDieSize = 6,
            HitPointDieAmount = 1,
            Description = "A scholarly magic user.",
            ShortDescription = null
        };

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateClass_WhenMissingDescription_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateClassRequest
        {
            Name = "Wizard",
            HitPointDieSize = 6,
            HitPointDieAmount = 1,
            Description = "",
            ShortDescription = null
        };

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [DataRow(0, 1)]
    [DataRow(-1, 1)]
    [DataRow(6, 0)]
    [DataRow(6, -1)]
    [TestMethod]
    public async Task CreateClass_WhenHitPointDieInvalid_ReturnsBadRequest(int hitPointDieSize, int hitPointDieAmount)
    {
        var client = _integration.CreateClient();

        var request = new CreateClassRequest
        {
            Name = "Wizard",
            HitPointDieSize = hitPointDieSize,
            HitPointDieAmount = hitPointDieAmount,
            Description = "A scholarly magic user.",
            ShortDescription = null
        };

        var response = await client.PostAsJsonAsync("/api/elements/classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
