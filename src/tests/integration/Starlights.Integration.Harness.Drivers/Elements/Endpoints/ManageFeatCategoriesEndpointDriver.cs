using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetList;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageFeatCategoriesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageFeatCategoriesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Get all feat categories via the API <code>/api/elements/feat-categories</code>
    /// </summary>
    public async Task<List<FeatCategoryDataModel>> GetAsync()
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync("/api/elements/feat-categories", _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GetFeatCategoriesResponse>(_integration.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull("expected Items to be not null");

        return [.. payload.Items];
    }

    /// <summary>
    /// Create a new feat category via the API <code>/api/elements/feat-categories/create</code>
    /// </summary>
    public async Task<Guid> CreateAsync(CreateFeatCategoryRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync("/api/elements/feat-categories/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateFeatCategoryResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!.Id;
    }

    /// <summary>
    /// Get a feat category by ID via the API <code>/api/elements/feat-categories/{id}</code>
    /// </summary>
    public async Task<FeatCategoryDataModel?> GetAsync(Guid id)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/feat-categories/{id}", _integration.CancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadFromJsonAsync<FeatCategoryDataModel>(_integration.CancellationToken);
            responseContent.Should().NotBeNull();
            return responseContent;
        }

        return null;
    }

    /// <summary>
    /// Update an existing feat category via the API <code>/api/elements/feat-categories/{id}</code>
    /// </summary>
    public async Task<bool> PutAsync(UpdateFeatCategoryRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/feat-categories/{request.Id}", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateFeatCategoryResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return true;
    }
}
