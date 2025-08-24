using FluentAssertions;
using Moq;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public class ClassComponentTests
{
    [TestMethod]
    public void AddClass_FirstClass_BecomesPrimary()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = new ClassComponent(character.Id);

        // Act
        var newClass = component.AddClass(RegistrationId.New(), "Wizard", Mock.Of<IEventRecorder>());

        // Assert
        newClass.IsPrimary.Should().BeTrue();
        component.Classes.Should().ContainSingle().Which.Should().BeSameAs(newClass);
    }

    [TestMethod]
    public void AddClass_RaisesCharacterClassCreatedEvent()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = new ClassComponent(character.Id);
        var recorder = new Mock<IEventRecorder>();

        // Act
        var newClass = component.AddClass(RegistrationId.New(), "Wizard", recorder.Object);

        // Assert
        recorder.Verify(x => x.AddDomainEvent(
            It.Is<CharacterClassCreatedEvent>(e => e.CharacterId == character.Id && e.ClassId == newClass.Id)),
            Times.Once);
    }

    [TestMethod]
    public void AddClass_SubsequentClasses_NotPrimary()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = new ClassComponent(character.Id);
        var recorder = new Mock<IEventRecorder>();

        // Act
        var first = component.AddClass(RegistrationId.New(), "Wizard", recorder.Object);
        var second = component.AddClass(RegistrationId.New(), "Fighter", recorder.Object);

        // Assert
        first.IsPrimary.Should().BeTrue();
        second.IsPrimary.Should().BeFalse();
        component.Classes.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetCombinedLevel_ReturnsSumOfLevels()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = new ClassComponent(character.Id);
        var recorder = Mock.Of<IEventRecorder>();

        // Act
        _ = component.AddClass(RegistrationId.New(), "Wizard", recorder); // level 1 by default
        _ = component.AddClass(RegistrationId.New(), "Fighter", recorder); // level 1 by default

        // Assert
        component.GetCombinedLevel().Should().Be(2);
    }
}
