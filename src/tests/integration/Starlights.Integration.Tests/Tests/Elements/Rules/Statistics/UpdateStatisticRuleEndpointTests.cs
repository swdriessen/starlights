using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

namespace Starlights.Integration.Tests.Elements.Rules.Statistics;

[TestClass]
public sealed class UpdateStatisticRuleEndpointTests : IntegrationTestBase
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
    public async Task UpdateStatisticRule_WhenRuleExists_ShouldReturnOk_AndUpdateAllProperties()
    {
        // Arrange
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

        var created = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            StackingBonus = "armor",
            LevelRequirement = 0,
            DisplayName = "Armor Class",
            Minimum = 0,
            Maximum = 30,
            RequirementsExpression = "level >= 1"
        });

        var updateRequest = new UpdateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+2",
            StackingBonus = null,
            LevelRequirement = 2,
            DisplayName = "Armor Class",
            Minimum = 1,
            Maximum = 25,
            RequirementsExpression = null
        };

        // Act
        var (response, status) = await rulesDriver.UpdateStatisticRuleAsync(elementId, created.RuleId, updateRequest);

        // Assert
        status.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(created.RuleId);

        var updated = await rulesDriver.GetStatisticRuleByIdAsync(elementId, created.RuleId);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("ac");
        updated.Value.Should().Be("2");
        updated.StackingBonus.Should().BeNull();
        updated.LevelRequirement.Should().Be(2);
        updated.DisplayName.Should().Be("Armor Class");
        updated.Minimum.Should().Be(1);
        updated.Maximum.Should().Be(25);
        updated.Requirements.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateStatisticRule_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();
        var elementId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();

        // Act
        var (response, status) = await rulesDriver.UpdateStatisticRuleAsync(elementId, ruleId, new UpdateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            StackingBonus = null,
            LevelRequirement = 0
        });

        // Assert
        status.Should().Be(HttpStatusCode.NotFound);
        response.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateStatisticRule_WhenRuleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
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

        // Act
        var (response, status) = await rulesDriver.UpdateStatisticRuleAsync(elementId, Guid.NewGuid(), new UpdateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            StackingBonus = null,
            LevelRequirement = 0
        });

        // Assert
        status.Should().Be(HttpStatusCode.NotFound);
        response.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateStatisticRule_WhenNameIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
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

        var created = await rulesDriver.CreateStatisticRuleAsync(elementId, new CreateStatisticRuleRequest
        {
            Name = "AC",
            Value = "+1",
            LevelRequirement = 0
        });

        // Act
        var (response, status) = await rulesDriver.UpdateStatisticRuleAsync(elementId, created.RuleId, new UpdateStatisticRuleRequest
        {
            Name = string.Empty,
            Value = "+1",
            LevelRequirement = 0
        });

        // Assert
        status.Should().Be(HttpStatusCode.BadRequest);
        response.Should().BeNull();
    }
}
