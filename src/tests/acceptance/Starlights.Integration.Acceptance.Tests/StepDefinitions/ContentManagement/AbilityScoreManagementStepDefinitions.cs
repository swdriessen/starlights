using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public sealed class AbilityScoreManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageAbilityScoresDriver _driver;

    public AbilityScoreManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _driver = _host.GetDriver<ManageAbilityScoresDriver>();
    }

    [When("a content creator creates an ability score with the following properties")]
    public async Task WhenAContentCreatorCreatesAnAbilityScoreWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var properties = new ManageAbilityScoresDriver.CreateProperties
        {
            Name = row.Name,
            Abbreviation = row.Abbreviation,
            Description = row.Description
        };

        await _driver.CreateAbilityScoreAsync(properties);
    }

    [Then("the ability score should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var abilityScore = await _driver.GetLastCreatedAbilityScore();

        var expected = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<AbilityScoreTableRow, AbilityScoreDataModel>>()
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["abbreviation"] = (e, a) => a.Abbreviation.Should().Be(e.Abbreviation),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, abilityScore, assertions);
    }

    [Given(@"an ability score exists with the name ""([^""]*)"" and an abbreviation ""([^""]*)""")]
    public async Task GivenAnAbilityScoreExistsWithTheNameAndAnAbbreviationAsync(string name, string abbreviation)
    {
        var properties = new ManageAbilityScoresDriver.CreateProperties
        {
            Name = name,
            Abbreviation = abbreviation,
            Description = string.Empty
        };

        await _driver.CreateAbilityScoreAsync(properties);
    }

    [When(@"the content creator updates the ability score ""([^""]*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheAbilityScoreWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var existing = await _driver.GetAbilityScoreByNameAsync(name);

        var updates = dataTable.Rows.Count == 0
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : dataTable.Rows[0]
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString(),
                    StringComparer.OrdinalIgnoreCase);

        var properties = ManageAbilityScoresDriver.UpdateProperties.FromDictionary(updates);

        await _driver.UpdateAbilityScoreAsync(existing.Id, properties);
    }

    [Then(@"the ability score ""([^""]*)"" should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var actual = await _driver.GetAbilityScoreByNameAsync(name);
        var expected = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<AbilityScoreTableRow, AbilityScoreDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["abbreviation"] = (e, a) => a.Abbreviation.Should().Be(e.Abbreviation),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, actual, assertions);
    }

    #region Table Bindings

    private sealed class AbilityScoreTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    #endregion
}
