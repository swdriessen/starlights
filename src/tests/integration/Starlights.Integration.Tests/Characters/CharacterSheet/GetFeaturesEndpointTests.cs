using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.CharacterSheet.GetFeatures;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;

namespace Starlights.Integration.Tests.Characters.CharacterSheet;

[TestClass]
public sealed class GetFeaturesEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public GetFeaturesEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();

        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.InitializeElementsAsync();

        // Get creation options
        var optionsResponse = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        optionsResponse.Options.Should().NotBeEmpty();

        // Get portrait options
        var portraitsResponse = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);
        portraitsResponse.Portraits.Should().NotBeEmpty();

        var characterName = $"Integration Test Character {Guid.NewGuid()}";

        var characterResponse = await client.CreateCharacterAsync(optionsResponse.Options[0].Id, characterName, portraitsResponse.Portraits[0].Url, TestCancellationToken);
        _integration.SetCharacterIdentifier(characterResponse.Id);

        // Wait for ability scores to complete (ensures root registration processed)
        await _eventListener.AbilityScoreCreated.WaitForEvent(count: 6, cancellationToken: TestCancellationToken);

        // Use helper extension to get selection rules filtered by Class type
        var selectionRules = await client.GetSelectionRulesAsync(characterResponse.Id, ["Class"], TestCancellationToken);
        selectionRules.Rules.Should().NotBeEmpty();
        var classRule = selectionRules.Rules.First(r => r.Type == "Class");

        // Use helper to get options for the selection rule
        var classOptions = await client.GetSelectionRuleOptionsAsync(characterResponse.Id, classRule.RegistrationSelectionRuleId, TestCancellationToken);
        classOptions.Options.Should().NotBeEmpty();
        var firstClass = classOptions.Options.First();

        // Register the selected class using generic Post helper
        var registerRequest = new RegisterSelectionRuleRequest
        {
            ParentRegistration = classRule.RegistrationId,
            ElementId = firstClass.ElementId
        };

        var registerResponse = await client.PostJsonAndReadAsync<RegisterSelectionRuleResponse>($"/api/characters/{characterResponse.Id}/builder/selection-rules/{classRule.RegistrationSelectionRuleId}/register", registerRequest, HttpStatusCode.OK, TestCancellationToken);
        registerResponse.RegistrationId.Should().NotBe(Guid.Empty);

        // Wait for at least one class feature registration to appear
        await _eventListener.RegistrationCreated
            .WaitForEvent(x => x.AssociatedElementType == "Class Feature", cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetFeatures_Returns_Class_And_ClassFeatures()
    {
        // Arrange
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();

        // Act
        var httpResponse = await client.GetAsync($"/api/characters/{characterId}/features", TestCancellationToken);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadFromJsonAsync<GetFeaturesResponse>(TestCancellationToken);
        response.Should().NotBeNull();
        response!.Features.Should().NotBeEmpty();

        // Should contain at least one class and some class features (from initializer: Barbarian / Rogue)
        response.Features.Any(f => f.Type == "Class").Should().BeTrue();
        response.Features.Any(f => f.Type == "Class Feature").Should().BeTrue();
    }
}
