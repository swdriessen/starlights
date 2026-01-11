using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.SavingThrows.GetSavingThrows;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class SavingThrowManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ElementsDriverContext _driverContext;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageSavingThrowsDriver _savingThrowsDriver;

    public SavingThrowManagementStepDefinitions(IIntegrationHost host, ElementsDriverContext driverContext, ScenarioContext scenarioContext)
    {
        _host = host;
        _driverContext = driverContext;
        _scenarioContext = scenarioContext;
        _savingThrowsDriver = _host.GetDriver<ManageSavingThrowsDriver>();
    }

    [When("a content creator creates a saving throw with the following properties")]
    public async Task WhenAContentCreatorCreatesASavingThrowWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var properties = new ManageSavingThrowsDriver.CreateProperties
        {
            Name = row.Name,
            AbilityName = row.Ability
        };

        await _savingThrowsDriver.CreateSavingThrowAsync(properties);
    }

    [Then("the saving throw should have at least the following properties")]
    public async Task ThenTheSavingThrowShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var id = _driverContext.CurrentElement.Id;
        var savingThrow = await _savingThrowsDriver.GetSavingThrowByIdAsync(id);

        var expected = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SavingThrowTableRow, SavingThrowListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, savingThrow, assertions);
    }

    [Given(@"a saving throw exists with the name ""(.*)"" and ability ""(.*)""")]
    public async Task GivenASavingThrowExistsWithTheNameAndAbilityAsync(string name, string ability)
    {
        var properties = new ManageSavingThrowsDriver.CreateProperties
        {
            Name = name,
            AbilityName = ability
        };

        await _savingThrowsDriver.CreateSavingThrowAsync(properties);
    }

    [When(@"the content creator updates the saving throw ""(.*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheSavingThrowWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _driverContext.CurrentElement.Id;

        var updates = dataTable.Rows.Count == 0
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : dataTable.Rows[0]
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString(),
                    StringComparer.OrdinalIgnoreCase);

        var properties = ManageSavingThrowsDriver.UpdateProperties.FromDictionary(updates);

        await _savingThrowsDriver.UpdateSavingThrowAsync(id, properties);
    }

    [Then(@"the saving throw ""(.*)"" should have at least the following properties")]
    public async Task ThenTheSavingThrowShouldHaveAtLeastTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var id = _driverContext.CurrentElement.Id;
        var savingThrow = await _savingThrowsDriver.GetSavingThrowByIdAsync(id);

        var expected = dataTable.CreateInstance<SavingThrowTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<SavingThrowTableRow, SavingThrowListItem>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["ability"] = (e, a) => a.Ability.Should().Be(e.Ability),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, savingThrow, assertions);
    }

    #region Table Bindings

    private sealed class SavingThrowTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    #endregion
}
