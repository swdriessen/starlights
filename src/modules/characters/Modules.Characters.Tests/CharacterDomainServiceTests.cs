using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Services;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class CharacterDomainServiceTests
{
    private readonly Mock<ILogger<CharacterDomainService>> _loggerMock = new();
    private readonly Character _character;
    private readonly ProgressionComponent _progressionComponent;
    private readonly ClassComponent _classComponent;

    // SUT
    private readonly CharacterDomainService _domainService;

    public CharacterDomainServiceTests()
    {
        _character = Character.Create("Test");
        _progressionComponent = _character.AddComponent(new ProgressionComponent(_character.Id));
        _classComponent = _character.AddComponent(new ClassComponent(_character.Id));

        _domainService = new CharacterDomainService(_loggerMock.Object);
    }

    [TestMethod]
    public void AddCharacterClass_ShouldCreateClassAndUpdateCharacterLevel()
    {
        // Arrange
        var registration = Registration.Create(_character.Id, default, "Wizard", "Class");

        // Act
        _domainService.AddCharacterClass(_character, registration, "Wizard");

        // Assert
        _classComponent.Classes.Should().ContainSingle();
        var wizard = _classComponent.Classes.Single();
        wizard.Level.Should().Be(1);
        _progressionComponent.CharacterLevel.Should().Be(1);
    }

    [TestMethod]
    public void AddCharacterClass_Twice_ShouldUpdateCharacterLevel()
    {
        // Arrange
        var reg1 = Registration.Create(_character.Id, default, "Wizard", "Class");
        var reg2 = Registration.Create(_character.Id, default, "Fighter", "Class");

        // Act
        _domainService.AddCharacterClass(_character, reg1, "Wizard");
        _domainService.AddCharacterClass(_character, reg2, "Fighter");

        // Assert
        _progressionComponent.CharacterLevel.Should().Be(2); // two classes level 1 each
    }
}
