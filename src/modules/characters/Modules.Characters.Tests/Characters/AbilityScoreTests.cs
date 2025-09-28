using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public class AbilityScoreTests
{
    [TestMethod]
    public void Create_Defaults_AreCorrect()
    {
        // Arrange
        var regId = RegistrationId.New();

        // Act
        var score = AbilityScore.Create(regId, "Strength", "STR");

        // Assert
        score.BaseScore.Should().Be(10);
        score.AdditionalScore.Should().Be(0);
        score.CalculatedScore.Should().Be(10);
        score.CalculatedModifier.Should().Be(0);
    }

    [TestMethod]
    public void UpdateBaseScore_Recalculates_ReturnsTrue()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Dexterity", "DEX");

        // Act
        var updated = score.UpdateBaseScore(18);

        // Assert
        updated.Should().BeTrue();
        score.BaseScore.Should().Be(18);
        score.CalculatedScore.Should().Be(18);
        score.CalculatedModifier.Should().Be(4);
    }

    [TestMethod]
    public void UpdateBaseScore_NoChange_ReturnsFalse()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Dexterity", "DEX");
        score.UpdateBaseScore(14).Should().BeTrue();
        var currentCalculatedScore = score.CalculatedScore;
        var currentModifier = score.CalculatedModifier;

        // Act
        var updated = score.UpdateBaseScore(14);

        // Assert
        updated.Should().BeFalse();
        score.CalculatedScore.Should().Be(currentCalculatedScore);
        score.CalculatedModifier.Should().Be(currentModifier);
    }

    [TestMethod]
    public void UpdateAdditionalScore_Recalculates_ReturnsTrue()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Constitution", "CON");

        // Act
        var updated = score.UpdateAdditionalScore(2);

        // Assert
        updated.Should().BeTrue();
        score.AdditionalScore.Should().Be(2);
        score.CalculatedScore.Should().Be(12);
        score.CalculatedModifier.Should().Be(1);
    }

    [TestMethod]
    public void UpdateAdditionalScore_NoChange_ReturnsFalse()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Constitution", "CON");
        score.UpdateAdditionalScore(3).Should().BeTrue();
        var currentCalculatedScore = score.CalculatedScore;
        var currentModifier = score.CalculatedModifier;

        // Act
        var updated = score.UpdateAdditionalScore(3);

        // Assert
        updated.Should().BeFalse();
        score.CalculatedScore.Should().Be(currentCalculatedScore);
        score.CalculatedModifier.Should().Be(currentModifier);
    }

    [TestMethod]
    [DataRow(1, -5)]
    [DataRow(2, -4)]
    [DataRow(3, -4)]
    [DataRow(4, -3)]
    [DataRow(5, -3)]
    [DataRow(6, -2)]
    [DataRow(7, -2)]
    [DataRow(8, -1)]
    [DataRow(9, -1)]
    [DataRow(10, 0)]
    [DataRow(11, 0)]
    [DataRow(12, 1)]
    [DataRow(13, 1)]
    [DataRow(14, 2)]
    [DataRow(15, 2)]
    [DataRow(16, 3)]
    [DataRow(17, 3)]
    [DataRow(18, 4)]
    [DataRow(19, 4)]
    [DataRow(20, 5)]
    [DataRow(21, 5)]
    [DataRow(22, 6)]
    [DataRow(23, 6)]
    [DataRow(24, 7)]
    [DataRow(25, 7)]
    [DataRow(26, 8)]
    [DataRow(27, 8)]
    [DataRow(28, 9)]
    [DataRow(29, 9)]
    [DataRow(30, 10)]
    public void Modifier_Table_Matches(int totalScore, int expectedMod)
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Intelligence", "INT");

        // Act
        score.UpdateBaseScore(10);
        score.UpdateAdditionalScore(totalScore - 10);

        // Assert
        score.CalculatedScore.Should().Be(totalScore);
        score.CalculatedModifier.Should().Be(expectedMod);
    }
}
