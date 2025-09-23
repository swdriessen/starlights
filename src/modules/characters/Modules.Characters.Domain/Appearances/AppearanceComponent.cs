using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;

namespace Starlights.Modules.Characters.Domain.Appearances;

public sealed class AppearanceComponent : CharacterComponentBase
{
    public AppearanceComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {

    }

    public string? PortraitUrl { get; set; }

    public static AppearanceComponent Create(CharacterId parentCharacter)
    {
        return new AppearanceComponent(parentCharacter);
    }
}
