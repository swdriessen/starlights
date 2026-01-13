using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;

namespace Starlights.Integration.Tests.Elements.Rules.Statistics;

[TestClass]
public sealed class DeleteStatisticRuleEndpointTests : IntegrationTestBase
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
    public async Task DeleteStatisticRule_WhenRuleExists_ShouldReturnNoContent_AndRemoveRule()
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
            StackingBonus = null,
            LevelRequirement = 0
        });

        // Act
        var (_, status) = await rulesDriver.DeleteStatisticRuleAsync(elementId, created.RuleId);

        // Assert
        status.Should().Be(HttpStatusCode.NoContent);

        var deleted = await rulesDriver.GetStatisticRuleByIdAsync(elementId, created.RuleId);
        deleted.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteStatisticRule_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();
        var elementId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();

        // Act
        var (_, status) = await rulesDriver.DeleteStatisticRuleAsync(elementId, ruleId);

        // Assert
        status.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task DeleteStatisticRule_WhenRuleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = Guid.NewGuid();

        // Act
        var (_, status) = await rulesDriver.DeleteStatisticRuleAsync(elementId, Guid.NewGuid());

        // Assert
        status.Should().Be(HttpStatusCode.NotFound);
    }
}
