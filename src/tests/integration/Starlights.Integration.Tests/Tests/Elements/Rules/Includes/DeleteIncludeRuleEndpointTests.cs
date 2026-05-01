using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Integration.Drivers;

namespace Starlights.Integration.Tests.Elements.Rules.Includes;

[TestClass]
public sealed class DeleteIncludeRuleEndpointTests : IntegrationTestBase
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
    public async Task DeleteIncludeRule_WhenRuleExists_ShouldReturnNoContent()
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
            description: "included");

        var created = await rulesDriver.CreateIncludeRuleAsync(elementId, new CreateIncludeRuleRequest(
            ElementId: default,
            IncludedElementId: includedElementId,
            LevelRequirement: 0,
            RequirementsExpression: null,
            DisplayName: null));

        // Act
        var (isSuccess, status) = await rulesDriver.DeleteIncludeRuleAsync(elementId, created.RuleId);

        // Assert
        isSuccess.Should().BeTrue();
        status.Should().Be(HttpStatusCode.NoContent);
    }

    [TestMethod]
    public async Task DeleteIncludeRule_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        // Act
        var (isSuccess, status) = await rulesDriver.DeleteIncludeRuleAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        isSuccess.Should().BeFalse();
        status.Should().Be(HttpStatusCode.NotFound);
    }
}
