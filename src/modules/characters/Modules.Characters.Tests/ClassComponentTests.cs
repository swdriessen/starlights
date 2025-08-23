using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Registrations;

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
        var cls = component.AddClass(RegistrationId.New(), "Wizard");

        // Assert
        cls.IsPrimary.Should().BeTrue();
        component.Classes.Should().ContainSingle().Which.Should().BeSameAs(cls);
    }

    [TestMethod]
    public void AddClass_SubsequentClasses_NotPrimary()
    {
        // Arrange
        var character = Character.Create("Test");
        var component = new ClassComponent(character.Id);

        // Act
        var first = component.AddClass(RegistrationId.New(), "Wizard");
        var second = component.AddClass(RegistrationId.New(), "Fighter");

        // Assert
        first.IsPrimary.Should().BeTrue();
        second.IsPrimary.Should().BeFalse();
        component.Classes.Should().HaveCount(2);
    }
}
