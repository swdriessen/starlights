namespace Starlights.Modules.Characters.Endpoints.Characters.AbilityScores.UpdateAdditionalScore;

public sealed class UpdateAbilityAdditionalScoreResponse
{
    public Guid AbilityScoreId { get; set; }
    public int BaseScore { get; set; }
    public int AdditionalScore { get; set; }
    public int CalculatedScore { get; set; }
    public int CalculatedModifier { get; set; }
}
