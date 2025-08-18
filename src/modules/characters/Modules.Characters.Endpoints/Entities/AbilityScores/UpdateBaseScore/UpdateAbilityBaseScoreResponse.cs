namespace Starlights.Modules.Characters.Endpoints.Entities.AbilityScores.UpdateBaseScore;

public sealed class UpdateAbilityBaseScoreResponse
{
    public Guid AbilityScoreId { get; set; }
    public int BaseScore { get; set; }
    public int AdditionalScore { get; set; }
    public int CalculatedScore { get; set; }
    public int CalculatedModifier { get; set; }
}
