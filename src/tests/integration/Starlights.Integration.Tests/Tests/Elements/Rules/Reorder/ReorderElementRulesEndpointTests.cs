using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;

namespace Starlights.Integration.Tests.Elements.Rules.Reorder;

[TestClass]
public sealed class ReorderElementRulesEndpointTests : IntegrationTestBase
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
    public async Task ReorderRules_WhenValidStatisticOrder_ShouldReturnNoContent_AndPersistOrder()
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

        var rule1 = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest { Name = "strength", Value = "2", LevelRequirement = 0 });
        var rule2 = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest { Name = "dexterity", Value = "3", LevelRequirement = 0 });
        var rule3 = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest { Name = "health", Value = "5", LevelRequirement = 0 });

        var (IsSuccessStatusCode, StatusCode) = await rulesDriver.ReorderRulesAsync(elementId, [rule3.RuleId, rule1.RuleId, rule2.RuleId]);

        StatusCode.Should().Be(HttpStatusCode.NoContent);

        var list = await rulesDriver.GetStatisticRulesAsync(elementId);
        list.Should().NotBeNull();

        list!.Rules.Select(r => r.RuleId)
            .Should()
            .ContainInOrder(rule3.RuleId, rule1.RuleId, rule2.RuleId);
    }

    [TestMethod]
    public async Task ReorderRules_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = Guid.NewGuid();

        var (IsSuccessStatusCode, StatusCode) = await rulesDriver.ReorderRulesAsync(elementId, [Guid.NewGuid()]);

        IsSuccessStatusCode.Should().BeFalse();
        StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task ReorderRules_WhenRuleIdsAreEmpty_ShouldReturnBadRequest()
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

        var (IsSuccessStatusCode, StatusCode) = await rulesDriver.ReorderRulesAsync(elementId, []);

        IsSuccessStatusCode.Should().BeFalse();
        StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
