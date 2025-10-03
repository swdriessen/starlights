using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class CharacterCreationTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationDriver _characterCreationDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        await _integration.InitializeElements();

        _characterCreationDriver = _integration.GetDriver<CharacterCreationDriver>();
    }

    [TestMethod]
    public async Task CreateDefaultCharacter()
    {
        // Act
        var id = await _characterCreationDriver.CreateCharacterAsync();

        // Assert
        id.Should().NotBe(Guid.Empty);
    }
}
