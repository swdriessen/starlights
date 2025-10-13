using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public class ClassComponentTests
{
    [TestMethod]
    public void CreateClass_FirstClass_BecomesPrimary()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act
        var newClass = component.CreateClass(RegistrationId.New(), "Wizard");

        // Assert
        newClass.IsPrimary.Should().BeTrue();
        component.Classes.Should().ContainSingle().Which.Should().BeSameAs(newClass);
    }

    [TestMethod]
    public void CreateClass_RaisesCharacterClassCreatedEvent()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act
        var newClass = component.CreateClass(RegistrationId.New(), "Wizard");

        // Assert
        component.DomainEvents.Should().HaveCount(1);
        component.DomainEvents.Single().Should().BeOfType<CharacterClassCreatedEvent>();
        component.DomainEvents.OfType<CharacterClassCreatedEvent>()
            .Should().ContainSingle(x => x.CharacterId == character.Id && x.ClassId == newClass.Id);
    }

    [TestMethod]
    public void CreateClass_SubsequentClasses_NotPrimary()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act
        var first = component.CreateClass(RegistrationId.New(), "Wizard");
        var second = component.CreateClass(RegistrationId.New(), "Fighter");

        // Assert
        first.IsPrimary.Should().BeTrue();
        second.IsPrimary.Should().BeFalse();
        component.Classes.Should().HaveCount(2);
    }

    [TestMethod]
    public void CalculateCharacterLevel_NoClasses_ReturnsZero()
    {
        // Arrange
        var character = Character.Create("Empty");
        var component = ClassComponent.Create(character.Id);

        // Act
        var total = component.CalculateCharacterLevel();

        // Assert
        total.Should().Be(0);
    }
    [TestMethod]
    public void CalculateCharacterLevel_ReturnsSumOfClassLevels()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act
        _ = component.CreateClass(RegistrationId.New(), "Wizard"); // level 1 by default
        _ = component.CreateClass(RegistrationId.New(), "Fighter"); // level 1 by default

        // Assert
        component.CalculateCharacterLevel().Should().Be(2);
    }

    [TestMethod]
    public void IsMulticlass_False_WithSingleClass_True_WithTwo()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act & Assert
        component.IsMulticlass.Should().BeFalse();
        _ = component.CreateClass(RegistrationId.New(), "Wizard");
        component.IsMulticlass.Should().BeFalse();
        _ = component.CreateClass(RegistrationId.New(), "Fighter");
        component.IsMulticlass.Should().BeTrue();
    }

    [TestMethod]
    public void CalculateCharacterLevel_WithUpdatedLevels_SumsCorrectly()
    {
        // Arrange
        var character = Character.Create("Hero");
        var component = ClassComponent.Create(character.Id);
        var c1 = component.CreateClass(RegistrationId.New(), "Wizard");
        var c2 = component.CreateClass(RegistrationId.New(), "Fighter");
        var c3 = component.CreateClass(RegistrationId.New(), "Rogue");

        // Act
        c1.UpdateLevel(3); // now 3
        c2.UpdateLevel(5); // now 5
        c3.UpdateLevel(2); // now 2
        var total = component.CalculateCharacterLevel();

        // Assert
        total.Should().Be(3 + 5 + 2);
    }

    [TestMethod]
    public void CreateMultipleClasses_RaisesDomainEventPerClass()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);

        // Act
        var first = component.CreateClass(RegistrationId.New(), "Wizard");
        var second = component.CreateClass(RegistrationId.New(), "Fighter");

        // Assert
        component.DomainEvents.Should().HaveCount(2);
        component.DomainEvents.OfType<CharacterClassCreatedEvent>()
            .Should().HaveCount(2)
            .And.Contain(e => e.ClassId == first.Id)
            .And.Contain(e => e.ClassId == second.Id);
    }

    [TestMethod]
    public void LevelUpClass_ClassExists_UpdatesLevel()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);
        var characterClass = component.CreateClass(RegistrationId.New(), "Wizard");
        characterClass.Level.Should().Be(1); // default

        // Act
        component.SetClassLevel(characterClass.Id, 5);

        // Assert
        characterClass.Level.Should().Be(5);
    }

    [TestMethod]
    public void LevelUpClass_ClassNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);
        _ = component.CreateClass(RegistrationId.New(), "Wizard");
        var missingId = CharacterClassId.New();

        // Act
        var action = () => component.SetClassLevel(missingId, 2);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void LevelUpClass_NewLevelZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);
        var characterClass = component.CreateClass(RegistrationId.New(), "Wizard");

        // Act
        var action = () => component.SetClassLevel(characterClass.Id, 0);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void LevelUpClass_NewLevelNegative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);
        var characterClass = component.CreateClass(RegistrationId.New(), "Wizard");

        // Act
        var action = () => component.SetClassLevel(characterClass.Id, -5);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void LevelUpClass_CalledMultipleTimes_SetsLatestLevel()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = ClassComponent.Create(character.Id);
        var characterClass = component.CreateClass(RegistrationId.New(), "Wizard");

        // Act
        component.SetClassLevel(characterClass.Id, 2);
        component.SetClassLevel(characterClass.Id, 7);

        // Assert
        characterClass.Level.Should().Be(7);
    }
}
