namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// A concise authored reminder, not an executed gameplay action.
/// </summary>
public sealed record SheetEntry(string Name, string? Description, string? Usage, string Id = "");
