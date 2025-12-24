using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForGenericElementsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForGenericElementsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"the content creator creates an element with the following properties")]
    public async Task WhenTheContentCreatorCreatesAnElementWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<ElementTableRow>(_scenarioContext);

        var properties = new ManageElementsDriver.CreateProperties
        {
            Name = row.Name,
            Type = row.Type,
            Description = row.Description
        };

        await _elementsDriver.CreateElement(properties);
    }

    [Then(@"the element should have at least the following properties")]
    public async Task ThenTheElementShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var element = await _elementsDriver.GetLastCreatedElement();
        var expected = dataTable.CreateInstance<ElementTableRow>(_scenarioContext);

        var assertions = new Dictionary<string, Action<ElementTableRow, ElementDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["type"] = (e, a) => a.Type.Should().Be(e.Type),
            ["description"] = (e, a) => a.Description.Should().Be(e.Description ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, element, assertions);
    }

    [Given(@"an element exists with the following properties")]
    public async Task GivenAnElementExistsWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<ElementTableRow>(_scenarioContext);

        var properties = new ManageElementsDriver.CreateProperties
        {
            Name = row.Name,
            Type = row.Type,
            Description = row.Description
        };

        await _elementsDriver.CreateElement(properties);
    }

    #region Element Rules

    [When(@"the content creator adds a new statistic rule to the element with the following properties")]
    public async Task WhenTheContentCreatorAddsANewStatisticRuleToTheElementWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<StatisticRuleTableRow>(_scenarioContext);

        var elementId = _host.Get<Guid>("last-created-element-id");

        var properties = new ManageElementsDriver.CreateStatisticRuleProperties
        {
            Name = row.Name,
            Value = row.Value,
            StackingBonus = row.StackingBonus,
            LevelRequirement = row.LevelRequirement ?? 0
        };

        await _elementsDriver.CreateStatisticRule(elementId, properties);
    }

    [Then(@"the element should have a statistic rule with the following properties")]
    public async Task ThenTheElementShouldHaveAStatisticRuleWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var expected = dataTable.CreateInstance<StatisticRuleTableRow>(_scenarioContext);

        var elementId = _host.Get<Guid>("last-created-element-id");
        var createdRule = _host.Get<CreateStatisticRuleResponse>("last-created-statistic-rule");

        var rule = await _elementsDriver.GetStatisticRuleById(elementId, createdRule.RuleId);

        var assertions = new Dictionary<string, Action<StatisticRuleTableRow, GetStatisticRuleResponse>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name.Trim().ToLowerInvariant()),
            ["value"] = (e, a) => a.Value.Should().Be(e.Value.Trim().ToLowerInvariant()),
            ["stacking bonus"] = (e, a) => a.StackingBonus.Should().Be(e.StackingBonus?.Trim().ToLowerInvariant()),
            ["level requirement"] = (e, a) => a.LevelRequirement.Should().Be(e.LevelRequirement ?? 0)
        };

        dataTable.AssertProvidedProperties(expected, rule, assertions);

        var rules = await _elementsDriver.GetStatisticRules(elementId);
        rules.Should().Contain(r => r.RuleId == createdRule.RuleId);
    }

    //[When(@"the content creator adds a new statistic rule to the ""([^""]*)"" element with the following properties")]
    //public async Task WhenTheContentCreatorAddsANewStatisticRuleToTheElementWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    //{
    //    throw new PendingStepException();
    //}

    //[Then(@"the ""([^""]*)"" element should have a statistic rule with the following properties")]
    //public async Task ThenTheElementShouldHaveAStatisticRuleWithTheFollowingPropertiesAsync(string name, DataTable dataTable)
    //{
    //    throw new PendingStepException();
    //}

    #endregion


    private sealed class ElementTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }

    private sealed class StatisticRuleTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? StackingBonus { get; set; }
        public int? LevelRequirement { get; set; }
        public string? Description { get; set; }
    }
}
