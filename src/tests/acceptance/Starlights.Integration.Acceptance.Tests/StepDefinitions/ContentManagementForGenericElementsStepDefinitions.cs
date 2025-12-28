using AwesomeAssertions;
using Reqnroll.Assist;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetById;
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

    //[Given(@"elements exist with the following properties")]
    [Given(@"the following elements with their respective properties exists")]
    public async Task GivenElementsExistWithTheFollowingNamesAsync(DataTable dataTable)
    {
        var names = dataTable.CreateSet<ElementTableRow>(_scenarioContext);

        foreach (var row in names)
        {
            var properties = new ManageElementsDriver.CreateProperties
            {
                Name = row.Name,
                Type = row.Type ?? "Type",
                Description = $"Description for {row.Name}"
            };

            await _elementsDriver.CreateElement(properties);
        }
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
            Maximum = row.Maximum,
            RequirementsExpression = row.RequirementsExpression
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
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["value"] = (e, a) => a.Value.Should().Be(e.Value),
            ["stacking bonus"] = (e, a) => a.StackingBonus.Should().Be(e.StackingBonus),
            ["level requirement"] = (e, a) => a.LevelRequirement.Should().Be(e.LevelRequirement),
            ["display name"] = (e, a) => a.DisplayName.Should().Be(e.DisplayName),
            ["requirements"] = (e, a) => a.Requirements.Should().Be(e.RequirementsExpression),
            ["minimum"] = (e, a) => a.Minimum.Should().Be(e.Minimum),
            ["maximum"] = (e, a) => a.Maximum.Should().Be(e.Maximum)
        };

        dataTable.AssertProvidedProperties(expected, rule, assertions);

        var rules = await _elementsDriver.GetStatisticRules(elementId);
        rules.Should().Contain(r => r.RuleId == createdRule.RuleId);
    }

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

    [Given(@"the element has the following include rules")]
    public async Task GivenTheElementHasTheFollowingIncludeRulesAsync(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<IncludeRuleTableRow>(_scenarioContext);
        var elementId = _host.Get<Guid>("last-created-element-id");

        foreach (var row in rows)
        {
            var includedElement = await _elementsDriver.GetElementByName(row.IncludedElement);

            var properties = new ManageElementsDriver.CreateIncludeRuleProperties
            {
                IncludedElementId = includedElement.Id,
                LevelRequirement = row.LevelRequirement ?? 0,
                RequirementsExpression = row.RequirementsExpression,
                DisplayName = row.DisplayName
            };

            await _elementsDriver.CreateIncludeRule(elementId, properties);
        }
    }

    [Given(@"the element has the following selection rules")]
    public async Task GivenTheElementHasTheFollowingSelectionRulesAsync(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<SelectionRuleTableRow>(_scenarioContext);
        var elementId = _host.Get<Guid>("last-created-element-id");

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.Default))
            {
                throw new PendingStepException("Selection rule 'default' is specified in the feature, but the current API/domain does not support a default selection yet.");
            }

            var properties = new ManageElementsDriver.CreateSelectionRuleProperties
            {
                DisplayName = row.DisplayName,
                Type = row.Type,
                Supports = row.Supports,
                Range = row.Range,
                Quantity = row.Quantity ?? 1,
                Optional = row.Optional ?? false,
                LevelRequirement = row.LevelRequirement ?? 0,
                Requirements = row.Requirements,
                Default = row.Default
            };

            await _elementsDriver.CreateSelectionRule(elementId, properties);
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

    [When(@"the content creator deletes the include rule with the name ""([^""]*)"" from the ""([^""]*)"" element")]
    public async Task WhenTheContentCreatorDeletesTheIncludeRuleWithTheNameFromTheElementAsync(string includeRuleName, string elementName)
    {
        var element = await _elementsDriver.GetElementByName(elementName);
        var includedElement = await _elementsDriver.GetElementByName(includeRuleName);

        var rules = await _elementsDriver.GetIncludeRules(element.Id);
        var rule = rules.SingleOrDefault(r => r.IncludedElementId == includedElement.Id);

        rule.Should().NotBeNull("Expected to find an include rule for included element '{0}' on element '{1}', but none was found.", includeRuleName, elementName);

        var deleted = await _elementsDriver.DeleteRules(element.Id, [rule.RuleId]);
        deleted.Should().BeTrue("Expected deletion of include rule '{0}' from element '{1}' to succeed.", includeRuleName, elementName);
    }

    [When(@"the content creator deletes the selection rule with the name ""([^""]*)"" from the ""([^""]*)"" element")]
    public async Task WhenTheContentCreatorDeletesTheSelectionRuleWithTheNameFromTheElementAsync(string selectionRuleName, string elementName)
    {
        var element = await _elementsDriver.GetElementByName(elementName);

        var rules = await _elementsDriver.GetSelectionRules(element.Id);
        var rule = rules.SingleOrDefault(r => r.DisplayName.Equals(selectionRuleName, StringComparison.OrdinalIgnoreCase));

        rule.Should().NotBeNull("Expected to find a selection rule with display name '{0}' on element '{1}', but none was found.", selectionRuleName, elementName);

        var deleted = await _elementsDriver.DeleteRules(element.Id, [rule.RuleId]);
        deleted.Should().BeTrue("Expected deletion of selection rule '{0}' from element '{1}' to succeed.", selectionRuleName, elementName);
    }


    [When(@"the content creator deletes all the rules from the element")]
    public async Task WhenTheContentCreatorDeletesAllTheRulesFromTheElementAsync()
    {
        var elementId = _host.Get<Guid>("last-created-element-id");

        var statisticRules = await _elementsDriver.GetStatisticRules(elementId);
        var includeRules = await _elementsDriver.GetIncludeRules(elementId);
        var selectionRules = await _elementsDriver.GetSelectionRules(elementId);

        var ruleIds = statisticRules.Select(r => r.RuleId)
            .Concat(includeRules.Select(r => r.RuleId))
            .Concat(selectionRules.Select(r => r.RuleId))
            .ToList();

        ruleIds.Should().NotBeEmpty("Expected at least one rule to be present before deleting all rules.");

        var deleted = await _elementsDriver.DeleteRules(elementId, ruleIds);
        deleted.Should().BeTrue("Expected deleting all rules to succeed.");
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

    [Then(@"the element should have the following selection rules")]
    public async Task ThenTheElementShouldHaveTheFollowingSelectionRulesAsync(DataTable dataTable)
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetSelectionRules(elementId);

        var expectedRows = dataTable.CreateSet<SelectionRuleTableRow>(_scenarioContext).ToList();

        rules.Should().HaveCount(expectedRows.Count);

        for (var i = 0; i < expectedRows.Count; i++)
        {
            var expected = expectedRows[i];
            var actual = rules[i];

            var assertions = new Dictionary<string, Action<SelectionRuleTableRow, Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetList.GetSelectionRulesResponse.SelectionRuleItem>>(StringComparer.OrdinalIgnoreCase)
            {
                ["display name"] = (e, a) => a.DisplayName.Should().Be(e.DisplayName),
                ["type"] = (e, a) => a.Type.Should().Be(e.Type)
            };

            dataTable.AssertProvidedProperties<SelectionRuleTableRow, Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetList.GetSelectionRulesResponse.SelectionRuleItem>(expected, actual, assertions);
        }
    }

    [Then(@"the element should have no statistic rules")]
    public async Task ThenTheElementShouldHaveNoStatisticRulesAsync()
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetStatisticRules(elementId);
        rules.Should().BeEmpty();
    }

    [Then(@"the element should have no include rules")]
    public async Task ThenTheElementShouldHaveNoIncludeRulesAsync()
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetIncludeRules(elementId);
        rules.Should().BeEmpty();
    }

    [Then(@"the element should have no selection rules")]
    public async Task ThenTheElementShouldHaveNoSelectionRulesAsync()
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetSelectionRules(elementId);
        rules.Should().BeEmpty();
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

    [When(@"the content creator updates the statistic rule with the name ""([^""]*)"" to have the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheStatisticRuleWithTheNameToHaveTheFollowingPropertiesAsync(string statisticName, DataTable dataTable)
    {
        var elementId = _host.Get<Guid>("last-created-element-id");
        var rules = await _elementsDriver.GetStatisticRules(elementId);
        var rule = rules.SingleOrDefault(r => r.Name.Equals(statisticName, StringComparison.OrdinalIgnoreCase));

        rule.Should().NotBeNull("Expected to find a statistic rule with the name '{0}', but none was found.", statisticName);

        var current = await _elementsDriver.GetStatisticRuleById(elementId, rule.RuleId);
        var updates = dataTable.CreateInstance<StatisticRuleTableRow>(_scenarioContext);

        var properties = new ManageElementsDriver.UpdateStatisticRuleProperties
        {
            Name = string.IsNullOrWhiteSpace(updates.Name) ? current.Name : updates.Name,
            Value = string.IsNullOrWhiteSpace(updates.Value) ? current.Value : updates.Value,
            StackingBonus = updates.StackingBonus ?? current.StackingBonus,
            LevelRequirement = updates.LevelRequirement ?? current.LevelRequirement,
            DisplayName = updates.DisplayName ?? current.DisplayName,
            Minimum = updates.Minimum ?? current.Minimum,
            Maximum = updates.Maximum ?? current.Maximum,
            RequirementsExpression = updates.RequirementsExpression ?? current.Requirements
        };

        await _elementsDriver.UpdateStatisticRule(elementId, rule.RuleId, properties);
    }


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
        public string? RequirementsExpression { get; set; }
    }



    [When(@"the content creator adds a new include rule to the element with the following properties")]
    public async Task WhenTheContentCreatorAddsANewIncludeRuleToTheElementWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var row = dataTable.CreateInstance<IncludeRuleTableRow>(_scenarioContext);

        var elementId = _host.Get<Guid>("last-created-element-id");

        var includedElementId = await _elementsDriver.CreateElement(new ManageElementsDriver.CreateProperties
        {
            Name = row.IncludedElement,
            Type = "Type",
            Description = $"Description for {row.IncludedElement}"
        }, storeAsLastCreated: false);

        var properties = new ManageElementsDriver.CreateIncludeRuleProperties
        {
            IncludedElementId = includedElementId,
            LevelRequirement = row.LevelRequirement ?? 0,
            RequirementsExpression = row.RequirementsExpression,
            DisplayName = row.DisplayName
        };

        await _elementsDriver.CreateIncludeRule(elementId, properties);
        _host.Set(row.IncludedElement, "last-created-include-rule-name");
    }

    [When(@"the content creator adds a new include rule to the ""([^""]*)"" element with the following properties")]
    public async Task WhenTheContentCreatorAddsANewIncludeRuleToTheElementWithTheFollowingPropertiesAsync(string elementName, DataTable dataTable)
    {
        var row = dataTable.CreateInstance<IncludeRuleTableRow>(_scenarioContext);

        var element = await _elementsDriver.GetElementByName(elementName);

        var elementToInclude = await _elementsDriver.GetElementByName(row.IncludedElement);

        var properties = new ManageElementsDriver.CreateIncludeRuleProperties
        {
            IncludedElementId = elementToInclude.Id,
            LevelRequirement = row.LevelRequirement ?? 0,
            RequirementsExpression = row.RequirementsExpression,
            DisplayName = row.DisplayName
        };

        await _elementsDriver.CreateIncludeRule(element.Id, properties);
        _host.Set(row.IncludedElement, "last-created-include-rule-name");
    }

    [Then(@"the element should have an include rule with the following properties")]
    public async Task ThenTheElementShouldHaveAnIncludeRuleWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        var expected = dataTable.CreateInstance<IncludeRuleTableRow>(_scenarioContext);

        var elementId = _host.Get<Guid>("last-created-element-id");
        var createdRule = _host.Get<CreateIncludeRuleResponse>("last-created-include-rule");

        var rule = await _elementsDriver.GetIncludeRuleById(elementId, createdRule.RuleId);

        var assertions = new Dictionary<string, Action<IncludeRuleTableRow, GetIncludeRuleResponse>>(StringComparer.OrdinalIgnoreCase)
        {
            ["included element"] = (e, a) => a.IncludedElementId.Should().NotBeEmpty(),
            ["level requirement"] = (e, a) => a.LevelRequirement.Should().Be(e.LevelRequirement ?? 0),
            ["requirements"] = (e, a) => a.Requirements.Should().Be(e.RequirementsExpression),
            ["display name"] = (e, a) => a.DisplayName.Should().Be(e.DisplayName)
        };

        dataTable.AssertProvidedProperties(expected, rule, assertions);

        var list = await _elementsDriver.GetIncludeRules(elementId);
        list.Should().Contain(r => r.RuleId == createdRule.RuleId);
    }

    private sealed record IncludeRuleTableRow : IMarkdownDescriptionTableRow
    {
        public string? Name { get; set; }
        public required string IncludedElement { get; set; }
        public int? LevelRequirement { get; set; }
        public string? RequirementsExpression { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
    }



    private sealed record SelectionRuleTableRow : IMarkdownDescriptionTableRow
    {
        public required string DisplayName { get; set; }
        public required string Type { get; set; }
        public string? Supports { get; set; }
        public string? Range { get; set; }
        public int? Quantity { get; set; }
        public bool? Optional { get; set; }
        public int? LevelRequirement { get; set; }
        public string? Requirements { get; set; }
        public string? Default { get; set; }
        public string? Description { get; set; }
    }

    [When(@"the content creator adds a new selection rule to the ""([^""]*)"" element with the following properties")]
    public async Task WhenTheContentCreatorAddsANewSelectionRuleToTheElementWithTheFollowingPropertiesAsync(string elementName, DataTable dataTable)
    {
        var row = dataTable.CreateInstance<SelectionRuleTableRow>(_scenarioContext);

        if (!string.IsNullOrWhiteSpace(row.Default))
        {
            throw new PendingStepException("Selection rule 'default' is specified in the feature, but the current API/domain does not support a default selection yet.");
        }

        ElementDataModel element;
        try
        {
            element = await _elementsDriver.GetElementByName(elementName);
        }
        catch (KeyNotFoundException)
        {
            var matches = await _elementsDriver.GetElements();
            element = matches.Single(e => e.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase));
        }

        var properties = new ManageElementsDriver.CreateSelectionRuleProperties
        {
            DisplayName = row.DisplayName,
            Type = row.Type,
            Supports = row.Supports,
            Range = row.Range,
            Quantity = row.Quantity ?? 1,
            Optional = row.Optional ?? false,
            LevelRequirement = row.LevelRequirement ?? 0,
            Requirements = row.Requirements,
            Default = row.Default
        };

        await _elementsDriver.CreateSelectionRule(element.Id, properties);
        _host.Set(element.Id, "last-selection-rule-element-id");
    }

    [Then(@"the element ""([^""]*)"" should have a selection rule with the following properties")]
    public async Task ThenTheElementShouldHaveASelectionRuleWithTheFollowingPropertiesAsync(string elementName, DataTable dataTable)
    {
        var expected = dataTable.CreateInstance<SelectionRuleTableRow>(_scenarioContext);

        if (!string.IsNullOrWhiteSpace(expected.Default))
        {
            throw new PendingStepException("Selection rule 'default' is specified in the feature, but the current API/domain does not support a default selection yet.");
        }

        ElementDataModel element;
        try
        {
            element = await _elementsDriver.GetElementByName(elementName);
        }
        catch (KeyNotFoundException)
        {
            var matches = await _elementsDriver.GetElements();
            element = matches.Single(e => e.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase));
        }

        var createdRule = _host.Get<Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create.CreateSelectionRuleResponse>("last-created-selection-rule");
        var rule = await _elementsDriver.GetSelectionRuleById(element.Id, createdRule.RuleId.Value);

        var assertions = new Dictionary<string, Action<SelectionRuleTableRow, GetSelectionRuleResponse>>(StringComparer.OrdinalIgnoreCase)
        {
            ["display name"] = (e, a) => a.DisplayName.Should().Be(e.DisplayName),
            ["type"] = (e, a) => a.Type.Should().Be(e.Type),
            ["supports"] = (e, a) => (a.Supports ?? string.Empty).Should().Be(e.Supports ?? string.Empty),
            ["range"] = (e, a) => (a.Range ?? string.Empty).Should().Be(e.Range ?? string.Empty),
            ["quantity"] = (e, a) => a.Quantity.Should().Be(e.Quantity ?? 1),
            ["optional"] = (e, a) => a.Optional.Should().Be(e.Optional ?? false),
            ["level requirement"] = (e, a) => a.LevelRequirement.Should().Be(e.LevelRequirement ?? 0),
            ["requirements"] = (e, a) => (a.Requirements ?? string.Empty).Should().Be(e.Requirements ?? string.Empty)
        };

        dataTable.AssertProvidedProperties(expected, rule, assertions);

        var rules = await _elementsDriver.GetSelectionRules(element.Id);
        rules.Should().Contain(r => r.RuleId == createdRule.RuleId.Value);
    }
}
