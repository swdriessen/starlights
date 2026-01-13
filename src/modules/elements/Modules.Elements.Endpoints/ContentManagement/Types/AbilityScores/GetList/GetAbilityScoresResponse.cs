using Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.GetList;

public sealed record GetAbilityScoresResponse(IEnumerable<AbilityScoreDataModel> Items);
