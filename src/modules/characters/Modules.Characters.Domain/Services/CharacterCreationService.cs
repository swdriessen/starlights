using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Domain.Services;

public sealed class CharacterCreationService : ICharacterCreationService
{
    public Character Create(string name)
    {
        var newCharacter = Character.Create(name);

        // initialize default components
        newCharacter.AddComponent(ProgressionComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(AbilitiesComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(SkillsComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(SavingThrowsComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(ProficiencyComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(AppearanceComponent.Create(newCharacter.Id));
        newCharacter.AddComponent(ClassComponent.Create(newCharacter.Id));

        return newCharacter;
    }
}

public interface ICharacterCreationService
{
    /// <summary>
    /// Creates a new character instance.
    /// </summary>
    Character Create(string name);
}