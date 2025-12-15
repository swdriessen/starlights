using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Registration;

[TestClass]
public sealed class GetRegistrationsEndpointTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private RegistrationDriver _driver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _driver = _integration.GetDriver<RegistrationDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>().CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetRegistrations_Returns_Hierarchy()
    {
        // Act
        var registrations = await _driver.GetRegistrations();

        // Assert
        registrations.Should().NotBeEmpty();
    }
}
