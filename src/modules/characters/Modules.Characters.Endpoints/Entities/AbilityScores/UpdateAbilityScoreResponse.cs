namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores;

public sealed class UpdateAbilityScoreResponse
{
    public Guid AbilityScoreId { get; set; }
    public int BaseScore { get; set; }
    public int AdditionalScore { get; set; }
    public int CalculatedScore { get; set; }
    public int CalculatedModifier { get; set; }
}
