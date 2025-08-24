namespace Starlights.Modules.Characters.Endpoints.Characters.SavingThrows;

/// <summary>
/// The DTO for a saving throw from a character.
/// </summary>
public sealed class SavingThrowDataModel
{
    public Guid SavingThrowId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid AbilityScoreId { get; init; }
    public string? AbilityScoreAbbreviation { get; init; }
    public int AbilityScoreModifier { get; init; }
    public int AdditionalBonus { get; init; }
    public int CalculatedBonus { get; init; }
}
