using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;
using Starlights.Integration.Tests.Constants;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class RegisterSelectionRuleEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public RegisterSelectionRuleEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();
        await client.InitializeElementsAsync(TestCancellationToken);

        // Create a character we can work with (refactored to helper)
        var characterId = await client.CreateDefaultCharacterAsync(TestCancellationToken);
        _integration.SetCharacterIdentifier(characterId);

        // wait for character initialization
        await _eventListener.RegistrationSelectionRuleCreated.WaitForEvent(count: 3, cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetSelectionRules_Returns_Rules()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();

        // Act
        var rules = await client.GetSelectionRulesAsync(characterId, [SelectionRuleTypes.Class, SelectionRuleTypes.Species, SelectionRuleTypes.Background], TestCancellationToken);

        // Assert (explicit - no opaque helper methods)
        rules.Should().NotBeNull();
        rules.Rules.Should().NotBeEmpty();
        rules.Rules.Select(r => r.Type)
            .Should().OnlyContain(t => t == SelectionRuleTypes.Class || t == SelectionRuleTypes.Species || t == SelectionRuleTypes.Background);
        rules.Rules.Should().OnlyContain(r => r.RegistrationId != Guid.Empty
                                              && r.RegistrationSelectionRuleId != Guid.Empty
                                              && !string.IsNullOrWhiteSpace(r.Name));
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetSelectionRuleOptions_Returns_Options()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();
        var rules = await client.GetSelectionRulesAsync(characterId, [SelectionRuleTypes.Class], TestCancellationToken);
        rules.Rules.Should().NotBeEmpty();
        var targetRuleId = rules.Rules[0].RegistrationSelectionRuleId;

        // Act
        var options = await client.GetSelectionRuleOptionsAsync(characterId, targetRuleId, TestCancellationToken);

        // Assert (explicit)
        options.Options.Should().NotBeEmpty();
        options.Options.Should().OnlyContain(o => o.ElementId != Guid.Empty && !string.IsNullOrWhiteSpace(o.Name));
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task RegisterSelectionRule_UnknownCharacter_Returns_NotFound()
    {
        // Arrange
        var client = _integration.CreateClient();
        var unknownCharacter = Guid.NewGuid();
        var unknownParent = Guid.NewGuid();
        var unknownRule = Guid.NewGuid();
        var unknownElement = Guid.NewGuid();
        var url = $"/api/characters/{unknownCharacter}/builder/selection-rules/{unknownRule}/register";
        var payload = new { characterId = unknownCharacter, parentRegistration = unknownParent, elementId = unknownElement, selectionRuleId = unknownRule };

        // Act
        var response = await client.PostAsJsonAsync(url, payload, TestCancellationToken);

        // Assert
        await response.ShouldHaveStatusAsync(HttpStatusCode.NotFound);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task RegisterSelectionRule_UnknownSelectionRuleOnParent_Returns_NotFound()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();

        // pick any existing registration for this character to use as parent
        var rules = await client.GetSelectionRulesAsync(characterId, [SelectionRuleTypes.Class, SelectionRuleTypes.Species, SelectionRuleTypes.Background], TestCancellationToken);
        rules.Rules.Should().NotBeEmpty();
        var parentId = rules.Rules[0].RegistrationId;

        var unknownRuleId = Guid.NewGuid();
        var unknownElementId = Guid.NewGuid(); // won't be reached if rule not found
        var url = $"/api/characters/{characterId}/builder/selection-rules/{unknownRuleId}/register";
        var payload = new { characterId, parentRegistration = parentId, elementId = unknownElementId, selectionRuleId = unknownRuleId };

        // Act
        var response = await client.PostAsJsonAsync(url, payload, TestCancellationToken);

        // Assert
        await response.ShouldHaveStatusAsync(HttpStatusCode.NotFound);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task RegisterSelectionRule_Adds_NewRegistration_For_ClassElement()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();

        // Use the endpoint to get the selection rule to register against
        var rules = await client.GetSelectionRulesAsync(characterId, [SelectionRuleTypes.Class], TestCancellationToken);
        rules.Rules.Should().NotBeEmpty();
        var targetRule = rules.Rules[0];

        // Get selection options
        var options = await client.GetSelectionRuleOptionsAsync(characterId, targetRule.RegistrationSelectionRuleId, TestCancellationToken);
        options.Options.Should().NotBeEmpty();
        var chosenOption = options.Options[0];

        // Act
        var newRegistrationId = await client.RegisterSelectionRuleAsync(characterId, targetRule.RegistrationId, targetRule.RegistrationSelectionRuleId, chosenOption.ElementId, ct: TestCancellationToken);

        // Assert
        newRegistrationId.Should().NotBe(Guid.Empty);
    }
}
