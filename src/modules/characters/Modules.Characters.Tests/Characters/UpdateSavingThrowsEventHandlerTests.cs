using FluentAssertions;
using Moq;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Services.EventHandlers;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class UpdateSavingThrowsEventHandlerTests
{
    private readonly Mock<IPersistence> _persistence = new();
    private readonly Mock<IElementsModuleQueries> _elements = new();
    private readonly Mock<ICharactersRepository> _characters = new();

    private readonly UpdateSavingThrowsEventHandler _sut;

    public UpdateSavingThrowsEventHandlerTests()
    {
        _persistence.Setup(p => p.GetRepository<ICharactersRepository>())
                    .Returns(_characters.Object);
        _persistence.Setup(p => p.SaveChangesAsync())
                    .ReturnsAsync(1);

        _sut = new UpdateSavingThrowsEventHandler(_persistence.Object, _elements.Object);
    }

    [TestMethod]
    public async Task HandleAsync_ShouldDoNothing_WhenCharacterNotFound()
    {
        // Arrange
        var abilityId = AbilityScoreId.New();
        var evt = new AbilityScoreUpdatedEvent
        {
            CharacterId = Guid.NewGuid(),
            AbilityScoreId = abilityId,
            NewAbilityScoreValue = 12,
            NewAbilityModifier = 1
        };

        _characters.Setup(r => r.GetCharacterAsync(evt.CharacterId))
                   .ReturnsAsync((Character?)null);

        // Act
        await _sut.HandleAsync(evt);

        // Assert
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task HandleAsync_ShouldUpdateMatchingSavingThrows_AndSave()
    {
        // Arrange
        var character = Character.Create("Test");
        var savingThrows = SavingThrowsComponent.Create(character.Id);
        character.AddComponent(savingThrows);

        var abilityId = AbilityScoreId.New();
        var regId = RegistrationId.New();
        var save = savingThrows.CreateSavingThrow(regId, "Strength Save", abilityId, "STR");

        // baseline
        save.AbilityScoreModifier.Should().Be(0);

        var evt = new AbilityScoreUpdatedEvent
        {
            CharacterId = character.Id,
            AbilityScoreId = abilityId,
            NewAbilityScoreValue = 18,
            NewAbilityModifier = 4
        };

        _characters.Setup(r => r.GetCharacterAsync(character.Id))
                   .ReturnsAsync(character);

        // Act
        await _sut.HandleAsync(evt);

        // Assert
        save.AbilityScoreModifier.Should().Be(4);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task HandleAsync_ShouldSave_WhenNoSavingThrowMatches()
    {
        // Arrange
        var character = Character.Create("Test");
        var savingThrows = SavingThrowsComponent.Create(character.Id);
        character.AddComponent(savingThrows);

        // unrelated saving throw with different ability
        var unrelatedAbility = AbilityScoreId.New();
        var regId = RegistrationId.New();
        var save = savingThrows.CreateSavingThrow(regId, "Dex Save", unrelatedAbility, "DEX");

        var evt = new AbilityScoreUpdatedEvent
        {
            CharacterId = character.Id,
            AbilityScoreId = AbilityScoreId.New(), // no match
            NewAbilityScoreValue = 14,
            NewAbilityModifier = 2
        };

        _characters.Setup(r => r.GetCharacterAsync(character.Id))
                   .ReturnsAsync(character);

        // Act
        await _sut.HandleAsync(evt);

        // Assert (unchanged)
        save.AbilityScoreModifier.Should().Be(0);
        _persistence.Verify(p => p.SaveChangesAsync(), Times.Once);
    }
}
