using Starlights.Modules.Characters.Domain.Building;
using Starlights.RuleSets.DND5E5;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterSheetCalculation : ICharacterSheetCalculation
{
    public Task<DndBuildView> CalculateAsync(DndBuildRules rules, ICharacterComposition composition)
        => Task.FromResult(rules.View(composition));
}
