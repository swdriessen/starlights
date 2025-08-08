using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Modules.Characters.Tests;

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
    public void UpdateBaseScore_Recalculates()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Dexterity", "DEX");

        // Act
        score.UpdateBaseScore(18);

        // Assert
        score.BaseScore.Should().Be(18);
        score.CalculatedScore.Should().Be(18);
        score.CalculatedModifier.Should().Be(4);
    }

    [TestMethod]
    public void UpdateAdditionalScore_Recalculates()
    {
        // Arrange
        var regId = RegistrationId.New();
        var score = AbilityScore.Create(regId, "Constitution", "CON");

        // Act
        score.UpdateAdditionalScore(2);

        // Assert
        score.AdditionalScore.Should().Be(2);
        score.CalculatedScore.Should().Be(12);
        score.CalculatedModifier.Should().Be(1);
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
