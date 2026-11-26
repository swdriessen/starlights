namespace Starlights.RuleSets.DND5E5;

/// <summary>The current build decisions and independently evaluated mechanics.</summary>
public sealed record DndBuildView(IReadOnlyList<DndSelection> Selections, IReadOnlyList<DndAbilityResult> Abilities,
    IReadOnlyList<DndStatistic> Statistics, IReadOnlyList<DndAttack> Attacks, IReadOnlyList<DndSheetEntry> Entries,
    bool BuildComplete, bool SheetReady, IReadOnlyList<int> AbilityInputOptions);
