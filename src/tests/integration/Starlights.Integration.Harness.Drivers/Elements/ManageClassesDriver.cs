using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageClassesDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsScenarioContext _elementsContext;
    private readonly ManageClassesEndpointDriver _classesApi;
    private readonly ManageClassFeaturesEndpointDriver _classFeaturesApi;
    private readonly ManageSubClassesEndpointDriver _subClassesApi;

    public ManageClassesDriver(IIntegrationHost integration)
    {
        _integration = integration;
        _elementsContext = _integration.Get<ElementsScenarioContext>();
        _classesApi = _integration.GetDriver<ManageClassesEndpointDriver>();
        _classFeaturesApi = _integration.GetDriver<ManageClassFeaturesEndpointDriver>();
        _subClassesApi = _integration.GetDriver<ManageSubClassesEndpointDriver>();
    }

    public async Task<Guid> CreateClassAsync(CreateClassProperties properties)
    {
        var request = new CreateClassRequest
        {
            Name = properties.Name,
            HitPointDieSize = properties.HitPointDieSize,
            HitPointDieAmount = properties.HitPointDieAmount,
            Description = properties.Description,
            ShortDescription = properties.ShortDescription
        };

        var id = await _classesApi.CreateAsync(request);
        id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, id);

        return id;
    }

    public async Task<ClassDataModel> GetClassByIdAsync(Guid id)
    {
        var payload = await _classesApi.GetAsync(id);
        payload.Should().NotBeNull();
        return payload!;
    }

    public Task<ClassDataModel> GetClassByNameAsync(string name)
    {
        return !_elementsContext.CreatedMap.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No class found with name '{name}'.")
            : GetClassByIdAsync(id);
    }

    public async Task<Guid> CreateClassFeatureAsync(CreateClassFeatureProperties properties)
    {
        var request = new CreateClassFeatureRequest
        {
            Name = properties.Name,
            ParentClassId = properties.ParentClassId,
            ParentClassName = properties.ParentClassName,
            Level = properties.Level,
            Description = properties.Description
        };

        var id = await _classFeaturesApi.CreateAsync(request);
        id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, id);

        return id;
    }

    public Task<ClassFeatureDataModel> GetClassFeatureByNameAsync(string name)
    {
        return !_elementsContext.CreatedMap.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No class feature found with name '{name}'.")
            : GetClassFeatureByIdAsync(id);
    }

    public async Task<ClassFeatureDataModel> GetClassFeatureByIdAsync(Guid id)
    {
        var payload = await _classFeaturesApi.GetAsync(id);
        payload.Should().NotBeNull();
        return payload!;
    }

    public async Task<Guid> CreateSubClassAsync(CreateSubClassProperties properties)
    {
        var request = new CreateSubClassRequest
        {
            Name = properties.Name,
            ParentClassId = properties.ParentClassId,
            ParentClassName = properties.ParentClassName,
            Description = properties.Description
        };

        var id = await _subClassesApi.CreateAsync(request);
        id.Should().NotBeEmpty();

        _elementsContext.ElementCreated(properties.Name, id);

        return id;
    }

    public Task<SubClassDataModel> GetSubClassByNameAsync(string name)
    {
        return !_elementsContext.CreatedMap.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No subclass found with name '{name}'.")
            : GetSubClassByIdAsync(id);
    }

    public async Task<SubClassDataModel> GetSubClassByIdAsync(Guid id)
    {
        var payload = await _subClassesApi.GetAsync(id);
        payload.Should().NotBeNull();
        return payload;
    }

    public sealed record CreateClassProperties
    {
        public required string Name { get; init; }
        public required int HitPointDieSize { get; init; }
        public int HitPointDieAmount { get; init; } = 1;
        public required string Description { get; init; }
        public string? ShortDescription { get; init; }
    }

    public sealed record CreateClassFeatureProperties
    {
        public required string Name { get; init; }
        public required int Level { get; init; }
        public required Guid ParentClassId { get; init; }
        public required string ParentClassName { get; init; }
        public string? Description { get; init; } = string.Empty;
    }

    public sealed record CreateSubClassProperties
    {
        public required string Name { get; init; }
        public required Guid ParentClassId { get; init; }
        public required string ParentClassName { get; init; }
        public string? Description { get; init; } = string.Empty;
    }
}
