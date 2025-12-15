using AwesomeAssertions;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public class CharacterClassTests
{
    [TestMethod]
    public void Create_SetsRegistrationAndName_Defaults()
    {
        // Arrange
        var reg = RegistrationId.New();

        // Act
        var cc = CharacterClass.Create(reg, "Wizard");

        // Assert
        cc.Registration.Should().Be(reg);
        cc.Name.Should().Be("Wizard");
        cc.Level.Should().Be(1);
        cc.IsPrimary.Should().BeFalse();
    }

    [TestMethod]
    public void SetPrimary_UpdatesFlag()
    {
        // Arrange
        var cc = CharacterClass.Create(RegistrationId.New(), "Wizard");

        // Act
        cc.SetPrimary();

        // Assert
        cc.IsPrimary.Should().BeTrue();

        // Act 2
        cc.SetPrimary(false);
        cc.IsPrimary.Should().BeFalse();
    }

    [TestMethod]
    public void UpdateLevel_SetsLevel()
    {
        // Arrange
        var cc = CharacterClass.Create(RegistrationId.New(), "Wizard");

        // Act
        cc.UpdateLevel(5);

        // Assert
        cc.Level.Should().Be(5);
    }
}
