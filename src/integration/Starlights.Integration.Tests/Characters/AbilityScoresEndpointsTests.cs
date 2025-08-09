using System.Net;
using FluentAssertions;
using Starlights.Integration.Tests.Core;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class AbilityScoresEndpointsTests
{
    private readonly IntegrationHost _integration;
    private Guid _characterId;
    private string _characterName = string.Empty;

    public AbilityScoresEndpointsTests()
    {
        _integration = IntegrationHost.CreateBuilder().Build();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        var client = _integration.CreateClient();

        await client.InitializeElementsAsync();

        // Get creation options
        var optionsResponse = await client.GetCharacterCreationOptionsAsync(CancellationToken.None);
        optionsResponse.Options.Should().NotBeEmpty();

        // Get portrait options
        var portraitsResponse = await client.GetCharacterPortraitOptionsAsync(CancellationToken.None);
        portraitsResponse.Portraits.Should().NotBeEmpty();

        _characterName = $"Integration Test Character {Guid.NewGuid()}";
        var characterResponse = await client.CreateCharacterAsync(optionsResponse.Options[0].Id, _characterName, portraitsResponse.Portraits[0].Url, CancellationToken.None);
        _characterId = characterResponse.Id;

        await client.WaitForAbilityScoresAsync(_characterId, minCount: 1, timeout: TimeSpan.FromMilliseconds(3000), CancellationToken.None);
    }

    [TestMethod]
    public async Task GetAbilities_Returns_DetailedScores()
    {
        // Arrange
        var client = _integration.CreateClient();

        // Act
        var abilities = await client.GetAbilityScoresAsync(_characterId, CancellationToken.None);

        // Assert
        abilities.Should().NotBeNull();
        abilities.AbilityScores.Should().NotBeEmpty();

        var first = abilities.AbilityScores[0];
        first.AbilityScoreId.Should().NotBe(Guid.Empty);
        first.Name.Should().NotBeNullOrWhiteSpace();
        first.Abbreviation.Should().NotBeNullOrWhiteSpace();
        first.CalculatedScore.Should().Be(first.BaseScore + first.AdditionalScore);
        first.CalculatedModifier.Should().Be((int)Math.Floor((first.CalculatedScore - 10) / 2.0));
    }

    [TestMethod]
    public async Task UpdateBaseScore_Updates_CalculatedFields()
    {
        // Arrange
        var client = _integration.CreateClient();
        var abilitiesBefore = await client.GetAbilityScoresAsync(_characterId, CancellationToken.None);
        var target = abilitiesBefore.AbilityScores[0];
        const int newBaseScore = 18;
        const int expectedScore = 18;
        const int expectedModifier = 4;

        // Act
        var postResponse = await client.SetAbilityBaseScoreAsync(_characterId, target.AbilityScoreId, newBaseScore, CancellationToken.None);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await client.GetAbilityScoresAsync(_characterId, CancellationToken.None);
        var updated = abilitiesAfter.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.BaseScore.Should().Be(newBaseScore);
        updated.CalculatedScore.Should().Be(expectedScore);
        updated.CalculatedModifier.Should().Be(expectedModifier);
    }

    [TestMethod]
    public async Task UpdateAdditionalScore_Updates_CalculatedFields()
    {
        // Arrange
        var client = _integration.CreateClient();
        var abilitiesBefore = await client.GetAbilityScoresAsync(_characterId, CancellationToken.None);
        var target = abilitiesBefore.AbilityScores[0];
        const int newAdditionalScore = 2;
        const int expectedScore = 12;
        const int expectedModifier = 1;

        // Act
        var postResponse = await client.SetAbilityAdditionalScoreAsync(_characterId, target.AbilityScoreId, newAdditionalScore, CancellationToken.None);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var abilitiesAfter = await client.GetAbilityScoresAsync(_characterId, CancellationToken.None);
        var updated = abilitiesAfter.AbilityScores.First(a => a.AbilityScoreId == target.AbilityScoreId);

        updated.AdditionalScore.Should().Be(newAdditionalScore);
        updated.CalculatedScore.Should().Be(expectedScore);
        updated.CalculatedModifier.Should().Be(expectedModifier);
    }
}
