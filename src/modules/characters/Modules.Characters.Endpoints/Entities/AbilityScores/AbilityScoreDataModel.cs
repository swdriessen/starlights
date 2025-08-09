namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores;

/// <summary>
/// The DTO for an ability score.
/// </summary>
public sealed class AbilityScoreDataModel
{
    public Guid AbilityScoreId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
    public int BaseScore { get; init; }
    public int AdditionalScore { get; init; }
    public int CalculatedScore { get; init; }
    public int CalculatedModifier { get; init; }
}
