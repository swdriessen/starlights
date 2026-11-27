using Starlights.Modules.Characters.Domain.Building;
using Starlights.RuleSets.DND5E5.Building;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterSheetCalculation : ICharacterSheetCalculation
{
    public Task<BuildView> CalculateAsync(BuildRules rules, ICharacterComposition composition)
        => Task.FromResult(rules.View(composition));
}
