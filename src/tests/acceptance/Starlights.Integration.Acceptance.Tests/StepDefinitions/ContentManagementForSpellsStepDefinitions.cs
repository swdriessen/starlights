using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Spells;
using static Starlights.Integration.Drivers.Elements.ManageSpellsDriver;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForSpellsStepDefinitions
{
    public const string CURRENT_SPELL_ID = "CURRENT-SPELL-ID";
    public const string CURRENT_SPELL_PROPERTIES = "CURRENT-SPELL-PROPERTIES";

    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageSpellsDriver _driver;

    public ContentManagementForSpellsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _driver = _host.GetDriver<ManageSpellsDriver>();
    }

    [Given("I am authenticated as a content creator")]
    public void GivenIAmAuthenticatedAsAContentCreator()
    {
        _host.WriteStepNotImplemented();
    }

    [Given("there are no existing spells")]
    public async Task GivenThereAreNoExistingSpells()
    {
        var spells = await _driver.GetSpells();
        spells.Should().BeEmpty("expected no existing spells before test execution");
    }

    [When("the content creator creates a spell with the following properties")]
    public async Task WhenTheContentCreatorCreatesASpellWithTheFollowingProperties(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<CreateSpellTableRow>();
        _scenarioContext.Set(row, CURRENT_SPELL_PROPERTIES);

        var properties = new CreateProperties
        {
            Name = row.Name,
            Level = row.Level,
            MagicSchool = row.MagicSchool,
            CastingTime = row.CastingTime,
            Range = row.Range,
            Duration = row.Duration,
            IsConcentration = row.Concentration,
            IsRitual = row.Ritual,
            HasSomatic = row.Somatic,
            HasVerbal = row.Verbal,
            HasMaterial = row.Material,
            MaterialComponent = row.MaterialComponents,
            Description = row.Description
        };

        var id = await _driver.CreateSpell(properties);
        id.Should().NotBeEmpty();

        _scenarioContext.Set(id, CURRENT_SPELL_ID);
    }

    [Then("the spell appears in the spell list with all provided properties")]
    public async Task ThenTheSpellAppearsInTheSpellListWithAllProvidedProperties()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _driver.GetSpell(id);
        spell.Should().NotBeNull();

        var expected = _scenarioContext.Get<CreateSpellTableRow>(CURRENT_SPELL_PROPERTIES);

        spell.Name.Should().Be(expected.Name);
        spell.Level.Should().Be(expected.Level);
        spell.MagicSchool.Should().Be(expected.MagicSchool);
        spell.CastingTime.Should().Be(expected.CastingTime);
        spell.Range.Should().Be(expected.Range);
        spell.Duration.Should().Be(expected.Duration);
        spell.IsConcentration.Should().Be(expected.Concentration);
        spell.IsRitual.Should().Be(expected.Ritual);
        spell.HasSomatic.Should().Be(expected.Somatic);
        spell.HasVerbal.Should().Be(expected.Verbal);
        spell.HasMaterial.Should().Be(expected.Material);
        spell.MaterialComponent.Should().Be(expected.MaterialComponents);
        spell.Description.Should().Be(expected.Description);
    }

    [Then("the spell appears in the spell list as a concentration spell")]
    public async Task ThenTheSpellAppearsInTheSpellListAsAConcentrationSpell()
    {
        var spell = await _driver.GetLastCreatedSpell();
        spell.IsConcentration.Should().BeTrue("expected the spell to be a concentration spell");
    }

    [Then("the spell appears in the spell list as a ritual spell")]
    public async Task ThenTheSpellAppearsInTheSpellListAsARitualSpell()
    {
        var spell = await _driver.GetLastCreatedSpell();
        spell.IsRitual.Should().BeTrue("expected the spell to be a ritual spell");
    }

    [Then("the spell appears in the spell list as having a somatic component")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingASomaticComponent()
    {
        var spell = await _driver.GetLastCreatedSpell();
        spell.HasSomatic.Should().BeTrue("expected the spell to have a somatic component");
    }

    [Then("the spell appears in the spell list as having a verbal component")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingAVerbalComponent()
    {
        var spell = await _driver.GetLastCreatedSpell();
        spell.HasVerbal.Should().BeTrue("expected the spell to have a verbal component");
    }

    [Then("the spell appears in the spell list as having a material component with the provided material components description")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingAMaterialComponentWithTheProvidedMaterialComponentsDescription()
    {
        var row = _scenarioContext.Get<CreateSpellTableRow>(CURRENT_SPELL_PROPERTIES);

        var spell = await _driver.GetLastCreatedSpell();
        spell.HasMaterial.Should().BeTrue("expected the spell to have a material component");
        spell.MaterialComponent.Should().Be(row.MaterialComponents);
    }

    [Then("the spell appears in the spell list with the provided description")]
    public async Task ThenTheSpellAppearsInTheSpellListWithTheProvidedDescription()
    {
        var row = _scenarioContext.Get<CreateSpellTableRow>(CURRENT_SPELL_PROPERTIES);

        var spell = await _driver.GetLastCreatedSpell();
        spell.Description.Should().Be(row.Description);
    }

    [Given("a spell exists that includes the following properties")]
    public async Task GivenASpellExistsWithTheFollowingProperties(DataTable dataTable)
    {
        var input = dataTable.CreateInstance<CreateSpellTableRow>();
        _scenarioContext.Set(input, CURRENT_SPELL_PROPERTIES);

        var row = new CreateSpellTableRow()
        {
            Name = "Name",
            Level = 0,
            MagicSchool = "MagicSchool",
            CastingTime = "CastingTime",
            Range = "Range",
            Duration = "Duration"
        };

        dataTable.FillInstance(row); // fill the properties from the table which may not contain all required fields

        var properties = new CreateProperties
        {
            Name = row.Name,
            Level = row.Level,
            MagicSchool = row.MagicSchool,
            CastingTime = row.CastingTime,
            Range = row.Range,
            Duration = row.Duration,
            IsConcentration = row.Concentration,
            IsRitual = row.Ritual,
            HasSomatic = row.Somatic,
            HasVerbal = row.Verbal,
            HasMaterial = row.Material,
            MaterialComponent = row.MaterialComponents,
            Description = row.Description
        };

        await _driver.CreateSpell(properties);

        var existingSpell = await _driver.GetLastCreatedSpell();
        existingSpell.Name.Should().Be(row.Name);
        existingSpell.Level.Should().Be(row.Level);
        existingSpell.MagicSchool.Should().Be(row.MagicSchool);
        existingSpell.CastingTime.Should().Be(row.CastingTime);
        existingSpell.Range.Should().Be(row.Range);
        existingSpell.Duration.Should().Be(row.Duration);
        existingSpell.IsConcentration.Should().Be(row.Concentration);
        existingSpell.IsRitual.Should().Be(row.Ritual);
        existingSpell.HasSomatic.Should().Be(row.Somatic);
        existingSpell.HasVerbal.Should().Be(row.Verbal);
        existingSpell.HasMaterial.Should().Be(row.Material);
        existingSpell.MaterialComponent.Should().Be(row.MaterialComponents);
        existingSpell.Description.Should().Be(row.Description);
    }

    [When("the content creator updates the level of the spell to {int}")]
    public async Task WhenTheContentCreatorUpdatesTheLevelOfTheSpellTo(int level)
    {
        var existingSpell = await _driver.GetLastCreatedSpell();
        var updatedSpell = existingSpell with { Level = level };
        await _driver.UpdateSpell(updatedSpell);
    }

    [Then("the spell in the spell list should have a level of {int}")]
    public async Task ThenTheSpellInTheSpellListShouldHaveALevelOf(int level)
    {
        var existingSpell = await _driver.GetLastCreatedSpell();
        existingSpell.Level.Should().Be(level);
    }

    [When("the content creator updates the {string} of the spell to {string}")]
    public async Task WhenTheContentCreatorUpdatesTheOfTheSpellTo(string propertyName, string propertyValue)
    {
        var spell = await _driver.GetLastCreatedSpell();

        var updatedSpell = ApplyPropertyUpdate(propertyName, propertyValue, spell);

        await _driver.UpdateSpell(updatedSpell);
    }

    [Then("the spell in the spell list should have a {string} of {string}")]
    public async Task ThenTheSpellInTheSpellListShouldHaveAOf(string propertyName, string propertyValue)
    {
        var spell = await _driver.GetLastCreatedSpell();

        var actualValue = propertyName switch
        {
            "name" => spell.Name,
            "magic school" => spell.MagicSchool,
            "casting time" => spell.CastingTime,
            "range" => spell.Range,
            "duration" => spell.Duration,
            _ => throw new NotImplementedException($"checking property '{propertyName}' is not implemented")
        };

        actualValue.Should().Be(propertyValue);
    }

    [When("the content creator updates the spell with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheSpellWithTheFollowingProperties(DataTable dataTable)
    {
        var existingSpell = await _driver.GetLastCreatedSpell();

        var updates = dataTable.CreateInstance<UpdateSpellTableRow>();

        var updatedSpell = existingSpell with
        {
            Name = updates.Name ?? existingSpell.Name,
            Level = updates.Level ?? existingSpell.Level,
            MagicSchool = updates.MagicSchool ?? existingSpell.MagicSchool,
            CastingTime = updates.CastingTime ?? existingSpell.CastingTime,
            Range = updates.Range ?? existingSpell.Range,
            Duration = updates.Duration ?? existingSpell.Duration,
            IsConcentration = updates.Concentration ?? existingSpell.IsConcentration,
            IsRitual = updates.Ritual ?? existingSpell.IsRitual,
            HasSomatic = updates.Somatic ?? existingSpell.HasSomatic,
            HasVerbal = updates.Verbal ?? existingSpell.HasVerbal,
            HasMaterial = updates.Material ?? existingSpell.HasMaterial,
            MaterialComponent = updates.MaterialComponents ?? existingSpell.MaterialComponent,
            Description = updates.Description ?? existingSpell.Description
        };

        await _driver.UpdateSpell(updatedSpell);
    }

    [Then("the spell in the spell list should have the following properties")]
    public async Task ThenTheSpellInTheSpellListShouldHaveTheFollowingProperties(DataTable dataTable)
    {
        var existingSpell = await _driver.GetLastCreatedSpell();
        var expected = dataTable.CreateInstance<UpdateSpellTableRow>();

        var provided = dataTable.Header
            .Select(h => h.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var assertions = new Dictionary<string, Action<UpdateSpellTableRow, SpellDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, s) => s.Name.Should().Be(e.Name),
            ["level"] = (e, s) => s.Level.Should().Be(e.Level!.Value),
            ["magic school"] = (e, s) => s.MagicSchool.Should().Be(e.MagicSchool),
            ["casting time"] = (e, s) => s.CastingTime.Should().Be(e.CastingTime),
            ["range"] = (e, s) => s.Range.Should().Be(e.Range),
            ["duration"] = (e, s) => s.Duration.Should().Be(e.Duration),
            ["concentration"] = (e, s) => s.IsConcentration.Should().Be(e.Concentration!.Value),
            ["ritual"] = (e, s) => s.IsRitual.Should().Be(e.Ritual!.Value),
            ["somatic"] = (e, s) => s.HasSomatic.Should().Be(e.Somatic!.Value),
            ["verbal"] = (e, s) => s.HasVerbal.Should().Be(e.Verbal!.Value),
            ["material"] = (e, s) => s.HasMaterial.Should().Be(e.Material!.Value),
            ["material components"] = (e, s) => s.MaterialComponent.Should().Be(e.MaterialComponents),
            ["description"] = (e, s) => s.Description.Should().Be(e.Description)
        };

        foreach (var header in provided)
        {
            if (assertions.TryGetValue(header, out var assert))
            {
                assert(expected, existingSpell);
            }
            else
            {
                throw new NotImplementedException($"checking property '{header}' is not implemented");
            }
        }
    }

    private static SpellDataModel ApplyPropertyUpdate(string propertyName, string propertyValue, SpellDataModel spell)
    {
        return propertyName switch
        {
            "name" => spell with { Name = propertyValue },
            "magic school" => spell with { MagicSchool = propertyValue },
            "casting time" => spell with { CastingTime = propertyValue },
            "range" => spell with { Range = propertyValue },
            "duration" => spell with { Duration = propertyValue },
            "concentration" => spell with { IsConcentration = bool.Parse(propertyValue) },
            _ => throw new NotImplementedException($"updating property '{propertyName}' is not implemented")
        };
    }
}
