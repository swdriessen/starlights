using System.Net;
using AwesomeAssertions;
using Starlights.Integration.Drivers;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Update;

namespace Starlights.Integration.Tests.Elements.Rules.Selections;

[TestClass]
public sealed class SelectionRuleCrudEndpointTests : IntegrationTestBase
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
    public async Task SelectionRules_WhenCreated_ShouldBeRetrievableByIdAndList()
    {
        // Arrange
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Rule Owner",
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
            description: "owner");

        // Act
        var created = await rulesDriver.CreateSelectionRuleAsync(elementId, new CreateSelectionRuleRequest(
            ElementId: default,
            DisplayName: "Language Selection",
            Type: "Language",
            Supports: "Any",
            Range: null,
            Quantity: 3,
            Optional: true,
            LevelRequirement: 0,
            Requirements: null));

        var byId = await rulesDriver.GetSelectionRuleByIdAsync(elementId, created.RuleId.Value);
        var list = await rulesDriver.GetSelectionRulesAsync(elementId);

        // Assert
        created.ElementId.Should().Be(elementId);
        byId.Should().NotBeNull();
        byId!.RuleId.Should().Be(created.RuleId.Value);
        byId.DisplayName.Should().Be("Language Selection");
        byId.Type.Should().Be("Language");
        byId.Quantity.Should().Be(3);
        byId.Optional.Should().BeTrue();

        list.Should().NotBeNull();
        list!.Rules.Should().ContainSingle(r => r.RuleId == created.RuleId.Value);
    }

    [TestMethod]
    public async Task UpdateSelectionRule_WhenRuleExists_ShouldReturnOk()
    {
        // Arrange
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Owner",
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
            description: "owner");

        var created = await rulesDriver.CreateSelectionRuleAsync(elementId, new CreateSelectionRuleRequest(
            ElementId: default,
            DisplayName: "Subclass",
            Type: "Subclass",
            Supports: "Barbarian",
            Range: null,
            Quantity: 1,
            Optional: false,
            LevelRequirement: 3,
            Requirements: "Warrior"));

        // Act
        var (response, status) = await rulesDriver.UpdateSelectionRuleAsync(elementId, created.RuleId.Value, new UpdateSelectionRuleRequest(
            DisplayName: "Barbarian Subclass",
            Type: "Subclass",
            Supports: "Barbarian",
            Range: null,
            Quantity: 2,
            Optional: true,
            LevelRequirement: 4,
            Requirements: null));

        // Assert
        status.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(created.RuleId.Value);

        var updated = await rulesDriver.GetSelectionRuleByIdAsync(elementId, created.RuleId.Value);
        updated.Should().NotBeNull();
        updated!.DisplayName.Should().Be("Barbarian Subclass");
        updated.Quantity.Should().Be(2);
        updated.Optional.Should().BeTrue();
        updated.LevelRequirement.Should().Be(4);
        updated.Requirements.Should().BeNull();
    }

    [TestMethod]
    public async Task CreateSelectionRule_WhenQuantityIsZero_ShouldReturnBadRequest()
    {
        // Arrange
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        using var client = _integration.CreateClient();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Owner",
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
            description: "owner");

        // Act
        var response = await client.PostJsonExpectAsync(
            $"/api/elements/{elementId}/rules/selections/create",
            new CreateSelectionRuleRequest(
                ElementId: default,
                DisplayName: "Invalid",
                Type: "Language",
                Supports: null,
                Range: null,
                Quantity: 0,
                Optional: false,
                LevelRequirement: 0,
                Requirements: null),
            expected: HttpStatusCode.BadRequest,
            ct: _integration.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task DeleteSelectionRule_WhenRuleExists_ShouldReturnNoContent()
    {
        // Arrange
        var elementsDriver = _integration.GetDriver<ElementsEndpointDriver>();
        var rulesDriver = _integration.GetDriver<ManageElementRulesEndpointDriver>();

        var elementId = await elementsDriver.CreateSpellAsync(
            name: "Owner",
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
            description: "owner");

        var created = await rulesDriver.CreateSelectionRuleAsync(elementId, new CreateSelectionRuleRequest(
            ElementId: default,
            DisplayName: "Language",
            Type: "Language",
            Supports: null,
            Range: null,
            Quantity: 1,
            Optional: false,
            LevelRequirement: 0,
            Requirements: null));

        // Act
        var (isSuccess, status) = await rulesDriver.DeleteSelectionRuleAsync(elementId, created.RuleId.Value);

        // Assert
        isSuccess.Should().BeTrue();
        status.Should().Be(HttpStatusCode.NoContent);

        var byId = await rulesDriver.GetSelectionRuleByIdAsync(elementId, created.RuleId.Value);
        byId.Should().BeNull();
    }
}
