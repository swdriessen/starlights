using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Create;
using Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

namespace Starlights.Integration.Drivers.Elements;

public class ManageSpellsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageSpellsEndpointDriver _api;

    public ManageSpellsDriver(IIntegrationHost integration, ManageSpellsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
    }

    public async Task<Guid> CreateSpell(CreateProperties properties)
    {
        _integration.WriteLine($"creating spell {properties.Name}");

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

        _integration.Properties["last-created-spell-id"] = id;
        _integration.Properties["last-created-spell-properties"] = properties;

        return id;
    }

    public async Task<SpellDataModel?> GetSpell(Guid id)
    {
        _integration.WriteLine($"retrieving spell {id}");

        var spell = await _api.GetAsync(id);

        if (spell is not null)
        {
            _integration.Properties["last-retrieved-spell"] = spell;
        }

        return spell;
    }

    public async Task<SpellDataModel> GetLastCreatedSpell()
    {
        var id = (Guid)_integration.Properties["last-created-spell-id"]!;
        var spell = await GetSpell(id);
        spell.Should().NotBeNull();
        return spell;
    }

    public Task<bool> UpdateSpell(SpellDataModel updatedModel)
    {
        _integration.WriteLine($"updating spell {updatedModel.Name} ({updatedModel.Id})");

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
        var spells = await _api.GetAsync();

        _integration.Properties["last-retrieved-spells"] = spells;

        _integration.WriteLine($"retrieved {spells.Count} spells.");

        foreach (var spell in spells)
        {
            _integration.WriteLine($"- {spell.Name} (Level {spell.Level} {spell.MagicSchool})");
        }

        return spells;
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

        public static CreateProperties Empty()
        {
            return new CreateProperties
            {
                Name = string.Empty,
                Level = 0,
                MagicSchool = string.Empty,
                CastingTime = string.Empty,
                Range = string.Empty,
                Duration = string.Empty,
                IsConcentration = false,
                IsRitual = false,
                HasSomatic = false,
                HasVerbal = false,
                HasMaterial = false,
                MaterialComponent = null,
                Description = string.Empty
            };
        }
    }
}