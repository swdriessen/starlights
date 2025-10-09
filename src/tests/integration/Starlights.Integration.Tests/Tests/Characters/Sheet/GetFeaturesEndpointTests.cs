using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Sheet;

[TestClass]
public sealed class GetFeaturesEndpointTests : IntegrationTestBase
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

        _characterCreationDriver = _integration.GetDriver<CharacterCreationDriver>();
        _characterManagementDriver = _integration.GetDriver<CharacterManagementDriver>();
        _registrationDriver = _integration.GetDriver<RegistrationDriver>();

        await _integration.InitializeElements();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetFeatures_Returns_Class_And_ClassFeatures()
    {
        // Arrange
        await _characterCreationDriver.CreateCharacterAsync();
        await _registrationDriver.RegisterClass("Barbarian");

        // Act
        var features = await _characterManagementDriver.GetFeatures();

        // Assert
        features.Should().NotBeEmpty();
        features.Any(f => f.Type == "Class").Should().BeTrue();
        features.Any(f => f.Type == "Class Feature").Should().BeTrue();
    }
}
