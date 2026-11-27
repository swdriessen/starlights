using Starlights.RuleSets.DND5E5.Content;

namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// The current build decisions and independently evaluated mechanics.
/// </summary>
public sealed record BuildView(IReadOnlyList<Selection> Selections, IReadOnlyList<AbilityResult> Abilities,
    IReadOnlyList<Statistic> Statistics, IReadOnlyList<Attack> Attacks, IReadOnlyList<SheetEntry> Entries,
    bool BuildComplete, bool SheetReady, IReadOnlyList<int> AbilityInputOptions, int? PointsSpent);
