using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Proficiencies.GetProficiencies;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class ProficiencyManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageProficienciesDriver _proficienciesDriver;

    public ProficiencyManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _proficienciesDriver = _host.GetDriver<ManageProficienciesDriver>();
    }

    [When(@"the content creator creates a proficiency with the following properties")]
    public async Task WhenTheContentCreatorCreatesAProficiencyWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<ProficiencyTableRow>(_scenarioContext);

        var properties = new ManageProficienciesDriver.CreateProperties
        {
            Name = row.Name,
            ProficiencyType = row.ProficiencyType
        };

        await _proficienciesDriver.CreateProficiencyAsync(properties);
    }

    [Then(@"the proficiency should have at least the following properties")]
    public async Task ThenTheProficiencyShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var id = _host.Get<Guid>("last-created-proficiency-id");
        var proficiency = await _proficienciesDriver.GetProficiencyByIdAsync(id);

        var expected = dataTable.CreateInstance<ProficiencyTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<ProficiencyTableRow, ProficiencyListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["proficiency type"] = (e, a) => a.ProficiencyType.Should().Be(e.ProficiencyType),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, proficiency, assertions);
    }

    private sealed class ProficiencyTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string ProficiencyType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
