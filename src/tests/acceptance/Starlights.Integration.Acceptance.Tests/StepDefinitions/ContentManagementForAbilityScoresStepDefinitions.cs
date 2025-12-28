using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.AbilityScores;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public sealed class ContentManagementForAbilityScoresStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageAbilityScoresDriver _abilityScoresDriver;

    public ContentManagementForAbilityScoresStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _abilityScoresDriver = _host.GetDriver<ManageAbilityScoresDriver>();
    }

    [When(@"a content creator creates an ability score with the following properties")]
    public async Task WhenAContentCreatorCreatesAnAbilityScoreWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var properties = new ManageAbilityScoresDriver.CreateProperties
        {
            Name = row.Name,
            Abbreviation = row.Abbreviation,
            Description = row.Description
        };

        await _abilityScoresDriver.CreateAbilityScoreAsync(properties);
    }

    [Then(@"the ability score should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var id = _host.Get<Guid>("last-created-ability-score-id");
        var abilityScore = await _abilityScoresDriver.GetAbilityScoreByIdAsync(id);

        var expected = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<AbilityScoreTableRow, AbilityScoreDataModel>>(StringComparer.OrdinalIgnoreCase)
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

        await _abilityScoresDriver.CreateAbilityScoreAsync(properties);
    }

    [When(@"the content creator updates the ability score ""([^""]*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheAbilityScoreWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var existing = await _abilityScoresDriver.GetAbilityScoreByNameAsync(name);

        var updates = dataTable.Rows.Count == 0
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : dataTable.Rows[0]
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString(),
                    StringComparer.OrdinalIgnoreCase);

        var properties = ManageAbilityScoresDriver.UpdateProperties.FromDictionary(updates);

        await _abilityScoresDriver.UpdateAbilityScoreAsync(existing.Id, properties);
    }

    [Then(@"the ability score ""([^""]*)"" should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(string name, DataTable dataTable)
    {
        var actual = await _abilityScoresDriver.GetAbilityScoreByNameAsync(name);
        var expected = dataTable.CreateInstance<AbilityScoreTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<AbilityScoreTableRow, AbilityScoreDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["abbreviation"] = (e, a) => a.Abbreviation.Should().Be(e.Abbreviation),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, actual, assertions);
    }

    private sealed class AbilityScoreTableRow : IMarkdownDescriptionTableRow
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
