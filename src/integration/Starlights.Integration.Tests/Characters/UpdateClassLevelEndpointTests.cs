using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class UpdateClassLevelEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public UpdateClassLevelEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();
        await client.InitializeElementsAsync(TestCancellationToken);

        // Create a character
        var creationOptions = await client.GetCharacterCreationOptionsAsync(TestCancellationToken);
        creationOptions.Options.Should().NotBeEmpty();
        var portraitOptions = await client.GetCharacterPortraitOptionsAsync(TestCancellationToken);
        portraitOptions.Portraits.Should().NotBeEmpty();

        var character = await client.CreateCharacterAsync(creationOptions.Options[0].Id, $"LvlUp {Guid.NewGuid()}", portraitOptions.Portraits[0].Url, TestCancellationToken);
        _integration.SetCharacterIdentifier(character.Id);

        // Wait for initial class creation event (likely none until registration)
        await _eventListener.RegistrationSelectionRuleCreated.WaitForEvent(predicate: e => e.CharacterId == character.Id, count: 1, cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task UpdateClassLevel_ToLevel3()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();

        // Get selection rules for Class and pick first option (Barbarian expected in seed data)
        var rules = await client.GetSelectionRulesAsync(characterId, ["Class"], TestCancellationToken);
        rules.Rules.Should().NotBeEmpty();
        var classRule = rules.Rules[0];
        var options = await client.GetSelectionRuleOptionsAsync(characterId, classRule.RegistrationSelectionRuleId, TestCancellationToken);
        options.Options.Should().NotBeEmpty();
        var barbarianOption = options.Options.First(o => o.Name.Contains("Barbarian"));

        // Register the Barbarian class
        var registrationId = await client.RegisterSelectionRuleAsync(characterId, classRule.RegistrationId, classRule.RegistrationSelectionRuleId, barbarianOption.ElementId, ct: TestCancellationToken);

        // Wait for class created event
        await _eventListener.CharacterClassCreated.WaitForEvent(predicate: e => e.ClassId != Guid.Empty, count: 1, cancellationToken: TestCancellationToken);

        // Retrieve classes via new endpoint
        var classesResponse = await client.GetCharacterClassesAsync(characterId, TestCancellationToken);
        classesResponse.Classes.Should().NotBeEmpty();
        var barbarianClass = classesResponse.Classes.First(c => c.RegistrationId == registrationId);
        barbarianClass.Level.Should().Be(1);

        // Act - level up to 3
        await client.UpdateClassLevelAsync(characterId, barbarianClass.CharacterClassId, 3, ct: TestCancellationToken);

        await _eventListener.CharacterLevelChanged
            .WaitForEvent(predicate: e => e.NewLevel == 3, cancellationToken: TestCancellationToken);

        // Assert - fetch classes again
        var updatedClasses = await client.GetCharacterClassesAsync(characterId, TestCancellationToken);
        var updatedBarbarian = updatedClasses.Classes.First(c => c.CharacterClassId == barbarianClass.CharacterClassId);
        updatedBarbarian.Level.Should().Be(3);

        // Also verify character details reflect new level
        var details = await client.GetCharacterDetailsAsync(characterId, TestCancellationToken);
        details.Character.Level.Should().Be(3);
    }
}
