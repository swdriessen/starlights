namespace Starlights.RuleSets.DND5E5;

/// <summary>A concise authored reminder, not an executed gameplay action.</summary>
public sealed record DndSheetEntry(string Name, string? Description, string? Usage, string Id = "");
