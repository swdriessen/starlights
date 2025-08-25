using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Domain;
using Starlights.Platform.SourceGenerators.Entities.Attributes;

namespace Starlights.Modules.Characters.Domain.Components;

[Entity]
public abstract class CharacterComponentBase : EntityBase<CharacterComponentBaseId>
{
    protected CharacterComponentBase(CharacterId parentCharacter)
        : base(CharacterComponentBaseId.New())
    {
        ParentCharacter = parentCharacter;
    }

    /// <summary>
    /// Gets the unique identifier of the parent character that this component belongs to.
    /// </summary>
    public CharacterId ParentCharacter { get; protected set; }
}
