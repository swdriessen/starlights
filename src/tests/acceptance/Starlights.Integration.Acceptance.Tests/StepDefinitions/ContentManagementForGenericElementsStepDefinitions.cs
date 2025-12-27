using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;

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

    [Given(@"an element exists with the name ""([^""]*)""")]
    public async Task GivenAnElementExistsWithTheNameAsync(string elementName)
    {
        var properties = new ManageElementsDriver.CreateProperties
        {
            Name = elementName,
            Type = "Type",
            Description = $"Description for {elementName}"
        };

        await _elementsDriver.CreateElement(properties);
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
            LevelRequirement = row.LevelRequirement ?? 0,
            DisplayName = row.DisplayName,
            Minimum = row.Minimum,
            Maximum = row.Maximum
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
            ["level requirement"] = (e, a) => a.LevelRequirement.Should().Be(e.LevelRequirement ?? 0),
            ["display name"] = (e, a) => a.DisplayName.Should().Be(e.DisplayName),
            ["minimum"] = (e, a) => a.Minimum.Should().Be(e.Minimum),
            ["maximum"] = (e, a) => a.Maximum.Should().Be(e.Maximum)
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

    [Given(@"the element has the following statistic rules")]
    public async Task GivenTheElementHasTheFollowingStatisticRulesAsync(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<StatisticRuleTableRow>(_scenarioContext);

        var elementId = _host.Get<Guid>("last-created-element-id");

        foreach (var row in rows)
        {
            var properties = new ManageElementsDriver.CreateStatisticRuleProperties
            {
                Name = row.Name,
                Value = row.Value,
                StackingBonus = row.StackingBonus,
                LevelRequirement = row.LevelRequirement ?? 0
            };

            await _elementsDriver.CreateStatisticRule(elementId, properties);
        }
    }

    [When(@"the content creator deletes the statistic rule with the name ""([^""]*)""")]
    public async Task WhenTheContentCreatorDeletesTheStatisticRuleWithTheNameAsync(string dexterity)
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetStatisticRules(elementId);
        var rule = rules.SingleOrDefault(r => r.Name.Equals(dexterity, StringComparison.OrdinalIgnoreCase));

        rule.Should().NotBeNull("Expected to find a statistic rule with the name '{0}', but none was found.", dexterity);

        var result = await _elementsDriver.DeleteStatisticRule(elementId, rule.RuleId);
        result.Should().BeTrue("Expected the deletion of the statistic rule '{0}' to succeed.", dexterity);
    }

    [Then(@"the element should have the following statistic rules")]
    public async Task ThenTheElementShouldHaveTheFollowingStatisticRulesAsync(DataTable dataTable)
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetStatisticRules(elementId);

        var expectedRows = dataTable.CreateSet<StatisticRuleTableRow>(_scenarioContext).ToList();

        rules.Should().HaveCount(expectedRows.Count);

        for (var i = 0; i < expectedRows.Count; i++)
        {
            var expected = expectedRows[i];
            var rule = rules[i];

            var assertions = new Dictionary<string, Action<StatisticRuleTableRow, GetStatisticRulesResponse.StatisticRuleItem>>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = (e, a) => a.Name.Should().Be(e.Name),
                ["value"] = (e, a) => a.Value.Should().Be(e.Value),
            };

            dataTable.AssertProvidedProperties(expected, rule, assertions);
        }
    }

    [When(@"the content creator re-arranges the statistic rules to the following order")]
    public async Task WhenTheContentCreatorRe_ArrangesTheStatisticRulesToTheFollowingOrderAsync(DataTable dataTable)
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetStatisticRules(elementId);



        var orderedRuleIds = new List<Guid>();
        foreach (var row in dataTable.CreateSet<StatisticRuleTableRow>(_scenarioContext))
        {
            var rule = rules.SingleOrDefault(r => r.Name.Equals(row.Name, StringComparison.OrdinalIgnoreCase));
            rule.Should().NotBeNull("Expected to find a statistic rule with the name '{0}', but none was found.", row.Name);
            orderedRuleIds.Add(rule.RuleId);
        }



        await _elementsDriver.ReorderRules(elementId, orderedRuleIds);
    }


    #endregion


    private sealed record ElementTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }

    private sealed record StatisticRuleTableRow : IMarkdownDescriptionTableRow
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? StackingBonus { get; set; }
        public int? LevelRequirement { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public string? Description { get; set; }
        public string? DisplayName { get; set; }
    }
}
