using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class UpdateClassLevelEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    private RegistrationDriver _registrationDriver = default!;
    private CharacterManagementDriver _characterManagementDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

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
}
