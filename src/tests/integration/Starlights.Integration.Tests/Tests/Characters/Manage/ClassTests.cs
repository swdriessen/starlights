using AwesomeAssertions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class ClassTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private EventObserverCollection _events = default!;
    private CharacterCreationDriver _characterCreationDriver = default!;
    private CharacterManagementDriver _characterManagementDriver = default!;
    private RegistrationDriver _registrationDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
            .Build();

        await _integration.InitializeElements();

        _events = _integration.GetEventObserverCollection();
        _characterCreationDriver = _integration.GetDriver<CharacterCreationDriver>();
        _characterManagementDriver = _integration.GetDriver<CharacterManagementDriver>();
        _registrationDriver = _integration.GetDriver<RegistrationDriver>();

        await _characterCreationDriver.CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task NewRegistrationShouldHaveAdditionalRegistrations()
    {
        // Arrange
        var before = await _registrationDriver.GetRegistrations();

        // Act
        await _registrationDriver.RegisterClass("Barbarian");

        // Assert
        var after = await _registrationDriver.GetRegistrations();
        after.Count.Should().BeGreaterThan(before.Count, "Expected more registrations after registering a class.");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task RemoveRegistrationShouldRemoveAllNewRegistrations()
    {
        // Arrange
        var expectedRegistrations = await _registrationDriver.GetRegistrations();
        await _registrationDriver.RegisterClass("Barbarian");

        // Act
        await _registrationDriver.UnregisterClass("Barbarian");

        // Assert
        var actualRegistrations = await _registrationDriver.GetRegistrations();
        actualRegistrations.Count.Should().Be(expectedRegistrations.Count, "Expected the same number of registrations after unregistering the class.");
        actualRegistrations.Should().BeEquivalentTo(expectedRegistrations, "Expected registrations to return to their original state after unregistering the class.");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task NewRegistrationNestedSubClass()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");
        await _characterManagementDriver.LevelUp("Barbarian", 3);
        await _events.EnsureObservation<RegistrationSelectionRuleCreatedEvent>(e => e.ElementType == ElementTypeConstants.SubClass);

        // Act
        var selectionRule = await _registrationDriver.GetSingleSelectionRule("SubClass");
        var option = await _registrationDriver.GetSelectionRuleOption(selectionRule.RegistrationSelectionRuleId, "Barbarian SubClass 1");
        await _registrationDriver.RegisterSelectionRule(selectionRule, option);

        // Assert
        var registrations = await _registrationDriver.GetRegistrations();
        registrations.Should().Contain(r => r.Type == "SubClass", "Expected to find a subclass registration after registering a subclass.");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task RemoveRegistrationShouldRemoveNestedRegistrations()
    {
        // Arrange
        var expectedRegistrations = await _registrationDriver.GetRegistrations();
        await _registrationDriver.RegisterClass("Barbarian");
        await _characterManagementDriver.LevelUp("Barbarian", 3);
        await _events.EnsureObservation<RegistrationSelectionRuleCreatedEvent>(e => e.ElementType == ElementTypeConstants.SubClass);
        await _registrationDriver.RegisterSubClass("Barbarian SubClass 1");

        // Act
        await _registrationDriver.UnregisterClass("Barbarian");

        // Assert
        var actualRegistrations = await _registrationDriver.GetRegistrations();
        actualRegistrations.Count.Should().Be(expectedRegistrations.Count, "Expected the same number of registrations after unregistering the class.");
        actualRegistrations.Should().BeEquivalentTo(expectedRegistrations, "Expected registrations to return to their original state after unregistering the class.");
        actualRegistrations.Should().NotContain(r => r.Type == "SubClass", "Expected all subclass registrations to be removed after unregistering the class.");
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task NewRegistrationShouldNotExposeSubClassSelectionUntilSupported()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");

        // Act + Assert: with current behavior, subclass selection rules are not generated early
        var rules = await _registrationDriver.GetSelectionRules("SubClass");
        rules.Should().BeEmpty("Subclass selection is level-gated and not reprocessed automatically in current implementation.");
    }



    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task LevelDownShouldRemoveRegistrationsNotMeetingLevelRequirements()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");
        await _events.EnsureObservation<RegistrationProcessedEvent>(x => x.ElementType == "Class Feature");
        var expectedRegistrations = await _registrationDriver.GetRegistrations();
        await _characterManagementDriver.LevelUp("Barbarian", 3);
        await _events.EnsureObservation<RegistrationSelectionRuleCreatedEvent>(e => e.ElementType == ElementTypeConstants.SubClass);
        await _registrationDriver.RegisterSubClass("Barbarian SubClass 1");
        _events.ClearInvocations();

        // Act
        await _characterManagementDriver.LevelUp("Barbarian", 1); // Level down to 1
        await _events.EnsureObservation<CharacterLevelChangedEvent>(e => e.NewLevel == 1);

        // Assert
        var actualRegistrations = await _registrationDriver.GetRegistrations();
        actualRegistrations.Count.Should().Be(expectedRegistrations.Count, "Expected the same number of registrations after unregistering the class.");
        actualRegistrations.Should().BeEquivalentTo(expectedRegistrations, "Expected registrations to return to their original state after unregistering the class.");
        actualRegistrations.Should().NotContain(r => r.Type == "SubClass", "Expected all subclass registrations to be removed after unregistering the class.");
    }
}