using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.GetProficiencies;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Proficiencies.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageProficienciesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly EventObserverCollection _events;
    private readonly ManageProficienciesEndpointDriver _endpoints;

    public ManageProficienciesDriver(IIntegrationHost integration, ElementsDriverContext driverContext)
    {
        _integration = integration;
        _driverContext = driverContext;
        _endpoints = _integration.GetDriver<ManageProficienciesEndpointDriver>();
        _events = _integration.Events;
    }

    public async Task<Guid> CreateProficiencyAsync(CreateProperties properties)
    {
        var request = new CreateProficiencyRequest(properties.Name, properties.ProficiencyType, Description: null);

        var id = await _endpoints.CreateAsync(request);

        _driverContext.WithCreatedElement(id, request.Name);

        await _events.EnsureObservation<ElementCreatedEvent>(e => e.ElementId == id && e.Type == ElementTypeConstants.Proficiency);

        return id;
    }

    public async Task<IReadOnlyList<ProficiencyListItem>> GetProficienciesAsync()
    {
        var payload = await _endpoints.GetListAsync();
        payload.Items.Should().NotBeNull();

        return [.. payload.Items];
    }

    public async Task<ProficiencyListItem> GetProficiencyByIdAsync(Guid id)
    {
        var items = await GetProficienciesAsync();
        return items.Single(x => x.Id == id);
    }

    public async Task<ProficiencyListItem> GetLastCreatedProficiency()
    {
        var items = await GetProficienciesAsync();
        return items.Single(x => x.Id == _driverContext.CurrentElement.Id);
    }

    public async Task UpdateProficiencyAsync(Guid id, UpdateProperties properties)
    {
        var current = await GetProficiencyByIdAsync(id);

        var request = new UpdateProficiencyRequest(
            Id: id,
            Name: properties.Name ?? current.Name,
            ProficiencyType: properties.ProficiencyType ?? current.ProficiencyType,
            Description: properties.Description ?? current.Description);

        await _endpoints.UpdateAsync(request);
    }

    public async Task DeleteProficiencyAsync(Guid id)
    {
        await _endpoints.DeleteAsync(id);
    }

    public sealed record CreateProperties
    {
        public required string Name { get; init; }
        public required string ProficiencyType { get; init; }
    }

    public sealed record UpdateProperties
    {
        public string? Name { get; init; }
        public string? ProficiencyType { get; init; }
        public string? Description { get; init; }

        public static UpdateProperties FromDictionary(IReadOnlyDictionary<string, string?> values)
        {
            values.TryGetValue("name", out var name);
            values.TryGetValue("proficiency type", out var proficiencyType);
            values.TryGetValue("description", out var description);

            return new UpdateProperties
            {
                Name = name,
                ProficiencyType = proficiencyType,
                Description = description
            };
        }
    }
}
