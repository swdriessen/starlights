using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.GetList;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class SubClassesEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task GetSubClasses_WhenCreated_ReturnsListContainingIt()
    {
        var client = _integration.CreateClient();

        var createClassRequest = new CreateClassRequest
        {
            Name = "Barbarian",
            HitPointDieSize = 12,
            HitPointDieAmount = 1,
            Description = "A fierce warrior.",
            ShortDescription = null
        };

        var createClassResponse = await client.PostAsJsonAsync("/api/elements/classes/create", createClassRequest, _integration.CancellationToken);
        createClassResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdClass = await createClassResponse.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        createdClass.Should().NotBeNull();

        var createSubClassRequest = new CreateSubClassRequest
        {
            Name = "Path of the Totem Warrior",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Description = "A primal path."
        };

        var createSubClassResponse = await client.PostAsJsonAsync("/api/elements/sub-classes/create", createSubClassRequest, _integration.CancellationToken);
        createSubClassResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdSubClass = await createSubClassResponse.Content.ReadFromJsonAsync<CreateSubClassResponse>(_integration.CancellationToken);
        createdSubClass.Should().NotBeNull();

        var listResponse = await client.GetAsync("/api/elements/sub-classes", _integration.CancellationToken);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await listResponse.Content.ReadFromJsonAsync<GetSubClassesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().ContainSingle(x => x.Id == createdSubClass!.Id);
    }

    [TestMethod]
    public async Task GetSubClassById_WhenNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/sub-classes/{Guid.NewGuid()}", _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetSubClassById_WhenCreated_ReturnsDetails()
    {
        var client = _integration.CreateClient();

        var createClassRequest = new CreateClassRequest
        {
            Name = "Barbarian",
            HitPointDieSize = 12,
            HitPointDieAmount = 1,
            Description = "A fierce warrior.",
            ShortDescription = null
        };

        var createClassResponse = await client.PostAsJsonAsync("/api/elements/classes/create", createClassRequest, _integration.CancellationToken);
        createClassResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdClass = await createClassResponse.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        createdClass.Should().NotBeNull();

        var createSubClassRequest = new CreateSubClassRequest
        {
            Name = "Path of the Totem Warrior",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Description = "A primal path."
        };

        var createSubClassResponse = await client.PostAsJsonAsync("/api/elements/sub-classes/create", createSubClassRequest, _integration.CancellationToken);
        createSubClassResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdSubClass = await createSubClassResponse.Content.ReadFromJsonAsync<CreateSubClassResponse>(_integration.CancellationToken);
        createdSubClass.Should().NotBeNull();

        var getResponse = await client.GetAsync($"/api/elements/sub-classes/{createdSubClass!.Id}", _integration.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await getResponse.Content.ReadFromJsonAsync<SubClassDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(createdSubClass.Id);
        payload.Name.Should().Be("Path of the Totem Warrior");
        payload.ParentId.Should().Be(createdClass.Id);
        payload.ParentName.Should().Be("Barbarian");
        payload.Description.Should().Be("A primal path.");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _integration.Dispose();
    }

    [TestMethod]
    public async Task CreateSubClass_WhenValid_ReturnsCreated()
    {
        var client = _integration.CreateClient();

        var createClassRequest = new CreateClassRequest
        {
            Name = "Barbarian",
            HitPointDieSize = 12,
            HitPointDieAmount = 1,
            Description = "A fierce warrior.",
            ShortDescription = null
        };

        var createClassResponse = await client.PostAsJsonAsync("/api/elements/classes/create", createClassRequest, _integration.CancellationToken);
        createClassResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdClass = await createClassResponse.Content.ReadFromJsonAsync<CreateClassResponse>(_integration.CancellationToken);
        createdClass.Should().NotBeNull();

        var request = new CreateSubClassRequest
        {
            Name = "Path of the Totem Warrior",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Description = "A primal path."
        };

        var response = await client.PostAsJsonAsync("/api/elements/sub-classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateSubClassResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreateSubClass_WhenInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateSubClassRequest
        {
            Name = "",
            ParentClassId = Guid.Empty,
            ParentClassName = "",
            Description = ""
        };

        var response = await client.PostAsJsonAsync("/api/elements/sub-classes/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
