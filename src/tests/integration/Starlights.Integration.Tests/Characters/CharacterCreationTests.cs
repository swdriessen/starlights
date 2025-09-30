using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class CharacterCreationTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;

    public CharacterCreationTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();
        await client.InitializeElementsAsync(TestCancellationToken);
    }

    [TestMethod]
    public async Task CreateCharacterFromCharacterCreationElement()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act - creation via helper
        var createdCharacterId = await client.CreateDefaultCharacterAsync(TestCancellationToken);

        // Assert
        createdCharacterId.Should().NotBe(Guid.Empty);
    }
}
