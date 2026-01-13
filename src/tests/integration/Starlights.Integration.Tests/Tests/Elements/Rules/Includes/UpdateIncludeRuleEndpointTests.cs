using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Integration.Drivers.Elements;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Update;

namespace Starlights.Integration.Tests.Elements.Rules.Includes;

[TestClass]
public sealed class UpdateIncludeRuleEndpointTests : IntegrationTestBase
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
    public async Task UpdateIncludeRule_WhenRuleExists_ShouldReturnOk()
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
            RequirementsExpression: "level >= 1",
            DisplayName: "LegacyId"));

        // Act
        var (response, status) = await rulesDriver.UpdateIncludeRuleAsync(elementId, created.RuleId, new UpdateIncludeRuleRequest
        {
            IncludedElementId = includedElementId,
            LevelRequirement = 2,
            RequirementsExpression = null,
            DisplayName = "Updated"
        });

        // Assert
        status.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(created.RuleId);
    }

    [TestMethod]
    public async Task UpdateIncludeRule_WhenElementDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();
        var elementId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();

        // Act
        var (response, status) = await rulesDriver.UpdateIncludeRuleAsync(elementId, ruleId, new UpdateIncludeRuleRequest
        {
            IncludedElementId = Guid.NewGuid(),
            LevelRequirement = 0
        });

        // Assert
        status.Should().Be(HttpStatusCode.NotFound);
        response.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateIncludeRule_WhenIncludedElementIdIsEmpty_ShouldReturnBadRequest()
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
        var (response, status) = await rulesDriver.UpdateIncludeRuleAsync(elementId, created.RuleId, new UpdateIncludeRuleRequest
        {
            IncludedElementId = Guid.Empty,
            LevelRequirement = 0
        });

        // Assert
        status.Should().Be(HttpStatusCode.BadRequest);
        response.Should().BeNull();
    }
}
