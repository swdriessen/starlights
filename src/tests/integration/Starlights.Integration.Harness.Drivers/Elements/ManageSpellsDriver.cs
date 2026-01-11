using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.Create;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ManageSpellsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly ManageSpellsEndpointDriver _api;

    public ManageSpellsDriver(IIntegrationHost integration, ElementsDriverContext driverContext, ManageSpellsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _driverContext = driverContext;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateSpell(CreateProperties properties)
    {
        var request = new CreateSpellRequest
        {
            Name = properties.Name,
            Level = properties.Level,
            MagicSchool = properties.MagicSchool,
            CastingTime = properties.CastingTime,
            Range = properties.Range,
            Duration = properties.Duration,
            IsConcentration = properties.IsConcentration,
            IsRitual = properties.IsRitual,
            HasSomatic = properties.HasSomatic,
            HasVerbal = properties.HasVerbal,
            HasMaterial = properties.HasMaterial,
            MaterialComponent = properties.MaterialComponent,
            Description = properties.Description
        };

        var id = await _api.CreateAsync(request);

        _driverContext.WithCreatedElement(id, request.Name, ElementTypeConstants.Spell);

        return id;
    }

    public async Task<SpellDataModel?> GetSpell(Guid id)
    {
        return await _api.GetAsync(id);
    }

    public async Task<SpellDataModel> GetLastCreatedSpell()
    {
        var spell = await _api.GetAsync(_driverContext.CurrentElement.Id);
        spell.Should().NotBeNull();
        return spell;
    }

    public Task<bool> UpdateSpell(SpellDataModel updatedModel)
    {
        var request = new UpdateSpellRequest()
        {
            Id = updatedModel.Id,
            Name = updatedModel.Name,
            Level = updatedModel.Level,
            MagicSchool = updatedModel.MagicSchool,
            CastingTime = updatedModel.CastingTime,
            Range = updatedModel.Range,
            Duration = updatedModel.Duration,
            IsConcentration = updatedModel.IsConcentration,
            IsRitual = updatedModel.IsRitual,
            HasSomatic = updatedModel.HasSomatic,
            HasVerbal = updatedModel.HasVerbal,
            HasMaterial = updatedModel.HasMaterial,
            MaterialComponent = updatedModel.MaterialComponent,
            Description = updatedModel.Description
        };

        return _api.PutAsync(request);
    }

    public async Task<List<SpellDataModel>> GetSpells()
    {
        return await _api.GetAsync();
    }

    public class CreateProperties
    {
        public required string Name { get; set; }
        public required int Level { get; set; }
        public required string MagicSchool { get; set; }
        public required string CastingTime { get; set; }
        public required string Range { get; set; }
        public required string Duration { get; set; }
        public bool IsConcentration { get; set; }
        public bool IsRitual { get; set; }
        public bool HasSomatic { get; set; }
        public bool HasVerbal { get; set; }
        public bool HasMaterial { get; set; }
        public string? MaterialComponent { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}