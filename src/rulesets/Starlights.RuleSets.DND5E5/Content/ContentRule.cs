namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// The validated bounded Grant, Selection or numeric Rule contract.
/// </summary>
public sealed record ContentRule(string Id, string Kind, string? Target, string? OptionType,
    IReadOnlyList<string> OptionIds, int Count, string? Value, bool RequiresArmor);
