using FluentAssertions;
using Moq;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Tests.Registrations;

[TestClass]
public sealed class RegistrationProcessingContextTests
{
    [TestMethod]
    public void Registration_Default_ProgressionOrigin_IsNull()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");

        // Assert
        registration.ProgressionOriginRegistrationId.Should().BeNull();
    }

    [TestMethod]
    public void Registration_SetProgressionOrigin_SetsValue()
    {
        // Arrange
        var characterId = CharacterId.New();
        var parent = Registration.Create(characterId, new ElementId(Guid.NewGuid()), "Parent", "Type");
        var child = Registration.Create(characterId, new ElementId(Guid.NewGuid()), "Child", "Type");

        // Act
        child.SetProgressionOrigin(parent.Id);

        // Assert
        child.ProgressionOriginRegistrationId.Should().Be(parent.Id);
    }

    [TestMethod]
    public void GetProgressionLevel_UsesOriginClassLevel_WhenOriginMatchesClass()
    {
        // Arrange
        var (character, progression, classes) = CreateCharacterWithComponents();
        progression.SetCharacterLevel(7); // character total level, should be ignored when origin resolves

        var classReg = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Fighter", "Class");
        var featureReg = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature", "Class Feature");
        featureReg.SetProgressionOrigin(classReg.Id);

        var fighter = classes.CreateClass(classReg.Id, classReg.AssociatedElementName);
        classes.SetClassLevel(fighter.Id, 3);

        var ctx = new ProcessingContext(featureReg, character, Mock.Of<IPersistence>());

        // Act
        var level = ctx.GetProgressionLevel(featureReg);

        // Assert
        level.Should().Be(3, "origin class level should be used when progression origin is set and resolves to a class");
    }

    [TestMethod]
    public void GetProgressionLevel_FallsBackToCharacterLevel_WhenOriginNotFound()
    {
        // Arrange
        var (character, progression, _) = CreateCharacterWithComponents();
        progression.SetCharacterLevel(5);

        var unrelatedReg = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Other", "Type");
        var reg = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Something", "Type");
        reg.SetProgressionOrigin(unrelatedReg.Id); // no class exists with this registration id

        var ctx = new ProcessingContext(reg, character, Mock.Of<IPersistence>());

        // Act
        var level = ctx.GetProgressionLevel(reg);

        // Assert
        level.Should().Be(5, "should fall back to character level when origin cannot be resolved");
    }

    [TestMethod]
    public void GetProgressionLevel_UsesCharacterLevel_WhenNoOrigin()
    {
        // Arrange
        var (character, progression, _) = CreateCharacterWithComponents();
        progression.SetCharacterLevel(4);

        var reg = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Something", "Type");
        // no origin set

        var ctx = new ProcessingContext(reg, character, Mock.Of<IPersistence>());

        // Act
        var level = ctx.GetProgressionLevel(reg);

        // Assert
        level.Should().Be(4);
    }

    private static (Character Character, ProgressionComponent Progression, ClassComponent Classes) CreateCharacterWithComponents()
    {
        var character = Character.Create("Test");
        var progression = ProgressionComponent.Create(character.Id);
        var classes = ClassComponent.Create(character.Id);
        character.AddComponent(progression);
        character.AddComponent(classes);
        return (character, progression, classes);
    }
}
