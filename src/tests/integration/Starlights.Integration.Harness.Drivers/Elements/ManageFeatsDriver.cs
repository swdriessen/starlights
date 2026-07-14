using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Create;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ManageFeatsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly EventObserverCollection _events;
    private readonly ManageFeatsEndpointDriver _api;

    public ManageFeatsDriver(IIntegrationHost integration, ElementsDriverContext driverContext, ManageFeatsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _driverContext = driverContext;
        _api = endpointDriver;
        _events = _integration.Events;
    }

    public async Task<Guid> CreateFeat(CreateProperties properties)
    {
        var request = new CreateFeatRequest
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

        _driverContext.WithCreatedElement(id, request.Name);

        await _events.EnsureObservation<ElementCreatedEvent>(e => e.ElementId == id && e.Type == ElementTypeConstants.Feat);

        return id;
    }

    public Task<FeatDataModel> GetFeatById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public Task<FeatDataModel> GetLastCreatedFeat()
    {
        return GetFeatById(_driverContext.CurrentElement.Id);
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
