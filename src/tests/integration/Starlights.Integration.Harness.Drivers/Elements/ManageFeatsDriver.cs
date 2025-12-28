using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Feats;
using Starlights.Modules.Elements.Endpoints.Content.Feats.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ManageFeatsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageFeatsEndpointDriver _api;

    private readonly ElementsScenarioContext _elementsContext;

    public ManageFeatsDriver(IIntegrationHost integration, ManageFeatsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _elementsContext = integration.Get<ElementsScenarioContext>();
        _api = endpointDriver;
    }

    public async Task<Guid> CreateFeat(CreateProperties properties)
    {
        var request = new Modules.Elements.Endpoints.Content.Feats.Create.CreateFeatRequest
        {
            Name = properties.Name,
            CategoryId = properties.CategoryId,
            ShortDescription = properties.ShortDescription,
            Description = properties.Description,
            IsRepeatable = properties.IsRepeatable,
            Prerequisite = properties.Prerequisite,
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(request.Name, id);

        _integration.Set(id, "last-created-feat-id");
        _integration.Set(properties, "last-created-feat-properties");

        return id;
    }

    public Task<FeatDataModel> GetFeatById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public Task<FeatDataModel> GetLastCreatedFeat()
    {
        var id = _integration.Get<Guid>("last-created-feat-id");
        return GetFeatById(id);
    }

    public async Task<List<FeatDataModel>> GetFeats()
    {
        var response = await _api.GetListAsync();
        return [.. response.Items];
    }

    public async Task UpdateFeat(FeatDataModel updatedFeat)
    {
        var request = new UpdateFeatRequest
        {
            Id = updatedFeat.Id,
            Name = updatedFeat.Name,
            CategoryId = updatedFeat.CategoryId,
            ShortDescription = updatedFeat.ShortDescription,
            Description = updatedFeat.Description,
            Prerequisite = updatedFeat.Prerequisites,
            IsRepeatable = updatedFeat.IsRepeatable,
        };

        await _api.PutAsync(request);
    }

    public class CreateProperties
    {
        public required string Name { get; set; }
        public required Guid CategoryId { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Prerequisite { get; set; }
        public bool IsRepeatable { get; set; }
    }
}
