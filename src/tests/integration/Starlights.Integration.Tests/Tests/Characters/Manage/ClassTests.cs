using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class ClassTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationDriver _characterCreationDriver = default!;
    private CharacterManagementDriver _characterManagementDriver = default!;
    private RegistrationDriver _registrationDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        await _integration.InitializeElements();

        _characterCreationDriver = _integration.GetDriver<CharacterCreationDriver>();
        _characterManagementDriver = _integration.GetDriver<CharacterManagementDriver>();
        _registrationDriver = _integration.GetDriver<RegistrationDriver>();

        await _characterCreationDriver.CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(IntegrationHost.TimeoutForDebugging, CooperativeCancellation = true)]
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
    [Timeout(IntegrationHost.TimeoutForDebugging, CooperativeCancellation = true)]
    public async Task RemoveRegistrationShouldRemoveAllNewRegistrations()
    {
        // Arrange
        var expectedRegistrations = await _registrationDriver.GetRegistrations();
        await _registrationDriver.RegisterClass("Barbarian");

        // Act
        await _registrationDriver.UnregisterClass("Barbarian");

        // Assert
        var actualRegistrations = await _registrationDriver.GetRegistrations();
        actualRegistrations.Should().BeEquivalentTo(expectedRegistrations, "Expected registrations to return to their original state after unregistering the class.");
    }


    [TestMethod]
    [Timeout(IntegrationHost.TimeoutForDebugging, CooperativeCancellation = true)]
    public async Task NewRegistrationNestedSubClass()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");

        // Act
        var selectionRule = await _registrationDriver.GetSingleSelectionRule("SubClass");
        var option = await _registrationDriver.GetSelectionRuleOption(selectionRule.RegistrationSelectionRuleId, "Barbarian SubClass 1");
        await _registrationDriver.RegisterSelectionRule(selectionRule, option);

        // Assert
        var registrations = await _registrationDriver.GetRegistrations();
        registrations.Should().Contain(r => r.Type == "SubClass", "Expected to find a subclass registration after registering a subclass.");
    }

    [TestMethod]
    [Timeout(IntegrationHost.TimeoutForDebugging, CooperativeCancellation = true)]
    public async Task RemoveRegistrationShouldRemoveNestedRegistrations()
    {
        // Arrange
        var expectedRegistrations = await _registrationDriver.GetRegistrations();
        await _registrationDriver.RegisterClass("Barbarian");
        await _registrationDriver.RegisterSubClass("Barbarian SubClass 1");

        // Act
        await _registrationDriver.UnregisterClass("Barbarian");

        // Assert
        var actualRegistrations = await _registrationDriver.GetRegistrations();
        actualRegistrations.Should().BeEquivalentTo(expectedRegistrations, "Expected registrations to return to their original state after unregistering the class.");
        actualRegistrations.Should().NotContain(r => r.Type == "SubClass", "Expected all subclass registrations to be removed after unregistering the class.");
    }
}