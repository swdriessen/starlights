using Starlights.Modules.Characters.Domain.Building;
using Starlights.RuleSets.DND5E5.Building;

namespace Starlights.Modules.Characters.Endpoints.Building;

/// <summary>
/// A replaceable calculation boundary for exercising accepted work and interruption.
/// </summary>
public interface ICharacterSheetCalculation
{
    /// <summary>
    /// Calculates from an immutable accepted snapshot, without a browser cancellation token.
    /// </summary>
    Task<BuildView> CalculateAsync(BuildRules rules, ICharacterComposition composition);
}
