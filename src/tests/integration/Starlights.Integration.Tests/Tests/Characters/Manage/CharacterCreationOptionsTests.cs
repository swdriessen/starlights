using AwesomeAssertions;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Extensions;

namespace Starlights.Integration.Tests.Characters.Manage;

[TestClass]
public sealed class CharacterCreationOptionsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private CharacterCreationOptionsDriver _creationOptionsDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateDefaultBuilder(this)
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
