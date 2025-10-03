using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class CharacterPortraitOptionsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;

    [TestInitialize]
    public void Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();
    }

    [TestMethod]
    public async Task GetCharacterPortraitOptions()
    {
        // Arrange
        var driver = _integration.GetDriver<CharacterPortraitDriver>();

        // Act
        var portraits = await driver.GetCharacterPortraitOptions();

        // Assert
        portraits.Should().NotBeEmpty();
    }
}