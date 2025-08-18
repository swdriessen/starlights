namespace Starlights.Modules.Characters.Endpoints.Entities.Skills;

/// <summary>
/// The DTO for a skill from a character.
/// </summary>
public sealed class SkillDataModel
{
    public Guid SkillId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid AbilityScoreId { get; init; }
    public string? AbilityScoreAbbreviation { get; init; }
    public int AbilityScoreModifier { get; init; }
    public int AdditionalBonus { get; init; }
    public int CalculatedBonus { get; init; }
}
