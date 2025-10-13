using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Eventing;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Modules.Characters.Domain.Progression.Eventing;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class UpdateClassLevelEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private EventObserverCollection _events = default!;
    private RegistrationDriver _registrationDriver = default!;
    private CharacterManagementDriver _characterManagementDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _events = _integration.GetEventObserverCollection();
        _registrationDriver = _integration.GetDriver<RegistrationDriver>();
        _characterManagementDriver = _integration.GetDriver<CharacterManagementDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task LevelUpRegisteredClass()
    {
        // Arrange
        const int newLevel = 5;
        await _registrationDriver.RegisterClass("Barbarian");

        // Act
        var barbarian = await _characterManagementDriver.GetClassByName("Barbarian");
        await _characterManagementDriver.LevelUp(barbarian.CharacterClassId, newLevel);

        // Assert
        var updatedBarbarian = await _characterManagementDriver.GetClassByName("Barbarian");
        updatedBarbarian.Level.Should().Be(newLevel);
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task LevelDownRegisteredClass()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");
        await _characterManagementDriver.LevelUp("Barbarian", 3);


        // Act
        await _characterManagementDriver.LevelUp("Barbarian", 2);

        // Assert
        var updatedBarbarian = await _characterManagementDriver.GetClassByName("Barbarian");
        updatedBarbarian.Level.Should().Be(2);
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task LevelDownRegisteredClass2()
    {
        // Arrange
        await _registrationDriver.RegisterClass("Barbarian");
        await _characterManagementDriver.LevelUp("Barbarian", 3);
        await _events.EnsureObservation<CharacterLevelChangedEvent>();


        // Act
        await _characterManagementDriver.LevelUp("Barbarian", 2);
        await _events.EnsureObservation<CharacterLevelChangedEvent>();

        // Assert
        var updatedBarbarian = await _characterManagementDriver.GetClassByName("Barbarian");
        updatedBarbarian.Level.Should().Be(2);
    }
}
