namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// A numeric result with occurrence-qualified evidence; null means unresolved, never zero.
/// </summary>
public sealed record Statistic(string Id, string Name, int? Value, IReadOnlyList<string> Sources);
