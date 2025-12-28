using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;
using Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageFeatCategoriesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageFeatCategoriesEndpointDriver _api;
    private readonly ElementsScenarioContext _elementsContext;

    public ManageFeatCategoriesDriver(IIntegrationHost integration, ManageFeatCategoriesEndpointDriver endpointDriver)
    {
        _integration = integration;
        _elementsContext = integration.Get<ElementsScenarioContext>();
        _api = endpointDriver;
    }

    public async Task<Guid> CreateFeatCategory(string name, string? description = null)
    {
        var request = new CreateFeatCategoryRequest
        {
            Name = name,
            Description = description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(request.Name, id);

        _integration.Set(id, "last-created-feat-category-id");

        return id;
    }

    public async Task<FeatCategoryDataModel?> GetFeatCategory(Guid id)
    {
        return await _api.GetAsync(id);
    }

    public Task<bool> UpdateFeatCategory(FeatCategoryDataModel updatedModel)
    {
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
        return await _api.GetAsync();
    }

    public async Task<FeatCategoryDataModel> GetLastCreatedFeatCategory()
    {
        var id = _integration.Get<Guid>("last-created-feat-category-id");
        var featCategory = await GetFeatCategory(id);
        featCategory.Should().NotBeNull();
        return featCategory;
    }

    public async Task<FeatCategoryDataModel> GetFeatCategoryByName(string name)
    {
        var categories = await _api.GetAsync();
        var category = categories.SingleOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        category.Should().NotBeNull($"expected feat category '{name}' to exist");
        return category;
    }

    public async Task<FeatCategoryDataModel> GetorCreateFeatCategoryByName(string name)
    {
        var categories = await _api.GetAsync();
        var category = categories.SingleOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

        if (category is null)
        {
            await CreateFeatCategory(name);
        }

        return await GetFeatCategoryByName(name);
    }

    public async Task CreateFeatCategoriesWhenNotExisting(IEnumerable<string> names)
    {
        var categories = await _api.GetAsync();

        var nonExistingNames = names.Except(categories.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

        foreach (var name in nonExistingNames)
        {
            await CreateFeatCategory(name);
        }
    }

    public async Task<bool> ExistsFeatCategory(string name)
    {
        var categories = await _api.GetAsync();
        return categories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
