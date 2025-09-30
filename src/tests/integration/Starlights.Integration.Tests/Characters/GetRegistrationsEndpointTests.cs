using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;
using Starlights.Integration.Tests.Constants;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class GetRegistrationsEndpointTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public GetRegistrationsEndpointTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();
        await client.InitializeElementsAsync(TestCancellationToken);

        // Create a character we can work with (refactored to helper)
        var characterId = await client.CreateDefaultCharacterAsync(TestCancellationToken);
        _integration.SetCharacterIdentifier(characterId);

        // wait for at least one selection rule (ensures some registrations exist)
        await _eventListener.RegistrationSelectionRuleCreated.WaitForEvent(count: 1, cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetRegistrations_Returns_Hierarchy()
    {
        // Arrange
        var client = _integration.CreateClient();
        var characterId = _integration.GetCharacterIdentifier();

        // Act
        var response = await client.GetRegistrationsAsync(characterId, TestCancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.Registrations.Should().NotBeNull();
        response.Registrations.Should().NotBeEmpty();
        response.Registrations.Should().OnlyContain(r => r.CharacterId == characterId);
        response.Registrations.Select(r => r.RegistrationId).Should().OnlyHaveUniqueItems();
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetRegistrations_UnknownCharacter_Returns_NotFound()
    {
        // Arrange
        var client = _integration.CreateClient();
        var unknownCharacter = Guid.NewGuid();

        // Act
        var http = await client.GetAsync($"/api/characters/{unknownCharacter}/registrations", TestCancellationToken);

        // Assert
        http.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
