using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests;

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
}
