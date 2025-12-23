using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Feats;

namespace Starlights.Integration.Drivers.Elements;

public class ManageFeatsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageFeatsEndpointDriver _api;

    public ManageFeatsDriver(IIntegrationHost integration, ManageFeatsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateFeat(CreateProperties properties)
    {
        _integration.WriteIndentedLine($"creating feat {properties.Name}");

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

        _integration.Properties["last-created-feat-id"] = id;
        _integration.Properties["last-created-feat-properties"] = properties;

        return id;
    }

    public Task<FeatDataModel> GetFeatById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public async Task<List<FeatDataModel>> GetFeats()
    {
        var r = await _api.GetListAsync();

        return r.Items.ToList();
    }

    public Task<FeatDataModel> GetLastCreatedFeat()
    {
        var id = (Guid)_integration.Properties["last-created-feat-id"];
        return GetFeatById(id);
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
