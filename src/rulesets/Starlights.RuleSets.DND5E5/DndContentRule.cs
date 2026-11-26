namespace Starlights.RuleSets.DND5E5;

/// <summary>The validated bounded Grant, Selection or numeric Rule contract.</summary>
public sealed record DndContentRule(string Id, string Kind, string? Target, string? OptionType,
    IReadOnlyList<string> OptionIds, int Count, string? Value, bool RequiresArmor);
