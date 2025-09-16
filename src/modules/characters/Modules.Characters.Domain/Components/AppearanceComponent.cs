using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Domain.Components;

public sealed class AppearanceComponent: CharacterComponentBase
{
    public AppearanceComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    public string? PortraitUrl { get; set; }
}
