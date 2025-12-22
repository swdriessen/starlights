using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetList;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class FeatCategoriesEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestMethod]
    public async Task CreateFeatCategory()
    {
        // Arrange
        var client = _integration.CreateClient();
        var request = new CreateFeatCategoryRequest
        {
            Name = "General",
            Description = "General feats"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/elements/feat-categories/create", request, _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<CreateFeatCategoryResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().NotBe(Guid.Empty);
    }

    [TestMethod]
    public async Task GetFeatCategories()
    {
        // Arrange
        var id = await CreateFeatCategoryAsync("General", "General feats");
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/feat-categories", _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<GetFeatCategoriesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().Contain(x => x.Id == id);
    }

    [TestMethod]
    public async Task GetFeatCategoryById()
    {
        // Arrange
        var id = await CreateFeatCategoryAsync("General", "General feats");
        var client = _integration.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/elements/feat-categories/{id}", _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<FeatCategoryDataModel>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(id);
    }

    [TestMethod]
    public async Task UpdateFeatCategory()
    {
        // Arrange
        var id = await CreateFeatCategoryAsync("General", "General feats");
        var client = _integration.CreateClient();
        var request = new UpdateFeatCategoryRequest
        {
            Id = id,
            Name = "Combat",
            Description = "Combat feats"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/elements/feat-categories/{id}", request, _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<UpdateFeatCategoryResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(id);
    }

    private async Task<Guid> CreateFeatCategoryAsync(string name, string? description)
    {
        using var client = _integration.CreateClient();

        var request = new CreateFeatCategoryRequest
        {
            Name = name,
            Description = description
        };

        var response = await client.PostAsJsonAsync("/api/elements/feat-categories/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CreateFeatCategoryResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();

        return payload!.Id;
    }
}
