using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.GetList;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ClassFeaturesEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task GetClassFeatures_WhenFeatureCreated_ReturnsListContainingIt()
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

        var createFeatureRequest = new CreateClassFeatureRequest
        {
            Name = "Danger Sense",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Level = 2,
            Description = "You gain an uncanny sense of danger."
        };

        var createFeatureResponse = await client.PostAsJsonAsync("/api/elements/class-features/create", createFeatureRequest, _integration.CancellationToken);
        createFeatureResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdFeature = await createFeatureResponse.Content.ReadFromJsonAsync<CreateClassFeatureResponse>(_integration.CancellationToken);
        createdFeature.Should().NotBeNull();

        var listResponse = await client.GetAsync("/api/elements/class-features", _integration.CancellationToken);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await listResponse.Content.ReadFromJsonAsync<GetClassFeaturesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().ContainSingle(x => x.Id == createdFeature!.Id);
    }

    [TestMethod]
    public async Task GetClassFeatureById_WhenNotFound_ReturnsNotFound()
    {
        var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/class-features/{Guid.NewGuid()}", _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetClassFeatureById_WhenCreated_ReturnsDetails()
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

        var createFeatureRequest = new CreateClassFeatureRequest
        {
            Name = "Danger Sense",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Level = 2,
            Description = "You gain an uncanny sense of danger."
        };

        var createFeatureResponse = await client.PostAsJsonAsync("/api/elements/class-features/create", createFeatureRequest, _integration.CancellationToken);
        createFeatureResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdFeature = await createFeatureResponse.Content.ReadFromJsonAsync<CreateClassFeatureResponse>(_integration.CancellationToken);
        createdFeature.Should().NotBeNull();

        var getResponse = await client.GetAsync($"/api/elements/class-features/{createdFeature!.Id}", _integration.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await getResponse.Content.ReadFromJsonAsync<ClassFeatureDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(createdFeature.Id);
        payload.Name.Should().Be("Danger Sense");
        payload.Level.Should().Be(2);
        payload.ParentId.Should().Be(createdClass.Id);
        payload.ParentName.Should().Be("Barbarian");
        payload.Description.Should().Be("You gain an uncanny sense of danger.");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _integration.Dispose();
    }

    [TestMethod]
    public async Task CreateClassFeature_WhenValid_ReturnsCreated()
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

        var request = new CreateClassFeatureRequest
        {
            Name = "Danger Sense",
            ParentClassId = createdClass!.Id,
            ParentClassName = "Barbarian",
            Level = 2,
            Description = "You gain an uncanny sense of when things nearby aren't as they should be."
        };

        var response = await client.PostAsJsonAsync("/api/elements/class-features/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<CreateClassFeatureResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreateClassFeature_WhenInvalid_ReturnsBadRequest()
    {
        var client = _integration.CreateClient();

        var request = new CreateClassFeatureRequest
        {
            Name = "",
            ParentClassId = Guid.Empty,
            ParentClassName = "",
            Level = -1,
            Description = ""
        };

        var response = await client.PostAsJsonAsync("/api/elements/class-features/create", request, _integration.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
