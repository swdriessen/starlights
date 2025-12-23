using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Create;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageElementsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageElementsEndpointDriver _api;

    public ManageElementsDriver(IIntegrationHost integration, ManageElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateElement(CreateProperties properties)
    {
        var request = new CreateElementRequest
        {
            Name = properties.Name,
            Type = properties.Type,
            Description = properties.Description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _integration.Set(id, "last-created-element-id");
        _integration.Set(properties, "last-created-element-properties");

        return id;
    }

    public Task<ElementDataModel> GetElementById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public Task<ElementDataModel> GetLastCreatedElement()
    {
        var id = _integration.Get<Guid>("last-created-element-id");
        return GetElementById(id);
    }

    public async Task<List<ElementDataModel>> GetElements(string? type = null)
    {
        var response = await _api.GetListAsync(type);
        return [.. response.Items];
    }

    public sealed class CreateProperties
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }
}
