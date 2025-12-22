using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageFeatCategoriesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageFeatCategoriesEndpointDriver _api;

    public ManageFeatCategoriesDriver(IIntegrationHost integration, ManageFeatCategoriesEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateFeatCategory(string name, string? description)
    {
        _integration.WriteLine($"creating feat category {name}");

        var request = new CreateFeatCategoryRequest
        {
            Name = name,
            Description = description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _integration.Properties["last-created-feat-category-id"] = id;

        return id;
    }

    public async Task<FeatCategoryDataModel?> GetFeatCategory(Guid id)
    {
        _integration.WriteLine($"retrieving feat category {id}");

        var featCategory = await _api.GetAsync(id);

        if (featCategory is not null)
        {
            _integration.Properties["last-retrieved-feat-category"] = featCategory;
        }

        return featCategory;
    }

    public async Task<FeatCategoryDataModel> GetLastCreatedFeatCategory()
    {
        var id = (Guid)_integration.Properties["last-created-feat-category-id"]!;
        var featCategory = await GetFeatCategory(id);
        featCategory.Should().NotBeNull();
        return featCategory!;
    }

    public Task<bool> UpdateFeatCategory(FeatCategoryDataModel updatedModel)
    {
        _integration.WriteLine($"updating feat category {updatedModel.Name} ({updatedModel.Id})");

        var request = new UpdateFeatCategoryRequest
        {
            Id = updatedModel.Id,
            Name = updatedModel.Name,
            Description = updatedModel.Description
        };

        return _api.PutAsync(request);
    }

    public async Task<List<FeatCategoryDataModel>> GetFeatCategories()
    {
        var featCategories = await _api.GetAsync();

        _integration.Properties["last-retrieved-feat-categories"] = featCategories;

        _integration.WriteLine($"retrieved {featCategories.Count} feat categories.");

        foreach (var featCategory in featCategories)
        {
            _integration.WriteLine($"- {featCategory.Name}");
        }

        return featCategories;
    }
}
