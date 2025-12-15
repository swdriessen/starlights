using AwesomeAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class CharacterCreationOptionsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationOptionsDriver _creationOptionsDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _creationOptionsDriver = _integration.GetDriver<CharacterCreationOptionsDriver>();

        await _integration.InitializeElements();
    }

    [TestMethod]
    public async Task GetCharacterCreationOptions()
    {
        // Act
        var creationOptions = await _creationOptionsDriver.GetCharacterCreationOptionsAsync();

        // Assert
        creationOptions.Should().NotBeEmpty();
    }
}
