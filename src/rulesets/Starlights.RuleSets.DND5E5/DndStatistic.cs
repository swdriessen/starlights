namespace Starlights.RuleSets.DND5E5;

/// <summary>A numeric result with occurrence-qualified evidence; null means unresolved, never zero.</summary>
public sealed record DndStatistic(string Id, string Name, int? Value, IReadOnlyList<string> Sources);
