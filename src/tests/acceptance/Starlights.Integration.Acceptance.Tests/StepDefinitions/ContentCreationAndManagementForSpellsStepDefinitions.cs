using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentCreationAndManagementForSpellsStepDefinitions
{
    public const string CURRENT_SPELL_ID = "CURRENT-SPELL-ID";
    public const string CURRENT_SPELL_PROPERTIES = "CURRENT-SPELL-PROPERTIES";

    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ElementsCreationDriver _elements;

    public ContentCreationAndManagementForSpellsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elements = _host.GetDriver<ElementsCreationDriver>();
    }

    [Given("I am authenticated as a content creator")]
    public void GivenIAmAuthenticatedAsAContentCreator()
    {
        _host.WriteStepNotImplemented();
    }

    [Given("there are no existing spells")]
    public async Task GivenThereAreNoExistingSpells()
    {
        var spells = await _elements.GetSpellsAsync();
        spells.Should().BeEmpty("expected no existing spells before test execution");
    }

    [When("the content creator creates a spell with the following properties")]
    public async Task WhenTheContentCreatorCreatesASpellWithTheFollowingProperties(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<CreateSpellTableRow>();
        _scenarioContext.Set(row, CURRENT_SPELL_PROPERTIES);

        var properties = new CreateSpellProperties
        {
            Name = row.Name!,
            Level = row.Level,
            MagicSchool = row.MagicSchool!,
            CastingTime = row.CastingTime!,
            Range = row.Range!,
            Duration = row.Duration!,
            IsConcentration = row.Concentration,
            IsRitual = row.Ritual,
            HasSomatic = row.Somatic,
            HasVerbal = row.Verbal,
            HasMaterial = row.Material,
            MaterialComponent = row.MaterialComponents,
            Description = row.Description
        };

        var id = await _elements.CreateSpellAsync(properties);
        id.Should().NotBeEmpty();

        _scenarioContext.Set(id, CURRENT_SPELL_ID);
    }


    [Then("the spell appears in the spell list with all provided properties")]
    public async Task ThenTheSpellAppearsInTheSpellListWithAllProvidedProperties()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _elements.GetSpellByIdAsync(id);
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
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.IsConcentration.Should().BeTrue("expected the spell to be a concentration spell");
    }

    [Then("the spell appears in the spell list as a ritual spell")]
    public async Task ThenTheSpellAppearsInTheSpellListAsARitualSpell()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.IsRitual.Should().BeTrue("expected the spell to be a ritual spell");
    }

    [Then("the spell appears in the spell list as having a somatic component")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingASomaticComponent()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.HasSomatic.Should().BeTrue("expected the spell to have a somatic component");
    }

    [Then("the spell appears in the spell list as having a verbal component")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingAVerbalComponent()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.HasVerbal.Should().BeTrue("expected the spell to have a verbal component");
    }

    [Then("the spell appears in the spell list as having a material component with the provided material components description")]
    public async Task ThenTheSpellAppearsInTheSpellListAsHavingAMaterialComponentWithTheProvidedMaterialComponentsDescription()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var expected = _scenarioContext.Get<CreateSpellTableRow>(CURRENT_SPELL_PROPERTIES);

        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.HasMaterial.Should().BeTrue("expected the spell to have a material component");
        spell.MaterialComponent.Should().Be(expected.MaterialComponents);
    }

    [Then("the spell appears in the spell list with the provided description")]
    public async Task ThenTheSpellAppearsInTheSpellListWithTheProvidedDescription()
    {
        var id = _scenarioContext.Get<Guid>(CURRENT_SPELL_ID);
        var expected = _scenarioContext.Get<CreateSpellTableRow>(CURRENT_SPELL_PROPERTIES);

        var spell = await _elements.GetSpellByIdAsync(id);
        spell.Should().NotBeNull();
        spell.Description.Should().Be(expected.Description);
    }
}

internal record CreateSpellTableRow
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string MagicSchool { get; set; } = string.Empty;
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public bool Concentration { get; set; }
    public bool Ritual { get; set; }
    public bool Somatic { get; set; }
    public bool Verbal { get; set; }
    public bool Material { get; set; }
    public string? MaterialComponents { get; set; }
    public string Description { get; set; } = string.Empty;
}
