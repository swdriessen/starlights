namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;

/// <summary>
/// DTO representing a character class.
/// </summary>
public sealed class CharacterClassDataModel
{
    public Guid CharacterClassId { get; init; }
    public Guid RegistrationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Level { get; init; }
    public bool IsPrimary { get; init; }
}
