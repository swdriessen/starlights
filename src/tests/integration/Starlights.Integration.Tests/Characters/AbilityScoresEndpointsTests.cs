using System.Net;
using FluentAssertions;
using Starlights.Integration.Tests.Core;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Integration.Tests.Core.Extensions;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class AbilityScoresEndpointsTests : IntegrationTestBase
{
    private readonly IntegrationHost _integration;
    private readonly IntegrationEventHandlerListener _eventListener;

    public AbilityScoresEndpointsTests()
    {
        _integration = IntegrationHost.CreateBuilder()
            .Build();

        _eventListener = _integration.GetIntegrationEventHandlerListener();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.InitializeElementsAsync();

        // Create a default character
        var characterId = await client.CreateDefaultCharacterAsync(TestCancellationToken);
        _integration.SetCharacterIdentifier(characterId);

        await _eventListener.AbilityScoreCreated.WaitForEvent(count: 6, cancellationToken: TestCancellationToken);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task GetAbilities_Returns_DetailedScores()
    {
        // Arrange
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();
        const int expectedScore = 10;
        const int expectedModifier = 0;

        // Act
        var abilities = await client.GetAbilityScoresAsync(characterId, TestCancellationToken);

        // Assert
        abilities.Should().NotBeNull();
        abilities.AbilityScores.Should().NotBeEmpty();

        var first = abilities.AbilityScores[0];
        first.AbilityScoreId.Should().NotBe(Guid.Empty);
        first.Name.Should().NotBeNullOrWhiteSpace();
        first.Abbreviation.Should().NotBeNullOrWhiteSpace();
        first.CalculatedScore.Should().Be(expectedScore);
        first.CalculatedModifier.Should().Be(expectedModifier);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task UpdateBaseScore_Updates_CalculatedFields()
    {
        // Arrange
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();
        var abilitiesBefore = await client.GetAbilityScoresAsync(characterId, TestContext.CancellationTokenSource.Token);
        var target = abilitiesBefore.AbilityScores[0];
        const int newBaseScore = 18;
        const int expectedScore = 18;
        const int expectedModifier = 4;

        // Act
        var postResponse = await client.SetAbilityBaseScoreAsync(characterId, target.AbilityScoreId, newBaseScore, TestContext.CancellationTokenSource.Token);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await client.GetAbilityScoresAsync(characterId, TestContext.CancellationTokenSource.Token);
        var updated = abilitiesAfter.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.BaseScore.Should().Be(newBaseScore);
        updated.CalculatedScore.Should().Be(expectedScore);
        updated.CalculatedModifier.Should().Be(expectedModifier);
    }

    [TestMethod]
    [Timeout(IntegrationHost.Timeout, CooperativeCancellation = true)]
    public async Task UpdateAdditionalScore_Updates_CalculatedFields()
    {
        // Arrange
        var characterId = _integration.GetCharacterIdentifier();
        var client = _integration.CreateClient();
        var abilitiesBefore = await client.GetAbilityScoresAsync(characterId, TestContext.CancellationTokenSource.Token);
        var target = abilitiesBefore.AbilityScores[0];
        const int newAdditionalScore = 2;
        const int expectedScore = 12;
        const int expectedModifier = 1;

        // Act
        var postResponse = await client.SetAbilityAdditionalScoreAsync(characterId, target.AbilityScoreId, newAdditionalScore, TestContext.CancellationTokenSource.Token);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await client.GetAbilityScoresAsync(characterId, TestContext.CancellationTokenSource.Token);
        var updated = abilitiesAfter.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.AdditionalScore.Should().Be(newAdditionalScore);
        updated.CalculatedScore.Should().Be(expectedScore);
        updated.CalculatedModifier.Should().Be(expectedModifier);
    }
}
