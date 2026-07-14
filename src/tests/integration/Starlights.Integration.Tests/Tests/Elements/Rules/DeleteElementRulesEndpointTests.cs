using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetById;
using Starlights.Integration.Drivers;

namespace Starlights.Integration.Tests.Elements.Rules;

[TestClass]
public sealed class DeleteElementRulesEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Setup()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _integration.Dispose();
    }

    [TestMethod]
    public async Task DeleteRules_WhenAllRuleIdsExist_ShouldReturnNoContent_AndRemoveAllRules()
    {
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Dummy",
            level: 0,
            school: "Abjuration",
            time: "1 action",
            range: "Self",
            duration: "Instant",
            isConcentration: false,
            isRitual: false,
            hasSomatic: false,
            hasVerbal: false,
            hasMaterial: false,
            materialComponent: null,
            description: "dummy");

        var includedElementId = await elementsDriver.CreateSpellAsync(
            name: "Included",
            level: 0,
            school: "Abjuration",
            time: "1 action",
            range: "Self",
            duration: "Instant",
            isConcentration: false,
            isRitual: false,
            hasSomatic: false,
            hasVerbal: false,
            hasMaterial: false,
            materialComponent: null,
            description: "dummy");

        var stat = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            StackingBonus = null,
            LevelRequirement = 0
        });

        var include = await rulesDriver.CreateIncludeRuleAsync(elementId, new CreateIncludeRuleRequest(
            ElementId: elementId,
            IncludedElementId: includedElementId,
            LevelRequirement: 0,
            RequirementsExpression: null,
            DisplayName: null));

        var selection = await rulesDriver.CreateSelectionRuleAsync(elementId, new CreateSelectionRuleRequest(
            ElementId: elementId,
            DisplayName: "Language",
            Type: "Language",
            Supports: null,
            Range: null,
            Quantity: 1,
            Optional: false,
            LevelRequirement: 0,
            Requirements: null));

        (bool IsSuccessStatusCode, HttpStatusCode StatusCode) result = await rulesDriver.DeleteRulesAsync(
            elementId,
            new[] { stat.RuleId, include.RuleId, selection.RuleId });

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.IsSuccessStatusCode.Should().BeTrue();

        (await rulesDriver.GetStatisticRuleByIdAsync(elementId, stat.RuleId)).Should().BeNull();
        (await rulesDriver.GetIncludeRuleByIdAsync(elementId, include.RuleId)).Should().BeNull();

        var selectionGet = await rulesDriver.GetSelectionRuleByIdAsync(elementId, selection.RuleId);
        selectionGet.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteRules_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        (bool IsSuccessStatusCode, HttpStatusCode StatusCode) missingElement = await rulesDriver.DeleteRulesAsync(
            Guid.NewGuid(),
            new[] { Guid.NewGuid() });

        missingElement.StatusCode.Should().Be(HttpStatusCode.NotFound);
        missingElement.IsSuccessStatusCode.Should().BeFalse();
    }

    [TestMethod]
    public async Task DeleteRules_WhenAnyRuleIdDoesNotExist_ShouldReturnNotFound()
    {
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Dummy",
            level: 0,
            school: "Abjuration",
            time: "1 action",
            range: "Self",
            duration: "Instant",
            isConcentration: false,
            isRitual: false,
            hasSomatic: false,
            hasVerbal: false,
            hasMaterial: false,
            materialComponent: null,
            description: "dummy");

        var stat = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            StackingBonus = null,
            LevelRequirement = 0
        });

        (bool IsSuccessStatusCode, HttpStatusCode StatusCode) missingRule = await rulesDriver.DeleteRulesAsync(
            elementId,
            new[] { stat.RuleId, Guid.NewGuid() });

        missingRule.StatusCode.Should().Be(HttpStatusCode.NotFound);
        missingRule.IsSuccessStatusCode.Should().BeFalse();

        (await rulesDriver.GetStatisticRuleByIdAsync(elementId, stat.RuleId)).Should().NotBeNull();
    }

    [TestMethod]
    public async Task DeleteRules_WhenRuleIdsIsEmpty_ShouldReturnBadRequest()
    {
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Dummy",
            level: 0,
            school: "Abjuration",
            time: "1 action",
            range: "Self",
            duration: "Instant",
            isConcentration: false,
            isRitual: false,
            hasSomatic: false,
            hasVerbal: false,
            hasMaterial: false,
            materialComponent: null,
            description: "dummy");

        (bool IsSuccessStatusCode, HttpStatusCode StatusCode) validation = await rulesDriver.DeleteRulesAsync(
            elementId,
            Array.Empty<Guid>());

        validation.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        validation.IsSuccessStatusCode.Should().BeFalse();
    }
}
