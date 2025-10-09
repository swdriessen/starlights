using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Integration.Tests.Characters;

[TestClass]
public sealed class AbilityScoresEndpointsTests : IntegrationTestBase
{
    private IntegrationHost _integration = default!;
    private AbilityScoreDriver _abilityScoreDriver = default!;

    [TestInitialize]
    public async Task Initialize()
    {
        _integration = IntegrationHost.CreateBuilder()
            .WithTestContext(TestContext)
            .Build();

        _abilityScoreDriver = _integration.GetDriver<AbilityScoreDriver>();

        await _integration.InitializeElements();
        await _integration.GetDriver<CharacterCreationDriver>()
            .CreateCharacterAsync();
    }

    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task GetAbilities_Returns_DefaultScores()
    {
        // Arrange
        const int expectedScore = 10;
        const int expectedModifier = 0;

        // Act
        var abilityScores = await _abilityScoreDriver.GetAbilityScores();

        // Assert
        abilityScores.Should().HaveCount(6, "a new character should have six standard ability scores");
        abilityScores.Should().AllSatisfy(score =>
        {
            score.AbilityScoreId.Should().NotBe(Guid.Empty);
            score.Name.Should().NotBeNullOrWhiteSpace();
            score.Abbreviation.Should().NotBeNullOrWhiteSpace();
            score.CalculatedScore.Should().Be(expectedScore);
            score.CalculatedModifier.Should().Be(expectedModifier);
        });
    }

    [DataRow("Strength")]
    [DataRow("Dexterity")]
    [DataRow("Constitution")]
    [DataRow("Intelligence")]
    [DataRow("Wisdom")]
    [DataRow("Charisma")]
    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task UpdateBaseScore_Updates_CalculatedFields(string abilityName)
    {
        // Arrange
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);

        // Act
        await _abilityScoreDriver.UpdateAbilityScoreBase(score.AbilityScoreId, 18);

        // Assert
        var updatedScore = await _abilityScoreDriver.GetAbilityScore(abilityName);
        updatedScore.BaseScore.Should().Be(18);
        updatedScore.CalculatedScore.Should().Be(18);
        updatedScore.CalculatedModifier.Should().Be(4);
    }

    [DataRow("Strength")]
    [DataRow("Dexterity")]
    [DataRow("Constitution")]
    [DataRow("Intelligence")]
    [DataRow("Wisdom")]
    [DataRow("Charisma")]
    [TestMethod]
    [Timeout(TestConstants.Timeout, CooperativeCancellation = true)]
    public async Task UpdateAdditionalScore_Updates_CalculatedFields(string abilityName)
    {
        // Arrange
        var score = await _abilityScoreDriver.GetAbilityScore(abilityName);

        // Act
        await _abilityScoreDriver.UpdateAdditionalValue(score.AbilityScoreId, 2);

        // Assert
        var updatedScore = await _abilityScoreDriver.GetAbilityScore(abilityName);
        updatedScore.AdditionalScore.Should().Be(2);
        updatedScore.CalculatedScore.Should().Be(12);
        updatedScore.CalculatedModifier.Should().Be(1);
    }
}
